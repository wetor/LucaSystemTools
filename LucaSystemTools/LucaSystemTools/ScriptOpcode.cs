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
        StringUnicode,
        StringSJIS,
        StringUTF8,
        StringCustom,
        LenStringUnicode,
        LenStringSJIS,
        LenStringUTF8,
        LenStringCustom,
        Position
    }
    public struct ParamType
    {
        public DataType type;
        public bool nullable;
        public ParamType(DataType _type, bool _nullable)
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
                            param.Add(new ParamType((DataType)Enum.Parse(typeof(DataType), tmp, true), flag_nullable));
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
                                param.Add(new ParamType((DataType)Enum.Parse(typeof(DataType), tmp, true), flag_nullable));
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
                DataType type = value.type;
                count++;
                curr_pos = (int)tbr.BaseStream.Position;
                switch (type)
                {
                    case DataType.Byte:
                    case DataType.Byte2:
                    case DataType.Byte3:
                    case DataType.Byte4:
                        {
                            var data_bytes = tbr.ReadBytes((int)type);
                            data_str = ScriptUtil.Byte2Hex(data_bytes);
                            datas.Add(new ParamData(type, data_bytes, data_str));
                            break;
                        }
                    case DataType.UInt16:
                        if (value.nullable && curr_pos + 2 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_uint16 = tbr.ReadUInt16();
                        data_str = data_uint16.ToString();
                        datas.Add(new ParamData(DataType.UInt16, data_uint16, data_str));
                        break;
                    case DataType.Int16:
                        if (value.nullable && curr_pos + 2 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_int16 = tbr.ReadInt16();
                        data_str = data_int16.ToString();
                        datas.Add(new ParamData(DataType.Int16, data_int16, data_str));
                        break;
                    case DataType.UInt32:
                        if (value.nullable && curr_pos + 4 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_uint32 = tbr.ReadUInt32();
                        data_str = data_uint32.ToString();
                        datas.Add(new ParamData(DataType.UInt32, data_uint32, data_str));
                        break;
                    case DataType.Int32:
                        if (value.nullable && curr_pos + 4 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_int32 = tbr.ReadInt32();
                        data_str = data_int32.ToString();
                        datas.Add(new ParamData(DataType.Int32, data_int32, data_str));
                        break;
                    case DataType.StringUnicode:
                    case DataType.StringSJIS:
                    case DataType.StringUTF8:
                        {
                            if (type == DataType.StringUnicode)
                            {
                                data_str = Encoding.Unicode.GetString(ReadStringDoubleEnd(ref tbr));
                                datas.Add(new ParamData(DataType.StringUnicode, data_str, data_str));
                            }
                            else if (type == DataType.StringSJIS)
                            {
                                data_str = Encoding.GetEncoding("Shift-Jis").GetString(ReadStringSingleEnd(ref tbr));
                                datas.Add(new ParamData(DataType.StringSJIS, data_str, data_str));
                            }
                            else if (type == DataType.StringUTF8)
                            {
                                data_str = Encoding.UTF8.GetString(ReadStringSingleEnd(ref tbr));
                                datas.Add(new ParamData(DataType.StringUTF8, data_str, data_str));
                            }

                            break;
                        }
                    case DataType.LenStringUnicode:
                    case DataType.LenStringSJIS:
                        {
                            if (value.nullable && curr_pos + 2 > tbr_len)
                            {
                                nullable_skip = true;
                                break;
                            }
                            int len = tbr.ReadUInt16();
                            if (type == DataType.LenStringUnicode)
                            {
                                data_str = Encoding.Unicode.GetString(tbr.ReadBytes(len * 2));
                                datas.Add(new ParamData(DataType.LenStringUnicode, data_str, data_str));
                            }
                            else if (type == DataType.LenStringSJIS)
                            {
                                data_str = Encoding.GetEncoding("Shift-Jis").GetString(tbr.ReadBytes(len * 2));
                                datas.Add(new ParamData(DataType.LenStringSJIS, data_str, data_str));
                            }
                            break;
                        }
                    case DataType.Position:
                        if (value.nullable && curr_pos + 4 > tbr_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        uint pos = tbr.ReadUInt32();
                        if (code_position.ContainsKey(pos))
                        {
                            datas.Add(new ParamData(DataType.Position, pos, code_position[pos].ToString()));
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
            Console.WriteLine(parameters.Length);
            List<string> retn = new List<string>();
            string tmp = "";
            foreach (var param in parameters)
            {
                DataType type = param.type;
                switch (type)
                {
                    case DataType.Byte:
                    case DataType.Byte2:
                    case DataType.Byte3:
                    case DataType.Byte4:
                        tmp = "[" + param.valueString + "]";
                        break;
                    case DataType.UInt16:
                        tmp = "(" + param.valueString + ")";
                        break;
                    case DataType.Int16:
                        tmp = "(" + param.valueString + ")";
                        break;
                    case DataType.UInt32:
                        tmp = "{" + param.valueString + "}";
                        break;
                    case DataType.Int32:
                        tmp = "{" + param.valueString + "}";
                        break;
                    case DataType.StringUnicode:
                        tmp = "$u\"" + param.valueString + "\"";
                        break;
                    case DataType.StringSJIS:
                        tmp = "$j\"" + param.valueString + "\"";
                        break;
                    case DataType.StringUTF8:
                        tmp = "$8\"" + param.valueString + "\"";
                        break;
                    case DataType.LenStringUnicode:
                        tmp = "&u\"" + param.valueString + "\"";
                        break;
                    case DataType.LenStringSJIS:
                        tmp = "&j\"" + param.valueString + "\"";
                        break;
                    case DataType.Position:
                        tmp = "<" + param.valueString + ">";
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
                retn += Enum.GetName(typeof(DataType), value.type) + ", ";
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
