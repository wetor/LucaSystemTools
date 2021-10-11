using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using LucaSystemTools;

namespace ProtScript.Entity
{
    /// <summary>
    /// 指令行，包含opcode、CodeInfo、ParamData、goto等
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CodeLine
    {
        // 行号
        public int index;
        // 所在脚本位置
        public int position;
        // opcode
        public byte opcodeIndex;
        // 参数的已知类型
        public ParamType[] paramTypes;
        // 此行的字节数据
        public byte[] bytes;

        [JsonProperty]
        public string opcode { get; set; }
        // 资源引用信息（待定）
        [JsonProperty]
        public CodeInfo info { get; set; }
        // 实际参数列表
        [JsonProperty]
        public List<ParamData> paramDatas { get; set; }
        // 是否存在Position参数
        [JsonProperty]
        public bool isPosition { get; set; } = false;
        // 是否为跳转目标
        [JsonProperty]
        public bool isLabel { get; set; } = false;
        // 标签名
        [JsonProperty]
        public string label { get; set; }

        public CodeLine() {}
        public CodeLine(int index, int position)
        {
            this.index = index;
            this.position = position;
        }
        /// <summary>
        /// 可导入lua的加载，转成CodeLine
        /// </summary>
        /// <param name="codeStr"></param>
        public CodeLine(string codeStr)
        {
            List<string> tokens = new List<string>();
            string token = "";
            bool isStr = false;
            int i = 0;
            while (i < codeStr.Length)
            {
                if (codeStr[i] == '\"')
                {
                    isStr = !isStr;
                }
                if (!isStr &&
                    (codeStr[i] == '(' || codeStr[i] == '{' ||
                    codeStr[i] == ')' || codeStr[i] == '}' ||
                    codeStr[i] == ',' || codeStr[i] == ' '))
                {
                    token = token.Trim();
                    if (token != "")
                    {
                        tokens.Add(token);
                        token = "";
                    }
                    if (!(codeStr[i] == ',' || codeStr[i] == ' '))
                    {
                        tokens.Add(codeStr[i].ToString());
                    }
                }
                else
                {
                    token += codeStr[i];
                }
                i++;
            }
            token = token.Trim();
            if (token != "")
            {
                tokens.Add(token);
            }

            paramDatas = new List<ParamData>();
            bool readParam = false;
            string tempStr = "";
            i = 0;
            while (i < tokens.Count)
            {
                if (!readParam)
                {
                    if (tokens[i][0] == ':')
                    {
                        isLabel = true;
                        label = tokens[i].Substring(2, tokens[i].Length - 4);
                        i++; // (
                    }
                    if (opcode == null)
                    {
                        opcode = tokens[i];
                    }
                    if (tokens[i] == "{")
                    {
                        i++; // {
                        CodeInfo info = new CodeInfo(0);
                        List<UInt16> datas = new List<UInt16>();
                        while (tokens[i] != "}")
                        {
                            datas.Add(Convert.ToUInt16(tokens[i++]));
                        }
                        info.count = datas.Count;
                        info.data = datas.ToArray();
                        this.info = info;
                        readParam = true;
                        // i = }
                    }
                }
                else
                {
                    ParamData param = new ParamData();
                    if (tokens[i] == ")")
                    {
                        break;
                    }
                    else if (tokens[i] == "(")
                    {
                        i++;
                        DataType type = ScriptEntity.ToDataType(tokens[i++]);
                        i++;
                        if (tokens[i][0] == '\"')
                        {
                            tempStr = tokens[i].Substring(1, tokens[i].Length - 2).Replace(@"\n", "\n");;
                            param = ScriptEntity.ToParamData(tempStr, type);
                            if (type == DataType.Position)
                            {
                                isPosition = true;
                                param.valueOp = tempStr;
                            }
                        }
                        else
                        {
                            param = ScriptEntity.ToParamData(tokens[i], type);
                        }
                        // i = )
                    }
                    else if (tokens[i].Length >= 4 && tokens[i].Substring(0, 2) == "0x") 
                    {
                        param = ScriptEntity.ToParamData(tokens[i], DataType.Byte2);
                    }
                    else
                    {
                        param = ScriptEntity.ToParamData(tokens[i], DataType.UInt16);
                    }
                   
                    paramDatas.Add(param);
                }
                i++;
            }
        }
        /// <summary>
        /// 此语句为跳转目标，设置唯一id
        /// </summary>
        /// <param name="name"></param>
        public void SetLabel(string name)
        {
            isLabel = true;
            label = name;
        }
        /// <summary>
        /// 标准lua脚本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string codeStr = "";
            if (isLabel)
            {
                codeStr += "::" + label + ":: ";
            }
            codeStr += opcode + "(" + info.ToString();
            foreach (var param in paramDatas)
            {
                if (ScriptEntity.IsString(param.type))
                {
                    codeStr += ", " + "\"" + param.valueString.Replace("\n", @"\n") + "\"";
                }
                else if (param.type == DataType.Position)
                {
                    codeStr += ", " + "\"" + param.valueString + "\"";
                }
                else
                {
                    codeStr += ", " + param.valueString;
                }

            }
            codeStr += ")";
            return codeStr;

        }
        /// <summary>
        /// 带类型标记的字符串
        /// </summary>
        /// <returns></returns>
        public string ToStringAll()
        {
            string codeStr = "";
            if (isLabel)
            {
                codeStr += "::" + label + ":: ";
            }
            codeStr += opcode + "(" + info.ToString();
            foreach (var param in paramDatas)
            {
                switch (param.type)
                {
                    case DataType.Byte:
                    case DataType.Byte2:
                    case DataType.Byte3:
                    case DataType.Byte4:
                    case DataType.UInt16:
                        codeStr += ", " + param.valueString;
                        break;
                    case DataType.Int16:
                        codeStr += ", (short)" + param.valueString;
                        break;
                    case DataType.UInt32:
                        codeStr += ", (uint)" + param.valueString;
                        break;
                    case DataType.Int32:
                        codeStr += ", (int)" + param.valueString;
                        break;
                    case DataType.StringUnicode:
                        codeStr += ", " + "($u)\"" + param.valueString.Replace("\n", @"\n") + "\"";
                        break;
                    case DataType.StringSJIS:
                        codeStr += ", " + "($j)\"" + param.valueString.Replace("\n", @"\n") + "\"";
                        break;
                    case DataType.StringUTF8:
                        codeStr += ", " + "($8)\"" + param.valueString.Replace("\n", @"\n") + "\"";
                        break;
                    case DataType.LenStringUnicode:
                        codeStr += ", " + "(&u)\"" + param.valueString.Replace("\n", @"\n") + "\"";
                        break;
                    case DataType.LenStringSJIS:
                        codeStr += ", " + "(&j)\"" + param.valueString.Replace("\n", @"\n") + "\"";
                        break;
                    case DataType.LenStringUTF8:
                        codeStr += ", " + "(&8)\"" + param.valueString.Replace("\n", @"\n") + "\"";
                        break;
                    case DataType.Position:
                        codeStr += ", (label)\"" + param.valueString + "\"";
                        break;
                    default:
                        break;
                }
            }
            codeStr += ")";

            return codeStr;
        }
    }
}
