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
        String,
        StringSJIS,
        PString,
        PStringSJIS,
        ENString,
        Opcode,
        Skip
    }
    class ScriptOpcode
    {
        public string opcode = "UNKNOW";
        public string comment = "";//注释说明
        public byte opcode_byte = 127;
        private List<Type> param = new List<Type>();

        public ScriptOpcode(byte opcode_byte, string text)
        {
            if (text == "")
                return;
            this.opcode_byte = opcode_byte;
            string tmp = "";
            bool is_param = false;
            bool is_comment = false;
            for(int i = 0; i < text.Length;)
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
                    case ',':
                        if (is_param && tmp!="")
                        {
                            param.Add((Type)Enum.Parse(typeof(Type), tmp, true));
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
                                param.Add((Type)Enum.Parse(typeof(Type), tmp, true));
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
                if(i == text.Length - 1)
                {
                    if (is_comment)
                    {
                        comment = tmp;
                        tmp = "";
                    }
                    else if(opcode == "" || opcode == "UNKNOW")
                    {
                        opcode = tmp;
                        tmp = "";
                    }
                }
                i++;
            }
            //Console.WriteLine(ToString());

/*

            string[] split = dic_text.Split(" ");
            opcode = split[0];
            if (split.Length > 1)
                for (int i = 1; i < split.Length; i++)
                    param.Add((Type)Enum.Parse(typeof(Type), split[i], true));*/
        }
        public ScriptOpcode(byte opcode_byte, string opcode, params Type[] values)
        {
            this.opcode_byte = opcode_byte;
            this.opcode = opcode;
            param.AddRange(values);
        }
        public string Load(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader mbr = new BinaryReader(ms);
            string retn = Load(ref mbr);
            mbr.Close();
            ms.Close();
            return retn;
        }
        public string Load(ref BinaryReader tbr)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            bool skip = false;
            string retn = "";
            string tmp = "";
            int count = 0;
            foreach (Type value in param)
            {
                count++;
                skip = false;
                tmp = "";
                switch (value)
                {
                    case Type.Byte:
                    case Type.Byte2:
                    case Type.Byte3:
                    case Type.Byte4:
                        tmp = "[" + ScriptParser.Byte2Hex(tbr.ReadBytes((int)value + 1)) + "]";
                        break;
                    case Type.UInt16:
                        tmp = "(" + tbr.ReadUInt16().ToString() + ")";
                        break;
                    case Type.UInt32:
                        tmp = "{" + tbr.ReadUInt32().ToString() + "}";
                        break;
                    case Type.String:
                    case Type.StringSJIS:
                    case Type.ENString:
                        {
                            List<byte> buff = new List<byte>();
                            if (value == Type.String)
                            {
                                byte[] btmp = tbr.ReadBytes(2);
                                while (!(btmp[0] == 0x00 && btmp[1] == 0x00))
                                {
                                    buff.AddRange(btmp);
                                    btmp = tbr.ReadBytes(2);
                                }
                                tmp = "u\"" + Encoding.Unicode.GetString(buff.ToArray()) + "\"";
                            }
                            else if (value == Type.StringSJIS)
                            {
                                byte btmp = tbr.ReadByte();
                                while (btmp != 0x00)
                                {
                                    buff.Add(btmp);
                                    btmp = tbr.ReadByte();
                                }
                                tmp = "j\"" + Encoding.GetEncoding("Shift-Jis").GetString(buff.ToArray()) + "\"";
                            }
                            else if (value == Type.ENString)
                            {
                                byte btmp = tbr.ReadByte();
                                while (btmp != 0x00)
                                {
                                    buff.Add(btmp);
                                    btmp = tbr.ReadByte();
                                }
                                byte[] begin = new byte[] { 0xE2, 0x9D, 0x9D };
                                byte[] end = new byte[] { 0xE2, 0x9D, 0x9E };

                                int idx_begin = -1;
                                int idx_end = -1;
                                for (int i = 0; i < buff.Count - 2; i++)
                                {
                                    if (buff.GetRange(i, 3).SequenceEqual(begin))
                                    {
                                        idx_begin = i;
                                    }
                                    else if (buff.GetRange(i, 3).SequenceEqual(end))
                                    {
                                        idx_end = i;
                                    }
                                }
                                //Console.WriteLine("debug:{0} {1}  len:{2}", idx_begin, idx_end, buff.Count);
                                if (idx_begin !=-1 && idx_end != -1)
                                {
                                    //Console.WriteLine("debug:{0} {1}",idx_begin,idx_end);
                                    buff.RemoveRange(idx_begin, 3);
                                    buff.RemoveRange(idx_end - 3, 3);
                                    buff.InsertRange(idx_begin, Encoding.ASCII.GetBytes(@"{\b}"));
                                    idx_end += 1;
                                    buff.InsertRange(idx_end, Encoding.ASCII.GetBytes(@"{\e}"));
                                }
                                tmp = "e\"" + Encoding.ASCII.GetString(buff.ToArray()) + "\"";
                            }
                            break;
                        }
                    case Type.PString:
                    case Type.PStringSJIS:
                        {
                            int len = tbr.ReadUInt16();
                            if (value == Type.PString)
                                tmp = "p\"" + Encoding.Unicode.GetString(tbr.ReadBytes(len * 2)) + "\"";
                            else if (value == Type.PStringSJIS)
                                tmp = "s\"" + Encoding.GetEncoding("Shift-Jis").GetString(tbr.ReadBytes(len * 2)) + "\"";
                            break;
                        }
                    case Type.Opcode:
                        byte scr_index = tbr.ReadByte();
                        if(scr_index == opcode_byte)
                            tmp = opcode;
                        else
                            tmp = "[" + ScriptParser.Byte2Hex(scr_index) + "]";
                        break;
                    case Type.Skip:
                        skip = true;
                        tbr.ReadByte();
                        break;
                    default:
                        break;
                }

                retn += tmp + ((count != param.Count && !skip) ? " " : "");
                retn = retn.Replace("\n", @"{\n}");

            }
            return retn;
        }

        public override string ToString()
        {
            string retn = opcode + " (";

            foreach (Type type in param)
            {
                retn += Enum.GetName(typeof(Type), type) + ", ";
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
