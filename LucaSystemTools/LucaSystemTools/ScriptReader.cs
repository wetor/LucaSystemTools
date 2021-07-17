using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using LucaSystemTools;
using Newtonsoft.Json;
using ProtScript.Entity;

namespace ProtScript
{
    
    public class ScriptReader
    {
        private FileStream fs;
        private BinaryReader br;

        private Dictionary<byte, ScriptOpcode> opcodeDict = new Dictionary<byte, ScriptOpcode>();
        // 当前代码行下标
        private int currentLine = 0;

        private Dictionary<int, int> gotoPosLine = new Dictionary<int, int>();

        // 跳转目标位置
        private HashSet<int> labelPos = new HashSet<int>();


        public ScriptEntity script = new ScriptEntity();

        public ScriptReader(string path, Dictionary<byte, ScriptOpcode> dict, int version)
        {
            fs = new FileStream(path, FileMode.Open);
            br = new BinaryReader(fs);
            opcodeDict = dict;
            script.version = version;
        }
        public void SetOpcodeDict(Dictionary<byte, ScriptOpcode> dict)
        {
            opcodeDict = dict;
        }
        public ScriptOpcode GetOpcodeDict(byte index)
        {
            return opcodeDict[index];
        }
        public void Close()
        {
            br.Close();
            fs.Close();
        }
        public void ReadScript()
        {
            fs.Seek(0, SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                script.lines.Add(ReadCodeLine());
            }
            FixGotoPosition();
            if (Program.debug)
            {
                foreach (var code in script.lines)
                {
                    Console.WriteLine(code.index + " " + code.position);
                    Console.WriteLine(code.ToString());
                }
            }
        }
        public void ReadScript_Clear()
        {
            script.lines.Clear();
        }
        public bool ReadScript_CanStepRead()
        {
            return fs.Position < fs.Length;
        }
        public void ReadScript_Seek(int step, SeekOrigin seek = SeekOrigin.Current)
        {
            fs.Seek(step, seek);
        }
        public CodeLine ReadScript_StepRead(out int position, out int length)
        {
            position = (int)fs.Position;
            CodeLine code = ReadCodeLine();
            length = (int)fs.Position - position;
            return code;
        }
        public CodeLine ReadScript_StepReadByOpcode(ScriptOpcode opcode)
        {
            opcodeDict[opcode.opcode_byte] = opcode;
            CodeLine code = ReadCodeLine();
            return code;
        }
        public void ReadScript_SetCodeLine(CodeLine code)
        {
            script.lines.Add(code);
        }
        public void ReadScript_ReadOver()
        {
            FixGotoPosition();
        }
        private void FixGotoPosition()
        {
            // 遍历行
            int id = 0;
            for (int line = 0; line < script.lines.Count; line++) 
            {
                /*if (script.lines[line].isGoto)
                {
                    int pos = (int)(uint)script.lines[line].GetGoto().value;
                    script.lines[line].SetGotoValue(gotoPosLine[pos]);
                }*/
                if (labelPos.Contains(script.lines[line].position))
                {
                    script.lines[line].SetLabel("label_" + id);
                    id++;
                }
            }
            for (int line = 0; line < script.lines.Count; line++)
            {
                if (script.lines[line].isPosition)
                {
                    for (int index = 0; index < script.lines[line].paramDatas.Count; index++)
                    {
                        if (script.lines[line].paramDatas[index].type == DataType.Position)
                        {
                            int pos = (int)(uint)script.lines[line].paramDatas[index].value;
                            script.lines[line].paramDatas[index].valueOp = script.lines[gotoPosLine[pos]].label;
                        }
                    }
   
                }
            }
        }
        private CodeLine ReadCodeLine()
        {
            CodeLine code = new CodeLine(currentLine, (int)fs.Position);
            if (!gotoPosLine.ContainsKey((int)fs.Position))
            {
                // 位置 下标
                gotoPosLine.Add((int)fs.Position, currentLine);
            }
            
            int codeLength = 0;
            int codeOffset = 0;


            if (script.version == 2)
            {
                // [xx]   [xx]
                // opcode len
                code.opcodeIndex = br.ReadByte();
                codeLength = br.ReadByte() * 2;
                codeOffset += 2;
            }
            else if(script.version == 3)
            {
                // [xx xx] [xx]
                // len     opcode
                codeLength = br.ReadUInt16() - 2;
                code.opcodeIndex = br.ReadByte();
                codeOffset++;
            }
            long savePosition = fs.Position;
            fs.Seek(-codeOffset, SeekOrigin.Current);
            code.bytes = br.ReadBytes(codeLength);
            fs.Seek(savePosition, SeekOrigin.Begin);

            if (!opcodeDict.ContainsKey(code.opcodeIndex))
            {
                throw new Exception("未知的opcode!");
            }
            code.opcode = opcodeDict[code.opcodeIndex].opcode;

            int infoCount = 0;
            CodeInfo info = null;
            if (script.version == 2)
            {
                info = new CodeInfo(0);
                info.count = 1;
                info.data = new UInt16[1];
                info.data[0] = br.ReadUInt16();
                //fs.Seek(-2, SeekOrigin.Current);
                codeOffset += 2;
                code.info = info;
                
            }
            else if (script.version == 3)
            {
                info = new CodeInfo(br.ReadByte());
                codeOffset++;

                infoCount = 0;
                if (info.count >= 1)
                {
                    infoCount++;
                }
                if (info.count >= 2)
                {
                    infoCount++;
                }
                /*if (info.count == 3)
                {
                    Console.WriteLine(code.opcode);
                }*/
                info.data = new UInt16[infoCount];
                for (int i = 0; i < infoCount; i++)
                {
                    info.data[i] = br.ReadUInt16();
                    codeOffset += 2;
                }
                code.info = info;
            }


            // 参数类型列表
            code.paramTypes = opcodeDict[code.opcodeIndex].param.ToArray();

            // LB_EN TASK

            if (code.opcodeIndex == 0x5A && code.opcode == "TASK" &&  codeLength > 12)
            {
                List<ParamType> tmpParamTypes = new List<ParamType>();
                switch (br.ReadUInt16())
                {
                    case 0x4:
                        if (br.ReadUInt16() == 0x6)
                        {
                            tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        }
                        fs.Seek(-2, SeekOrigin.Current);
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false, true));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false, true));
                        break;
                    case 0x17:
                    case 0x1C:
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUTF8, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUTF8, true));
                        tmpParamTypes.Add(new ParamType(DataType.StringUTF8, true));
                        break;
                    case 0x36:
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false));
                        break;
                    case 0x45:
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.UInt16, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false));
                        tmpParamTypes.Add(new ParamType(DataType.StringUnicode, false));
                        break;
                    default:
                        tmpParamTypes.AddRange(code.paramTypes);
                        break;
                }
                fs.Seek(-2, SeekOrigin.Current);
                code.paramTypes = tmpParamTypes.ToArray(); 
                
            }


            // 读取已知参数数据
            code.paramDatas = ReadParamData(code.paramTypes, codeLength, ref codeOffset);
            // 处理未知参数数据
            while (codeOffset + 1 < codeLength)
            {
                byte[] temp = br.ReadBytes(2);
                codeOffset += 2;
                code.paramDatas.Add(new ParamData(DataType.Byte2, temp, ScriptUtil.Byte2Hex(temp, false, true)));
            }
            if (codeOffset < codeLength)
            {
                byte[] temp = br.ReadBytes(1);
                codeOffset++;
                if (script.version == 2 && temp[0] == 0x00)
                {
                    // 最后多出的单字节，若为0x00则舍弃
                }
                else 
                {
                    code.paramDatas.Add(new ParamData(DataType.Byte, temp, ScriptUtil.Byte2Hex(temp, false, true)));
                }
            }
            // 长度非偶数，最后会有补位
            if (codeLength % 2 != 0)
            {
                br.ReadByte();
            }

            // 判断是否含跳转
            foreach (var param in code.paramDatas)
            {
                if(param.type == DataType.Position)
                {
                    code.isPosition = true;
                    labelPos.Add((int)(uint)param.value);
                }
            }


            currentLine++;
            return code;
        }
        public void SaveJson(string path)
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            sw.WriteLine(JsonConvert.SerializeObject(script, Formatting.Indented, jsetting));
            sw.Close();
        }

        private delegate string StringFormater(int codeIndex, int index, CodeLine code);
        /// <summary>
        /// 导出在OPCODE中被"@"标记的数据。可导入，需要结合json或lua
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode">0：review模式，仅文本；1：replace模式，id和文本；2：translate模式，对照</param>
        public void SaveStringLB(string path, int mode = 0)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);

            StringFormater formater0 = delegate (int codeIndex, int index, CodeLine code)
            {
                return code.paramDatas[index].valueString.Replace("\n", @"\n");
            };
            StringFormater formater1 = delegate (int codeIndex, int index, CodeLine code)
            {
                string str = code.paramDatas[index].valueString.Replace("\n", @"\n");
                var checkInt = 0;
                if (code.info.count > 0)
                {
                    checkInt = code.info.data[0];
                }
                string checkStr = checkInt.ToString().PadLeft(5, '0'); // 校验码
               
                return "○" + codeIndex.ToString().PadLeft(5, '0') + "|" + checkStr + "○" + str;
            };
            
            StringFormater formater2 = delegate (int codeIndex, int index, CodeLine code)
            {
                string str = code.paramDatas[index].valueString.Replace("\n", @"\n");
                var checkInt = 0;
                if (code.info.count > 0)
                {
                    checkInt = code.info.data[0];
                }
                string checkStr = checkInt.ToString().PadLeft(5, '0'); // 校验码
                                                                       //"●(\d{5})\|(\d{8})●\s?(.*?)[\r\n|\n]"
                return "●" + codeIndex.ToString().PadLeft(5, '0') + "|" + checkStr + "●" + str;
            };
            //"○(\d{5})\|(\d{5})○([\w\W]*?)○(\d{5})\|(\d{5})○([\w\W]*?)●(\d{5})\|(\d{5})●([\w\W]*?)[\r\n|\n][\r\n|\n]"
            int i = 0;
            foreach (var code in script.lines)
            {
                int tranIndex = -1;
                for (int index = 0; index < code.paramTypes.Length; index++)
                {
                    if (code.paramTypes[index].export)
                    {
                        if (tranIndex < 0) // 以第一个导出字符串为翻译字符串
                        {
                            tranIndex = index;
                        }
                        sw.WriteLine(formater1(i, index, code));
                    }
                }
                if (tranIndex > 0)
                {
                    if(mode == 2)
                    {  sw.WriteLine(formater2(i, tranIndex, code));
                    }
                    sw.WriteLine();
                    i++;
                }
                
            }
            sw.Close();
        }
        /// <summary>
        /// 导出在OPCODE中被"@"标记的数据。可导入，需要结合json或lua
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode">0：review模式，仅文本；1：replace模式，id和文本；2：translate模式，对照</param>
        public void SaveString(string path, int mode = 0)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);

            StringFormater formater;
            if (mode == 1)
            {
                formater = delegate (int codeIndex, int index, CodeLine code)
                {
                    string str = code.paramDatas[index].valueString.Replace("\n", @"\n");
                    string indexStr = code.info.data[0].ToString().PadLeft(8, '0');
                    //"●(\d{5})\|(\d{8})●\s?(.*?)[\r\n|\n]"
                    return "●" + codeIndex.ToString().PadLeft(5, '0') + "|" + indexStr + "● " + str;
                };
            }
            else if (mode == 2)
            {
                formater = delegate (int codeIndex, int index, CodeLine code)
                {
                    string str = code.paramDatas[index].valueString.Replace("\n", @"\n");
                    string indexStr = code.info.data[0].ToString().PadLeft(8, '0');
                    string codeIndexStr = codeIndex.ToString().PadLeft(5, '0');
                    //"○(\d{5})\|(\d{8})○\s?(.*?)[\r\n|\n]●(\d{5})\|(\d{8})●\s?(.*?)[\r\n|\n]"
                    return "○" + codeIndexStr + "|" + indexStr + "○ " + str + "\n" +
                    "●" + codeIndexStr + "|" + indexStr + "● " + str;
                };
            }
            else
            {
                formater = delegate (int codeIndex, int index, CodeLine code)
                {
                    return code.paramDatas[index].valueString.Replace("\n", @"\n");
                };
            }
            int i = 0;
            foreach (var code in script.lines)
            {
                for (int index = 0; index < code.paramTypes.Length; index++)
                {
                    if (code.paramTypes[index].export)
                    {
                        sw.WriteLine(formater(++i, index, code));
                        sw.WriteLine();
                    }
                }
            }
            sw.Close();
        }
        public void SaveLua(string path, bool canLoad = true)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            if (canLoad)
            {
                sw.WriteLine("-- ToolVersion:{0}", Program.toolVersion);
                sw.WriteLine("-- GameVersion:{0}", script.version);
                foreach (var code in script.lines)
                {

                    sw.WriteLine(code.ToStringAll());
                }
            }
            else
            {
                foreach (var code in script.lines)
                {
                    sw.WriteLine(code.ToString());
                }
            }
            
            sw.Close();
        }
        /// <summary>
        /// 读取参数的实际值
        /// </summary>
        /// <param name="param">类型列表</param>
        /// <param name="codeLength">此指令长度</param>
        /// <param name="codeOffset">当前位置引用</param>
        /// <returns></returns>
        private List<ParamData> ReadParamData(ParamType[] param, int codeLength, ref int codeOffset)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> retn = new List<string>();
            List<ParamData> datas = new List<ParamData>();
            int count = 0;
            bool nullableSkip = false;
            string dataStr = "";
            foreach (var value in param)
            {
                DataType type = value.type;
                count++;
                var tempPos = fs.Position;
                switch (type)
                {
                    case DataType.Byte:
                    case DataType.Byte2:
                    case DataType.Byte3:
                    case DataType.Byte4:
                        if (value.nullable && codeOffset + (int)type > codeLength)
                        {
                            nullableSkip = true;
                            break;
                        }
                        var dataBytes = br.ReadBytes((int)type);
                        dataStr = ScriptUtil.Byte2Hex(dataBytes, false, true);
                        datas.Add(new ParamData(type, dataBytes, dataStr));
                        break;
                    case DataType.UInt16:
                        if (value.nullable && codeOffset + 2 > codeLength)
                        {
                            nullableSkip = true;
                            break;
                        }
                        var dataUint16 = br.ReadUInt16();
                        dataStr = dataUint16.ToString();
                        datas.Add(new ParamData(type, dataUint16, dataStr));
                        break;
                    case DataType.Int16:
                        if (value.nullable && codeOffset + 2 > codeLength)
                        {
                            nullableSkip = true;
                            break;
                        }
                        var dataInt16 = br.ReadInt16();
                        dataStr = dataInt16.ToString();
                        datas.Add(new ParamData(type, dataInt16, dataStr));
                        break;
                    case DataType.UInt32:
                    case DataType.Position:
                        if (value.nullable && codeOffset + 4 > codeLength)
                        {
                            nullableSkip = true;
                            break;
                        }
                        var dataUint32 = br.ReadUInt32();
                        dataStr = dataUint32.ToString();
                        datas.Add(new ParamData(type, dataUint32, dataStr));
                        break;
                    case DataType.Int32:
                        if (value.nullable && codeOffset + 4 > codeLength)
                        {
                            nullableSkip = true;
                            break;
                        }
                        var dataInt32 = br.ReadInt32();
                        dataStr = dataInt32.ToString();
                        datas.Add(new ParamData(type, dataInt32, dataStr));
                        break;
                    case DataType.StringUnicode:
                    case DataType.StringSJIS:
                    case DataType.StringUTF8:
                        if (value.nullable && codeOffset + 2 > codeLength)
                        {
                            nullableSkip = true;
                            //Console.WriteLine("{0} {1} {2}", codeOffset, codeLength, fs.Position);
                            break;
                        }
                        if (type == DataType.StringUnicode)
                        {
                            dataStr = Encoding.Unicode.GetString(ReadStringDoubleEnd());
                        }
                        else if (type == DataType.StringSJIS)
                        {
                            dataStr = Encoding.GetEncoding("Shift-Jis").GetString(ReadStringSingleEnd());
                        }
                        else if (type == DataType.StringUTF8)
                        {
                            dataStr = Encoding.UTF8.GetString(ReadStringSingleEnd());
                        }
                        CheckString(ref dataStr);
                        datas.Add(new ParamData(type, dataStr, dataStr));
                        break;
                    case DataType.LenStringUnicode:
                    case DataType.LenStringSJIS:
                        if (value.nullable && codeOffset + 2 > codeLength)
                        {
                            nullableSkip = true;
                            break;
                        }
                        int len = br.ReadUInt16();
                        if (type == DataType.LenStringUnicode)
                        {
                            dataStr = Encoding.Unicode.GetString(br.ReadBytes(len * 2));
                        }
                        else if (type == DataType.LenStringSJIS)
                        {
                            dataStr = Encoding.GetEncoding("Shift-Jis").GetString(br.ReadBytes(len * 2));
                        }
                        CheckString(ref dataStr);
                        datas.Add(new ParamData(type, dataStr, dataStr));
                        break;
                    default:
                        break;
                }
                codeOffset += (int)(fs.Position - tempPos);
                if(codeOffset > codeLength)
                {
                    throw new Exception("opcode参数数量错误！ ");
                }

            }
            return datas;
        }
        private void CheckString(ref string str)
        {
            if (str.LastIndexOf('\"') > 0)
            {
                StringBuilder sb = new StringBuilder(str);
                str = sb.Replace('\"', '“', 0, str.IndexOf('\"') + 1).Replace('\"', '”', 0, str.LastIndexOf('\"') + 1).ToString();
            }
            else if (str.LastIndexOf('❞') > 0)
            {
                str = str.Replace("❝", "“").Replace("❞", "”");
            }
        }
        private byte[] ReadStringDoubleEnd()
        {
            List<byte> buff = new List<byte>();
            byte[] btmp = br.ReadBytes(2);
            while (!(btmp[0] == 0x00 && btmp[1] == 0x00))
            {
                buff.AddRange(btmp);
                btmp = br.ReadBytes(2);
            }
            return buff.ToArray();
        }
        private byte[] ReadStringSingleEnd()
        {
            List<byte> buff = new List<byte>();
            byte btmp = br.ReadByte();
            while (btmp != 0x00)
            {
                buff.Add(btmp);
                btmp = br.ReadByte();
            }
            return buff.ToArray();
        }

    }
}
