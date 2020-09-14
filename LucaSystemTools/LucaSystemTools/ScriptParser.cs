/*
 LucaSystem引擎.脚本部分
 目前完美支持Nintendo Switch版本的《Summer Pocket》以及PSVita《ISLAND》脚本反汇编以及汇编
 2020.9.13 追加Nintendo Switch版本的《Tomoyo After Its a Wonderful Life CS Edition》的完美支持
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
        private string DecompileCode(byte[] line)
        {
            if (line.Length == 0) return "";
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
                byte flag2 = mbr.ReadByte();
                string data = decompress_dic[scr_index].ReadFunc(ref mbr);
                retn += (data != "" ? " " : "") + data;
                while (ms.Position + 1 < ms.Length)
                {
                    retn += " [" + ScriptUtil.Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
                }
                if (ms.Position < ms.Length)
                {
                    retn += " [" + ScriptUtil.Byte2Hex(mbr.ReadByte()) + "]";
                }
                switch (game)
                {
                    case GameScript.ISLAND:
                        //len = flag2 * 2
                        retn = flag + retn;
                        break;
                    case GameScript.SP:
                    case GameScript.TAWL:
                        retn = flag + " " + "[" + ScriptUtil.Byte2Hex(flag2) + "]" + retn;
                        break;
                    default:
                        break;
                }
            }
            if (retn != "    " && retn != "" && Program.debug)
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
                            mbw.Write(ScriptUtil.Hex2Byte(token.Substring(1, token.Length - 2)));
                            break;
                        case '('://uint16
                            mbw.Write(BitConverter.GetBytes(Convert.ToUInt16(token.Substring(1, token.Length - 2))));
                            break;
                        case '{'://uint32
                            mbw.Write(BitConverter.GetBytes(Convert.ToUInt32(token.Substring(1, token.Length - 2))));
                            break;
                        case '$':
                            {
                                token = token.Replace(@"{\n}", "\n");
                                string tmp = token.Substring(3, token.Length - 4);
                                if (token[1] == 'u') //unicode string + 00 00
                                {
                                    mbw.Write(Encoding.Unicode.GetBytes(tmp));
                                    mbw.Write(new byte[] { 0x00, 0x00 });
                                }
                                else if (token[1] == 'j')//sjis string + 00
                                {
                                    mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(tmp));
                                    mbw.Write((byte)0x00);
                                }
                                else if (token[1] == '8')//utf8 string + 00
                                {
                                    mbw.Write(Encoding.UTF8.GetBytes(tmp));
                                    mbw.Write((byte)0x00);
                                }
                                break;
                            }
                        case '&':
                            {
                                token = token.Replace(@"{\n}", "\n");
                                string tmp = token.Substring(3, token.Length - 4);
                                if (token[1] == 'u') //len + unicode string
                                {
                                    mbw.Write(BitConverter.GetBytes((UInt16)tmp.Length));
                                    mbw.Write(Encoding.Unicode.GetBytes(tmp));
                                }
                                else if (token[1] == 'j')//len + sjis string
                                {
                                    mbw.Write(BitConverter.GetBytes((UInt16)tmp.Length));
                                    mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(tmp));
                                }
                                break;
                            }
                        case 'u': // 兼容保留
                            token = token.Replace(@"{\n}","\n");
                            mbw.Write(Encoding.Unicode.GetBytes(token.Substring(2, token.Length - 3)));
                            mbw.Write(new byte[] { 0x00, 0x00 });
                            break;
                        case 'j': // 兼容保留
                            token = token.Replace(@"{\n}", "\n");
                            mbw.Write(Encoding.GetEncoding("Shift-Jis").GetBytes(token.Substring(2, token.Length - 3)));
                            mbw.Write((byte)0x00);
                            break;
                        case 'p': // 兼容保留
                            token = token.Replace(@"{\n}", "\n");
                            mbw.Write(BitConverter.GetBytes((UInt16)(token.Length - 3)));
                            mbw.Write(Encoding.Unicode.GetBytes(token.Substring(2, token.Length - 3)));
                            break;
                        case 's': // 兼容保留
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
                    if (len % 2 != 0)
                    {
                        mbw.Write((byte)0x00);
                    }
                    ms.Seek(len_pos, SeekOrigin.Begin);
                    mbw.Write(BitConverter.GetBytes(len));
                    
                    break;
                default:
                    break;
            }

            

            byte[] retn = ms.ToArray();
            if (retn.Length>0 && Program.debug)
            {
                Console.WriteLine(ScriptUtil.Byte2Hex(retn, true));
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
