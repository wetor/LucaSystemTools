/*
 LucaSystem引擎.脚本部分
 目前完美支持Nintendo Switch版本的《Summer Pocket》以及PSVita《ISLAND》脚本反汇编以及汇编（但并不完善）
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
        CL,
        TAWL
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
                case GameScript.TAWL:
                    InitDic("TAWL");
                    break;
                default:
                    throw new Exception("不支持的游戏类型");
                    break;
            }

        }
        public void Decompile(string path)
        {
            if (!CanRead()) return;
            FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
            StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);
            while (fs.Position < fs.Length)
            {
                byte[] tmp = ReadCodeBytes();
                tsw.WriteLine(DecompileCode(tmp));
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
                case GameScript.TAWL:
                    len = br.ReadUInt16() - 2;
                    break;
                default:
                    break;
            }
            
            if (Program.debug)
                Console.WriteLine("{0}  {1}", fs.Position, len);

            datas.AddRange(br.ReadBytes(len));
            if (len % 2 != 0)
            {
                //长度非偶数，最后会有补位
                br.ReadByte();
            }
            return datas.ToArray();
        }
        private string DecompileCode(byte[] line, bool rec = false)
        {
            if (line.Length == 0) return "";
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
                    case GameScript.TAWL:
                        retn = "[" + Byte2Hex(scr_index) + "]";
                        if (ms.Length - ms.Position > 0)
                            retn += " " + DecompileCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                string flag = decompress_dic[scr_index].opcode;
                byte flag2 = mbr.ReadByte();
                string data = decompress_dic[scr_index].Load(ref mbr);
                retn += (data != "" ? " " : "") + data;
                while (ms.Position + 1 < ms.Length)
                {
                    retn += " [" + Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
                }
                if (ms.Position < ms.Length)
                {
                    retn += " [" + Byte2Hex(mbr.ReadByte()) + "]";
                }
                switch (game)
                {
                    case GameScript.ISLAND:
                        //len = flag2 * 2
                        retn = flag + retn;
                        break;
                    case GameScript.SP:
                    case GameScript.TAWL:
                        retn = flag + " " + "[" + Byte2Hex(flag2) + "]" + retn;
                        break;
                    default:
                        break;
                }
            }
            if (retn != "    " && retn != "" && !rec && Program.debug)
            {
                Console.WriteLine(retn);
            }
            mbr.Close();
            ms.Close();
            return retn;
        }

        public void Compile(string path)
        {
            FileStream ifs = new FileStream(path + ".txt", FileMode.Open);
            StreamReader isr = new StreamReader(ifs);

            FileStream ofs = new FileStream(path + ".scr", FileMode.Create);
            BinaryWriter obw = new BinaryWriter(ofs);
            while (isr.Peek()>=0)
            {
                obw.Write(CompileCode(isr.ReadLine()));
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

        private byte[] CompileCode(string code)
        {
            code += " ";
            MemoryStream ms = new MemoryStream();
            BinaryWriter mbw = new BinaryWriter(ms);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int i = 0;
            while(i+4 <= code.Length && code.Substring(i,4) == "    ")
            {
                mbw.Write((byte)0x00);
                i += 4;
            }
            int len_pos = (int)ms.Position;
            mbw.Write(new byte[2]);//长度填充

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
                            token = token.Replace(@"{\n}","\n");
                            mbw.Write(Encoding.Unicode.GetBytes(token.Substring(2, token.Length - 3)));
                            mbw.Write(new byte[] { 0x00, 0x00 });
                            break;
                        case 'j'://sjis string + 00
                            token = token.Replace(@"{\n}", "\n");
                            mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(token.Substring(2, token.Length - 3)));
                            mbw.Write((byte)0x00);
                            break;
                        case 'p'://len + unicode string
                            token = token.Replace(@"{\n}", "\n");
                            mbw.Write(BitConverter.GetBytes((UInt16)(token.Length - 3)));
                            mbw.Write(Encoding.Unicode.GetBytes(token.Substring(2, token.Length - 3)));
                            break;
                        case 's'://len + sjis string
                            token = token.Replace(@"{\n}", "\n");
                            mbw.Write(BitConverter.GetBytes((UInt16)(token.Length - 3)));
                            mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(token.Substring(2, token.Length - 3)));
                            break;
                        default:
                            if (token[0] >= 'A' && token[0] <= 'Z' && compress_dic.ContainsKey(token))
                            {
                                switch (game)
                                {
                                    case GameScript.ISLAND:
                                        // [opcode] length/2 code
                                        // 1byte  1byte
                                        ms.Seek(len_pos, SeekOrigin.Begin);
                                        mbw.Write(compress_dic[token]);
                                        mbw.Write(new byte[1]);
                                        break;
                                    case GameScript.SP:
                                    case GameScript.TAWL:
                                        // length [opcode] code
                                        // 2byte  1byte
                                        mbw.Write(compress_dic[token]);
                                        break;
                                    default:
                                        break;
                                }
                            }
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
            switch (game)
            {
                case GameScript.ISLAND:
                    // opcode [length/2] code
                    // 1byte  1byte
                    ms.Seek(len_pos+1, SeekOrigin.Begin);
                    mbw.Write((byte)(len / 2));
                    break;
                case GameScript.SP:
                case GameScript.TAWL:
                    // [length] opcode code
                    // 2byte  1byte
                    ms.Seek(len_pos, SeekOrigin.Begin);
                    mbw.Write(BitConverter.GetBytes(len));
                    break;
                default:
                    break;
            }

            

            byte[] retn = ms.ToArray();
            if (retn.Length>0 && Program.debug)
            {
                Console.WriteLine(ScriptParser.Byte2Hex(retn, true));
            }
            mbw.Close();
            ms.Close();
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
                compress_dic.Add(decompress_dic[(byte)i].opcode, (byte)i);
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
            Decompile(name);
        }

        public override void FileImport(string name)
        {
            fs = new FileStream(name, FileMode.Open);
            br = new BinaryReader(fs);
            Compile(name);
        }
    }

}
