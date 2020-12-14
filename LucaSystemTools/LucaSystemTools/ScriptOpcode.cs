/*
 LucaSystem引擎.脚本部分
 脚本文件解析
 Wetor
 2020.9.14
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ProtScript.Entity;

namespace ProtScript
{
    public enum DataType
    {
        Null,
        Byte,
        Byte2,
        Byte3,
        Byte4,
        UInt16,
        Int16,
        UInt32,
        Int32,
        Position,
        StringUnicode,
        StringSJIS,
        StringUTF8,
        StringCustom,
        LenStringUnicode,
        LenStringSJIS,
        LenStringUTF8,
        LenStringCustom
    }
    [Serializable]
    public struct ParamType
    {
        public DataType type;
        public bool nullable;
        public bool export;
        public ParamType(DataType _type, bool _nullable, bool _export = false)
        {
            type = _type;
            nullable = _nullable;
            export = _export;
        }

    }
    [Serializable]
    public class ScriptOpcode
    {
        public string opcode = "UNKNOW";
        public string comment = "";//注释说明
        public byte opcode_byte = 127;
        public List<ParamType> param = new List<ParamType>();

        public ScriptOpcode(byte opcode_byte)
        {
            this.opcode_byte = opcode_byte;
        }
        public ScriptOpcode(byte opcode_byte, string text)
        {
            this.opcode_byte = opcode_byte;
            param = InitParams(text, ref opcode, ref comment);
            //Console.WriteLine(ToString());
        }
        public ScriptOpcode(byte opcode_byte, string opcode, params ParamType[] values)
        {
            this.opcode_byte = opcode_byte;
            this.opcode = opcode;
            param.Clear();
            param.AddRange(values);
        }

        public static List<ParamType> InitParams(string text, ref string opcode, ref string comment)
        {
            List<ParamType> param = new List<ParamType>();
            if (text == "")
                return param;
            string tmp = "";
            bool is_param = false;
            bool is_comment = false;
            bool flag_nullable = false;
            bool flag_export = false;
            for (int i = 0; i < text.Length;)
            {
                char ch = text[i];
                switch (ch)
                {
                    case ' ':
                        break;
                    case '(':
                        opcode = tmp;
                        tmp = "";
                        is_param = true;
                        break;
                    case '!':
                        flag_nullable = true;
                        break;
                    case '@':
                        flag_export = true;
                        break;
                    case ',':
                        if (is_param && tmp != "")
                        {
                            param.Add(new ParamType((DataType)Enum.Parse(typeof(DataType), tmp, true), flag_nullable, flag_export));
                            flag_nullable = false;
                            flag_export = false;
                        }
                        else
                        {
                            Console.WriteLine("Opcode格式错误：{0}", text);
                            break;
                        }
                        tmp = "";
                        break;
                    case ')':
                        if (is_param)
                        {
                            if (tmp != "")
                                param.Add(new ParamType((DataType)Enum.Parse(typeof(DataType), tmp, true), flag_nullable, flag_export));
                            flag_nullable = false;
                            flag_export = false;
                            is_param = false;
                        }
                        else
                        {
                            Console.WriteLine("Opcode格式错误：{0}", i + 1, text);
                            break;
                        }
                        tmp = "";
                        break;
                    case ';':
                        tmp = "";
                        is_comment = true;
                        break;
                    default:
                        tmp += text[i];
                        break;
                }
                if (i == text.Length - 1)
                {
                    if (is_comment)
                    {
                        comment = tmp;
                        tmp = "";
                    }
                    else if (opcode == "" || opcode == "UNKNOW")
                    {
                        opcode = tmp;
                        tmp = "";
                    }
                }
                i++;
            }
            return param;
        }


        public override string ToString()
        {
            string retn = "";
            if (param.Count > 0)
            {
                retn = opcode + " (" + string.Join(", ", param.Select(x => x.type).ToArray()) + ")";
            }
            else
            {
                retn = opcode;
            }
            if (comment != "")
                retn += " ;" + comment;
            return retn;
  
            
        }
        public string ToStringParam()
        {
            return string.Join(", ", param.Select(
                x => (x.export ? "@" : "") + (x.nullable ? "!" : "") + x.type).ToArray());
        }
    }
}
