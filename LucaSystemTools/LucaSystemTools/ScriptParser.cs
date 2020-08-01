/*
 LucaSystem引擎.脚本部分
 目前完美支持Nintendo Switch版本的《Summer Pocket》脚本反汇编以及汇编（但并不完善）
 Nintendo Switch版本的《Clannad》有待完善
 脚本文件解析
 作者：Wetor
 时间：2019.7.25
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AdvancedBinary;
using LucaSystemTools;
using LucaSystem;

namespace ProtScript
{
    public enum GameScript
    {
        ISLAND,
        SP,
        CL
    }

    public class ScriptParser:AbstractFileParser
    {
        private GameScript game;
        private FileStream fs;
        private BinaryReader br;


        public ScriptParser(GameScript game)
        {
            this.game = game;
            switch (game)
            {
                case GameScript.ISLAND:
                    InitDic("ISLAND");
                    break;
                case GameScript.SP:
                    InitDic("SP");
                    break;
                default:
                    throw new Exception("不支持的游戏类型");
                    break;
            }

        }
        public void DeCompress(string path)
        {
            if (!CanRead()) return;
            //FileStream ffs = new FileStream(path + ".out", FileMode.Create);
            //BinaryWriter bw = new BinaryWriter(ffs);
            FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
            StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);
            while (fs.Position < fs.Length)
            {
                byte[] tmp = ReadCodeBytes();
                tsw.WriteLine(DeCompressCode(tmp));
                //bw.Write(tmp);
                //while (ffs.Position % 16 != 0)
                //{
                //    bw.Write((byte)0xff);
                //}
                //bw.Write(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
            }
            tsw.Close();
            tfs.Close();
            //bw.Close();
            //ffs.Close();
        }
        public void IslandDeCompress(string path)
        {
            if (!CanRead()) return;
            FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
            StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);

            while (fs.Position < fs.Length)
            {
                byte[] tmp = ReadCodeBytes();
                //tsw.WriteLine(DeCompressCode(tmp));
                //tsw.WriteLine(Byte2Hex(tmp, true));
                tsw.WriteLine(IslandDeCompressCode(tmp));
            }
            tsw.Close();
            tfs.Close();
        }
        private byte[] ReadCodeBytes()
        {
            //[0x00 / null] [len:UInt16] [bytes]
            //              |<-     len      ->|
            if (!CanRead()) return new byte[0];
            List<byte> datas = new List<byte>();
            int len = 0;
            switch (game)
            {
                case GameScript.ISLAND:
                    byte opcode = br.ReadByte();
                    byte data = br.ReadByte();
                    len = data * 2;
                    fs.Seek(-2, SeekOrigin.Current);
                    break;
                case GameScript.SP:
                    // 00 填充部分，用0xFF标记
                    byte tmp = br.ReadByte();
                    while (tmp == 0x00)
                    {
                        datas.Add(0xFF);
                        tmp = br.ReadByte();
                    }
                    fs.Seek(-1, SeekOrigin.Current);

                    len = br.ReadUInt16() - 2;
                    break;
                default:
                    break;
            }
            
            if (Program.debug)
                Console.WriteLine("{0}  {1}", fs.Position, len);
            //datas.AddRange(BitConverter.GetBytes(len));
            datas.AddRange(br.ReadBytes(len ));
            return datas.ToArray();
        }
        private string DeCompressCode(byte[] line, bool rec = false)
        {
            MemoryStream ms = new MemoryStream(line);
            BinaryReader mbr = new BinaryReader(ms);
            string retn = "";
            byte scr_index = mbr.ReadByte();
            if (!decompress_dic.ContainsKey(scr_index))
            {
                switch (game)
                {
                    case GameScript.ISLAND:
                        throw new Exception("未知的opcode！");
                        break;
                    case GameScript.SP:
                        // 递归解析
                        if (scr_index == 0xFF) //0x00
                            retn = "    " + DeCompressCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                        else
                        {
                            retn = "[" + Byte2Hex(scr_index) + "]";
                            if (ms.Length - ms.Position > 0)
                                retn += " " + DeCompressCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                string flag = decompress_dic[scr_index].opcode;
                byte args = mbr.ReadByte();
                switch (flag)
                {
                    case "MESSAGE":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        break;
                    case "SELECT":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        break;
                    case "LOG":
                        retn += " " + DeCompressFunc(ref mbr, Type.Byte, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        break;
                    case "IFN":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.StringSJIS, Type.UInt32);
                        break;
                    case "IFY":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.StringSJIS, Type.UInt32);
                        break;
                    case "TASK":
                        if (args == 0x03)
                        {
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16);
                            UInt16 tmp = mbr.ReadUInt16();
                            retn += " (" + tmp.ToString() + ")";
                            if (tmp == 1)
                                retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        }
                        break;
                    case "FARCALL":
                        if (args == 0x00)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.Byte, Type.StringSJIS, Type.UInt16, Type.UInt16);
                        if (args == 0x01)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.StringSJIS, Type.UInt16, Type.UInt16);
                        break;
                    case "JUMP":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.StringSJIS);
                        break;
                    case "VARSTR":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16, Type.String);
                        break;
                    case "EQU":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.StringSJIS);
                        break;
                    case "GOTO":
                        if (args == 0x00)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16);
                        if (args == 0x01)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt32);
                        break;
                    case "END":
                        break;
                    default:
                        if (args == 0x01)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16);
                        if (args == 0x02)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16);
                        if (args == 0x03)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16);
                        break;

                }
                while (ms.Position + 1 < ms.Length)
                {
                    retn += " [" + Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
                }
                if (ms.Position < ms.Length)
                {
                    retn += " [" + Byte2Hex(mbr.ReadByte()) + "]";
                }
                retn = flag + " " + "[" + Byte2Hex(args) + "]" + retn;
            }


            if (retn != "    " && retn != "" && !rec && Program.debug)
            {
                Console.WriteLine(retn);
            }



            mbr.Close();
            ms.Close();
            return retn;
        }
        private string IslandDeCompressCode(byte[] line)
        {
            MemoryStream ms = new MemoryStream(line);
            BinaryReader mbr = new BinaryReader(ms);
            string retn = "";
            byte scr_index = mbr.ReadByte();
            if (!decompress_dic.ContainsKey(scr_index))
            {
                throw new Exception("未知的opcode！");
            }
            else
            {
                string flag = decompress_dic[scr_index].opcode;
                int len = mbr.ReadByte() * 2 - 2;
                string tmp = decompress_dic[scr_index].Load(ref mbr);
                retn += (tmp!="" ? " " : "") + tmp;
                while (ms.Position + 1 < ms.Length)
                {
                    retn += " [" + Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
                }
                if (ms.Position < ms.Length)
                {
                    retn += " [" + Byte2Hex(mbr.ReadByte()) + "]";
                }
                retn = flag + retn;

            }


            if (retn != "    " && retn != "" && Program.debug)
            {
                Console.WriteLine(retn);
            }



            mbr.Close();
            ms.Close();
            return retn;
        }


        public void Compress(string path)
        {
            FileStream ifs = new FileStream(path + ".txt", FileMode.Open);
            StreamReader isr = new StreamReader(ifs);

            FileStream ofs = new FileStream(path + ".scr", FileMode.Create);
            BinaryWriter obw = new BinaryWriter(ofs);
            while (isr.Peek()>=0)
            {
                obw.Write(CompressCode(isr.ReadLine()));
            }
            obw.Close();
            ofs.Close();
            isr.Close();
            ifs.Close();

        }
        private bool CanRead()
        {
            return fs.CanRead && fs.Position < fs.Length;
        }

        private byte[] CompressCode(string code)
        {
            code += " ";
            MemoryStream ms = new MemoryStream();
            BinaryWriter mbw = new BinaryWriter(ms);

            int i = 0;
            while(i+4 <= code.Length && code.Substring(i,4) == "    ")
            {
                mbw.Write((byte)0x00);
                i += 4;
            }
            int len_pos = (int)ms.Position;
            mbw.Write(BitConverter.GetBytes((UInt16)0));//长度填充
            string token = "";
            bool str = false;
            for(; i < code.Length; i++)
            {
                if (code[i] == '\"') str = !str;
                if (code[i]==' ' && !str)
                {

                    switch (token[0])
                    {
                        case '['://byte
                            mbw.Write(Hex2Byte(token.Substring(1, token.Length - 2)));
                            break;
                        case '('://uint16
                            mbw.Write(BitConverter.GetBytes(Convert.ToUInt16(token.Substring(1, token.Length - 2))));
                            break;
                        case '{'://uint32
                            mbw.Write(BitConverter.GetBytes(Convert.ToUInt32(token.Substring(1, token.Length - 2))));
                            break;
                        case 'u'://unicode string + 00 00
                            mbw.Write(Encoding.Unicode.GetBytes(token.Substring(2, token.Length - 3)));
                            mbw.Write(new byte[] { 0x00, 0x00 });
                            break;
                        case 'j'://sjis string + 00
                            mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(token.Substring(2, token.Length - 3)));
                            mbw.Write((byte)0x00);
                            break;
                        case 'p'://len + unicode string
                            mbw.Write(BitConverter.GetBytes((UInt16)(token.Length - 3)));
                            mbw.Write(Encoding.Unicode.GetBytes(token.Substring(2, token.Length - 3)));
                            break;
                        case 's'://len + sjis string
                            mbw.Write(BitConverter.GetBytes((UInt16)(token.Length - 3)));
                            mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(token.Substring(2, token.Length - 3)));
                            break;
                        default:
                            if (token[0] >= 'A' && token[0] <= 'Z' && compress_dic.ContainsKey(token))
                                mbw.Write(compress_dic[token]);
                            else
                                throw new Exception("未知的指令或错误的数据格式:" + token);
                            break;

                    }



                    token = "";
                }
                else
                {
                    token += code[i];
                }

            }
            UInt16 len = (UInt16)(ms.Position - len_pos);
            ms.Seek(len_pos,SeekOrigin.Begin);
            mbw.Write(BitConverter.GetBytes(len));

            byte[] retn = ms.ToArray();
            mbw.Close();
            ms.Close();
            return retn;

        }

        private string DeCompressFunc(ref BinaryReader tbr, params Type[] values)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string retn = "";
            bool end_flag = false;
            for (int i = 0; i < values.Length; i++)
            {
                string tmp = "";
                switch (values[i])
                {
                    case Type.Byte:
                    case Type.Byte2:
                    case Type.Byte3:
                    case Type.Byte4:
                        tmp = "[" + Byte2Hex(tbr.ReadBytes((int)values[i] + 1)) + "]";
                        break;
                    case Type.UInt16:
                        tmp = "(" + tbr.ReadUInt16().ToString() + ")";
                        break;
                    case Type.UInt32:
                        tmp = "{" + tbr.ReadUInt32().ToString() + "}";
                        break;
                    case Type.String:
                    case Type.StringSJIS:
                        {
                            List<byte> buff = new List<byte>();
                            if (values[i] == Type.String)
                            {
                                byte[] btmp = tbr.ReadBytes(2);
                                while (!(btmp[0] == 0x00 && btmp[1] == 0x00))
                                {
                                    buff.AddRange(btmp);
                                    btmp = tbr.ReadBytes(2);
                                }
                                tmp = "u\"" + Encoding.Unicode.GetString(buff.ToArray()) + "\"";
                            }
                            else if (values[i] == Type.StringSJIS)
                            {
                                byte btmp = tbr.ReadByte();
                                while (btmp != 0x00 )
                                {
                                    buff.Add(btmp);
                                    btmp = tbr.ReadByte();
                                }
                                Console.WriteLine(buff.Count);
                                tmp = "j\"" + Encoding.GetEncoding("Shift-Jis").GetString(buff.ToArray()) + "\"";
                            }
                            break;
                        }
                    case Type.PString:
                    case Type.PStringSJIS:
                        {
                            int len = tbr.ReadUInt16();
                            if(values[i] == Type.PString)
                                tmp = "p\"" + Encoding.Unicode.GetString(tbr.ReadBytes(len * 2)) + "\"";
                            else if (values[i] == Type.PStringSJIS)
                                tmp = "s\"" + Encoding.GetEncoding("Shift-Jis").GetString(tbr.ReadBytes(len * 2)) + "\"";
                            break;
                        }
                    case Type.Opcode:
                        byte scr_index = tbr.ReadByte();
                        if (decompress_dic.ContainsKey(scr_index))
                            tmp = decompress_dic[scr_index].opcode;
                        else
                            tmp = "[" + Byte2Hex(scr_index) + "]";
                        break;
                    default:
                        break;
                }

                retn += tmp + ((i != values.Length - 1 || end_flag) ? " " : "");
                retn = retn.Replace("\n", @"{\n}");
                if (end_flag) break;

            }
            return retn;
        }
        
        public void Close()
        {

            br.Close();

            fs.Close();
        }
        // 默认是SP的
        private Dictionary<byte, ScriptOpcode> decompress_dic = new Dictionary<byte, ScriptOpcode>();
        private Dictionary<string , byte> compress_dic = new Dictionary< string, byte>();

        private void InitDic(string game) //SP CL
        {

            string[] dic = new string[0];
            string path = Path.Combine("OPCODE", game + ".txt");
            if (File.Exists(path))
            {
                dic = File.ReadAllLines(path);
            }
            else
            {
                throw new Exception("未找到指定游戏");
            }
            decompress_dic.Clear();
            compress_dic.Clear();
            for (int i = 0; i < dic.Length; i++)
            {
                decompress_dic.Add((byte)i, new ScriptOpcode((byte)i,dic[i].Replace("\r", "")));
                compress_dic.Add( dic[i].Replace("\r", ""), (byte)i);
            }
        }
        public static byte[] Hex2Byte(string hexString)// 字符串转16进制字节数组
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Substring(0, 2).ToLower() == "0x")
                hexString.Remove(0, 2);
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public static string Byte2Hex(byte[] bytes, bool space = false, bool head = false)// 字节数组转16进制字符串
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                    if (space && i < bytes.Length-1) returnStr += " ";
                }
            }
            return (head ? "0x" : "") + returnStr;
        }
        public static string Byte2Hex(byte bytes)// 字节数组转16进制字符串
        {
            return bytes.ToString("X2");
        }

        public override void FileExport(string name)
        {
            fs = new FileStream(name, FileMode.Open);
            br = new BinaryReader(fs);
            IslandDeCompress(name);
        }

        public override void FileImport(string name)
        {
            fs = new FileStream(name, FileMode.Open);
            br = new BinaryReader(fs);
            Compress(name);
        }
    }

}
