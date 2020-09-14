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
using System.Text;

namespace ProtScript
{
    public enum Type
    {
        Byte,
        Byte2,
        Byte3,
        Byte4,
        UInt16,
        UInt32,
        StringUnicode,
        StringSJIS,
        StringUTF8,
        LenStringUnicode,
        LenStringSJIS,
        Opcode,
        Skip
    }
    public struct Param
    {
        public Type type;
        public bool nullable;
        public Param(Type _type, bool _nullable)
        {
            type = _type;
            nullable = _nullable;
        }

    }
    public class ScriptOpcode
    {
        public string opcode = "UNKNOW";
        public string comment = "";//注释说明
        public byte opcode_byte = 127;
        private List<Param> param = new List<Param>();

        public ScriptOpcode(byte opcode_byte, string text)
        {
            this.opcode_byte = opcode_byte;
            param = InitParams(text, ref opcode, ref comment);
            //Console.WriteLine(ToString());
        }
        public ScriptOpcode(byte opcode_byte, string opcode, params Param[] values)
        {
            this.opcode_byte = opcode_byte;
            this.opcode = opcode;
            param.AddRange(values);
        }
        public string ReadFunc(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader mbr = new BinaryReader(ms);
            string retn = ReadFunc(ref mbr, opcode, opcode_byte, param);
            mbr.Close();
            ms.Close();
            return retn;
        }
        public string ReadFunc(ref BinaryReader tbr)
        {
            return ReadFunc(ref tbr, opcode, opcode_byte, param);
        }
        public static List<Param> InitParams(string text, ref string opcode, ref string comment)
        {
            List<Param> param = new List<Param>();
            if (text == "")
                return param;
            string tmp = "";
            bool is_param = false;
            bool is_comment = false;
            bool flag_nullable = false;
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
                    case ',':
                        if (is_param && tmp != "")
                        {
                            param.Add(new Param((Type)Enum.Parse(typeof(Type), tmp, true), flag_nullable));
                            flag_nullable = false;
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
                                param.Add(new Param((Type)Enum.Parse(typeof(Type), tmp, true), flag_nullable));
                            flag_nullable = false;
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
        public static string ReadFunc(ref BinaryReader tbr, string opcode, byte opcode_byte, List<Param> param)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            bool skip = false;
            string retn = "";
            string tmp = "";
            int count = 0;
            int curr_pos = (int)tbr.BaseStream.Position;
            int tbr_len = (int)tbr.BaseStream.Length;
            bool nullable_skip = false;
            foreach (var value in param)
            {
                Type type = value.type;
                count++;
                skip = false;
                tmp = "";
                curr_pos = (int)tbr.BaseStream.Position;
                switch (type)
                {
                    case Type.Byte:
                    case Type.Byte2:
                    case Type.Byte3:
                    case Type.Byte4:
                        {
                            tmp = "[" + ScriptUtil.Byte2Hex(tbr.ReadBytes((int)type + 1)) + "]";
                            break;
                        }
                    case Type.UInt16:
                        if (value.nullable && curr_pos + 2 >= tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        tmp = "(" + tbr.ReadUInt16().ToString() + ")";
                        break;
                    case Type.UInt32:
                        if (value.nullable && curr_pos + 4 >= tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        tmp = "{" + tbr.ReadUInt32().ToString() + "}";
                        break;
                    case Type.StringUnicode:
                    case Type.StringSJIS:
                    case Type.StringUTF8:
                        {
                            if (type == Type.StringUnicode)
                            {
                                tmp = "$u\"" + Encoding.Unicode.GetString(ReadStringDoubleEnd(ref tbr)) + "\"";
                            }
                            else if (type == Type.StringSJIS)
                            {
                                tmp = "$j\"" + Encoding.GetEncoding("Shift-Jis").GetString(ReadStringSingleEnd(ref tbr)) + "\"";
                            }
                            else if (type == Type.StringUTF8)
                            {
                                tmp = "$8\"" + Encoding.UTF8.GetString(ReadStringSingleEnd(ref tbr)) + "\"";
                            }
                            break;
                        }
                    case Type.LenStringUnicode:
                    case Type.LenStringSJIS:
                        {
                            if (value.nullable && curr_pos + 2 >= tbr_len)
                            {
                                nullable_skip = true;
                                break;
                            }
                            int len = tbr.ReadUInt16();
                            if (type == Type.LenStringUnicode)
                            {
                                tmp = "&u\"" + Encoding.Unicode.GetString(tbr.ReadBytes(len * 2)) + "\"";
                            }
                            else if (type == Type.LenStringSJIS)
                            {
                                tmp = "&j\"" + Encoding.GetEncoding("Shift-Jis").GetString(tbr.ReadBytes(len * 2)) + "\"";
                            }
                            break;
                        }
                    case Type.Opcode:
                        byte scr_index = tbr.ReadByte();
                        if(scr_index == opcode_byte)
                            tmp = opcode;
                        else
                            tmp = "[" + ScriptUtil.Byte2Hex(scr_index) + "]";
                        break;
                    case Type.Skip:
                        skip = true;
                        tbr.ReadByte();
                        break;
                    default:
                        break;
                }
                if (nullable_skip)
                {
                    if (retn.Length >= 1 && retn[retn.Length - 1] == ' ') 
                    {
                        retn = retn.Remove(retn.Length - 1);
                    }
                    break;
                }
                retn += tmp + ((count != param.Count && !skip) ? " " : "");
                retn = retn.Replace("\n", @"{\n}");

            }
            return retn;
        }
        private static byte[] ReadStringDoubleEnd(ref BinaryReader tbr)
        {
            List<byte> buff = new List<byte>();
            byte[] btmp = tbr.ReadBytes(2);
            while (!(btmp[0] == 0x00 && btmp[1] == 0x00))
            {
                buff.AddRange(btmp);
                btmp = tbr.ReadBytes(2);
            }
            return buff.ToArray();
        }
        private static byte[] ReadStringSingleEnd(ref BinaryReader tbr)
        {
            List<byte> buff = new List<byte>();
            byte btmp = tbr.ReadByte();
            while (btmp != 0x00)
            {
                buff.Add(btmp);
                btmp = tbr.ReadByte();
            }
            return buff.ToArray();
        }
        public override string ToString()
        {
            string retn = opcode + " (";

            foreach (var value in param)
            {
                retn += Enum.GetName(typeof(Type), value.type) + ", ";
            }
            retn = retn.Remove(retn.Length - 2);
            if(param.Count>0)
                retn += ")";
            if (comment != "")
                retn += " ;" + comment;
            return retn;
  
            
        }
    }
}
