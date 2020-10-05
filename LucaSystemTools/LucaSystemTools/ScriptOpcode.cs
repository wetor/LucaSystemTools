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

namespace ProtScript
{
    public enum Type
    {
        Byte,
        Byte2,
        Byte3,
        Byte4,
        UInt16,
        Int16,
        UInt32,
        Int32,
        StringUnicode,
        StringSJIS,
        StringUTF8,
        LenStringUnicode,
        LenStringSJIS,
        Position
    }
    public struct ParamType
    {
        public Type type;
        public bool nullable;
        public ParamType(Type _type, bool _nullable)
        {
            type = _type;
            nullable = _nullable;
        }

    }
    

    public class ScriptOpcode
    {
        public static Dictionary<uint, int> code_position = new Dictionary<uint, int>();

        public string opcode = "UNKNOW";
        public string comment = "";//注释说明
        public byte opcode_byte = 127;
        public List<ParamType> param = new List<ParamType>();

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
            param.AddRange(values);
        }
        public ParamData[] ReadFunc(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader mbr = new BinaryReader(ms);
            ParamData[] retn = ReadFunc(ref mbr, opcode, opcode_byte, param);
            mbr.Close();
            ms.Close();
            return retn;
        }
        public ParamData[] ReadFunc(ref BinaryReader tbr)
        {
            return ReadFunc(ref tbr, opcode, opcode_byte, param);
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
                            param.Add(new ParamType((Type)Enum.Parse(typeof(Type), tmp, true), flag_nullable));
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
                                param.Add(new ParamType((Type)Enum.Parse(typeof(Type), tmp, true), flag_nullable));
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
        public static ParamData[] ReadFunc(ref BinaryReader tbr, string opcode, byte opcode_byte, List<ParamType> param)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> retn = new List<string>();
            List<ParamData> datas = new List<ParamData>();
            int count = 0;
            int curr_pos = (int)tbr.BaseStream.Position;
            int tbr_len = (int)tbr.BaseStream.Length;
            bool nullable_skip = false;
            string data_str;
            foreach (var value in param)
            {
                Type type = value.type;
                count++;
                curr_pos = (int)tbr.BaseStream.Position;
                switch (type)
                {
                    case Type.Byte:
                    case Type.Byte2:
                    case Type.Byte3:
                    case Type.Byte4:
                        {
                            var data_bytes = tbr.ReadBytes((int)type + 1);
                            data_str = ScriptUtil.Byte2Hex(data_bytes);
                            datas.Add(new ParamData(type, data_bytes, data_str));
                            break;
                        }
                    case Type.UInt16:
                        if (value.nullable && curr_pos + 2 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_uint16 = tbr.ReadUInt16();
                        data_str = data_uint16.ToString();
                        datas.Add(new ParamData(Type.UInt16, data_uint16, data_str));
                        break;
                    case Type.Int16:
                        if (value.nullable && curr_pos + 2 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_int16 = tbr.ReadInt16();
                        data_str = data_int16.ToString();
                        datas.Add(new ParamData(Type.Int16, data_int16, data_str));
                        break;
                    case Type.UInt32:
                        if (value.nullable && curr_pos + 4 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_uint32 = tbr.ReadUInt32();
                        data_str = data_uint32.ToString();
                        datas.Add(new ParamData(Type.UInt32, data_uint32, data_str));
                        break;
                    case Type.Int32:
                        if (value.nullable && curr_pos + 4 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_int32 = tbr.ReadInt32();
                        data_str = data_int32.ToString();
                        datas.Add(new ParamData(Type.Int32, data_int32, data_str));
                        break;
                    case Type.StringUnicode:
                    case Type.StringSJIS:
                    case Type.StringUTF8:
                        {
                            if (type == Type.StringUnicode)
                            {
                                data_str = Encoding.Unicode.GetString(ReadStringDoubleEnd(ref tbr));
                                datas.Add(new ParamData(Type.StringUnicode, data_str, data_str));
                            }
                            else if (type == Type.StringSJIS)
                            {
                                data_str = Encoding.GetEncoding("Shift-Jis").GetString(ReadStringSingleEnd(ref tbr));
                                datas.Add(new ParamData(Type.StringSJIS, data_str, data_str));
                            }
                            else if (type == Type.StringUTF8)
                            {
                                data_str = Encoding.UTF8.GetString(ReadStringSingleEnd(ref tbr));
                                datas.Add(new ParamData(Type.StringUTF8, data_str, data_str));
                            }

                            break;
                        }
                    case Type.LenStringUnicode:
                    case Type.LenStringSJIS:
                        {
                            if (value.nullable && curr_pos + 2 > tbr_len)
                            {
                                nullable_skip = true;
                                break;
                            }
                            int len = tbr.ReadUInt16();
                            if (type == Type.LenStringUnicode)
                            {
                                data_str = Encoding.Unicode.GetString(tbr.ReadBytes(len * 2));
                                datas.Add(new ParamData(Type.LenStringUnicode, data_str, data_str));
                            }
                            else if (type == Type.LenStringSJIS)
                            {
                                data_str = Encoding.GetEncoding("Shift-Jis").GetString(tbr.ReadBytes(len * 2));
                                datas.Add(new ParamData(Type.LenStringSJIS, data_str, data_str));
                            }
                            break;
                        }
                    case Type.Position:
                        if (value.nullable && curr_pos + 4 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        uint pos = tbr.ReadUInt32();
                        if (code_position.ContainsKey(pos))
                        {
                            datas.Add(new ParamData(Type.Position, pos, code_position[pos].ToString()));
                        }
                        else
                        {
                            throw new Exception("错误的跳转位置: " + pos + "！");
                        }
                        break;
                    default:
                        break;
                }
 
            }
            return datas.ToArray();
        }

        public static string[] ParamDataToArray(ParamData[] parameters)
        {
            List<string> retn = new List<string>();
            string tmp = "";
            foreach (var param in parameters)
            {
                Type type = param.type;
                switch (type)
                {
                    case Type.Byte:
                    case Type.Byte2:
                    case Type.Byte3:
                    case Type.Byte4:
                        tmp = "[" + param.value_string + "]";
                        break;
                    case Type.UInt16:
                        tmp = "(" + param.value_string + ")";
                        break;
                    case Type.Int16:
                        tmp = "(" + param.value_string + ")";
                        break;
                    case Type.UInt32:
                        tmp = "{" + param.value_string + "}";
                        break;
                    case Type.Int32:
                        tmp = "{" + param.value_string + "}";
                        break;
                    case Type.StringUnicode:
                        tmp = "$u\"" + param.value_string + "\"";
                        break;
                    case Type.StringSJIS:
                        tmp = "$j\"" + param.value_string + "\"";
                        break;
                    case Type.StringUTF8:
                        tmp = "$8\"" + param.value_string + "\"";
                        break;
                    case Type.LenStringUnicode:
                        tmp = "&u\"" + param.value_string + "\"";
                        break;
                    case Type.LenStringSJIS:
                        tmp = "&j\"" + param.value_string + "\"";
                        break;
                    case Type.Position:
                        tmp = "<" + param.value_string + ">";
                        break;
                    default:
                        break;
                }
                retn.Add(tmp.Replace("\n", @"{\n}"));
            }

            return retn.ToArray();
        }

        public static string ParamDataToString(ParamData[] parameters)
        {
            string retn = "";
            foreach(var str in ParamDataToArray(parameters))
            {
                if (str != "")
                {
                    retn += str + " ";
                }
                else
                {
                    retn += "null" + " ";
                }
            }
            if (retn.Length > 0)
            {
                retn = retn.Remove(retn.Length - 1);
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
