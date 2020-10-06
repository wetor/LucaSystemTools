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
        FLOWERS_PSV,
        SP,
        CL,
        TAWL,
        FLOWERS,
        CUSTOM,
    }

    public class ScriptParser:AbstractFileParser
    {
        private GameScript game;
        private FileStream fs;
        private BinaryReader br;
        private int ScriptVersion = 3;

        private List<byte[]> code_bytes;
        // 文件位置，行号（1开始）
        private Dictionary<uint, int> code_position;

        // 行对应的位置
        private List<uint> line_position;
        // 位置，要跳转的行号（1开始）
        private Dictionary<uint, int> goto_position;

        public ScriptParser(GameScript game, string custom_game = "")
        {
            this.game = game;
            // Decompile
            code_bytes = new List<byte[]>();
            code_position = new Dictionary<uint, int>();

            // Compile
            goto_position = new Dictionary<uint, int>();
            line_position = new List<uint>();
            switch (game)
            {
                case GameScript.ISLAND:
                    InitDic("ISLAND");
                    ScriptVersion = 2;
                    break;
                case GameScript.FLOWERS_PSV:
                    InitDic("FLOWERS_PSV");
                    ScriptVersion = 2;
                    break;
                case GameScript.SP:
                    InitDic("SP");
                    break;
                case GameScript.TAWL:
                    InitDic("TAWL");
                    break;
                case GameScript.FLOWERS:
                    InitDic("FLOWERS");
                    break;
                case GameScript.CUSTOM:
                    InitDic(custom_game);
                    break;
                default:
                    throw new Exception("不支持的游戏类型！");
            }

        }
        public void Decompile(string path)
        {
            if (!CanRead()) return;
            code_bytes.Clear();
            code_position.Clear();
            FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
            StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);
            while (fs.Position < fs.Length)
            {
                code_position.Add((uint)fs.Position, code_bytes.Count + 1);
                byte[] tmp = ReadCodeBytes();
                code_bytes.Add(tmp);
                //tsw.WriteLine(DecompileCode(tmp));
            }
            DecompileAllCode(ref tsw);
            tsw.Close();
            tfs.Close();
        }
        private void DecompileAllCode(ref StreamWriter tsw)
        {
            ScriptOpcode.code_position = code_position;
            foreach (byte[] bytes in code_bytes)
            {
                tsw.WriteLine(DecompileCode(bytes));
            }
        }
        private byte[] ReadCodeBytes()
        {
            //[0x00 / null] [len:UInt16] [bytes]
            //              |<-     len      ->|
            if (!CanRead()) return new byte[0];
            List<byte> datas = new List<byte>();
            int len = 0;
            if(ScriptVersion == 2)
            {
                byte opcode = br.ReadByte();
                byte data = br.ReadByte();
                len = data * 2;
                fs.Seek(-2, SeekOrigin.Current);
            }
            else if(ScriptVersion == 3)
            {
                len = br.ReadUInt16() - 2;
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
                int param_num = 0;
                byte[] param_data = null;
                if (ScriptVersion == 3)
                {
                    param_num = mbr.ReadByte();
                    param_data = mbr.ReadBytes(param_num * 2);
                }

                string data = ScriptOpcode.ParamDataToString(decompress_dic[scr_index].ReadFunc(ref mbr));
                retn = (data.Length > 0 ? " " : "") + data;


                while (ms.Position + 1 < ms.Length)
                {
                    retn += " [" + ScriptUtil.Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
                }
                if (ms.Position < ms.Length)
                {
                    retn += " [" + ScriptUtil.Byte2Hex(mbr.ReadByte()) + "]";
                }
                if (ScriptVersion == 2)
                {
                    //len = flag2 * 2
                    retn = flag + retn;
                }
                else if (ScriptVersion == 3)
                {
                    param_num = param_data.Length / 2 < param_num ? param_data.Length / 2 : param_num;
                    string tmp = "(";
                    for(int i = 0; i < param_num; i++)
                    {
                        tmp += BitConverter.ToUInt16(param_data, i * 2).ToString();
                        if(i != param_num - 1)
                        {
                            tmp += ",";
                        }
                    }
                    tmp += ")";

                    retn = flag + tmp + retn;
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
            FileStream ifs = new FileStream(path, FileMode.Open);
            StreamReader isr = new StreamReader(ifs);
            FileStream ofs = new FileStream(path + ".scr", FileMode.Create);
            BinaryWriter obw = new BinaryWriter(ofs);
            goto_position.Clear();
            line_position.Clear();
            while (isr.Peek()>=0)
            {
                if (Program.debug)
                {
                    Console.WriteLine(isr.BaseStream.Position);
                }
                line_position.Add((uint)ofs.Position);
                obw.Write(CompileCode(isr.ReadLine(), ofs.Position));
            }
            CompileFixGoto(ref obw);
            obw.Close();
            ofs.Close();
            isr.Close();
            ifs.Close();

        }

        private void CompileFixGoto(ref BinaryWriter obw)
        {
            foreach (KeyValuePair<uint, int> kvp in goto_position)
            {
                obw.BaseStream.Seek(kvp.Key, SeekOrigin.Begin);
                obw.Write(BitConverter.GetBytes(line_position[kvp.Value - 1]));
            }
        }
        private bool CanRead()
        {
            return fs.CanRead && fs.Position < fs.Length;
        }

        private byte[] CompileCode(string code, long position)
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
                        case '<':
                            int goto_line = Convert.ToInt32(token.Substring(1, token.Length - 2));
                            goto_position.Add((uint)(position + ms.Position), goto_line);
                            mbw.Write(BitConverter.GetBytes((uint)0));
                            break;
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
                                else if (token[1] == 'c')//custom string + 00
                                {
                                    mbw.Write(CustomEncoding.GetBytes(tmp));
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
                                else if (token[1] == 'c')//custom string + 00
                                {
                                    mbw.Write(BitConverter.GetBytes((UInt16)tmp.Length));
                                    mbw.Write(CustomEncoding.GetBytes(tmp));
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
                            if (token[0] >= 'A' && token[0] <= 'Z')
                            {
                                if (ScriptVersion == 2 && compress_dic.ContainsKey(token))
                                {
                                    // [opcode] length/2 code
                                    // 1byte  1byte
                                    ms.Seek(len_pos, SeekOrigin.Begin);
                                    mbw.Write(compress_dic[token]);
                                    mbw.Write(new byte[1]);
                                }
                                else if (ScriptVersion == 3)
                                {
                                    int index_info = token.IndexOf('(');
                                    string info = "";
                                    if (index_info > 0)
                                    {
                                        info = token.Substring(index_info);
                                        token = token.Substring(0, index_info);
                                    }
                                    if (compress_dic.ContainsKey(token))
                                    {
                                        // length [opcode] code
                                        // 2byte  1byte
                                        mbw.Write(compress_dic[token]);
                                    }
                                    if (index_info > 0)
                                    {
                                        if (info.Length > 2) 
                                        {
                                            string[] info_split = info.Substring(1, info.Length - 2).Split(',');
                                            mbw.Write((byte)info_split.Length);
                                            foreach (string str1 in info_split)
                                            {
                                                mbw.Write(BitConverter.GetBytes(Convert.ToUInt16(str1)));
                                            }
                                        }
                                        else
                                        {
                                            mbw.Write((byte)0);
                                        }
                                        
                                    }
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
            if (ScriptVersion == 2)
            {
                // opcode [length/2] code
                // 1byte  1byte
                ms.Seek(len_pos + 1, SeekOrigin.Begin);
                mbw.Write((byte)(len / 2));
            }
            else if (ScriptVersion == 3)
            {
                // [length] opcode code
                // 2byte  1byte
                if (len % 2 != 0)
                {
                    mbw.Write((byte)0x00);
                }
                ms.Seek(len_pos, SeekOrigin.Begin);
                mbw.Write(BitConverter.GetBytes(len));
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
            if (dic.Length > 0)
            {
                if(dic[0].Trim() == ";Ver3")
                {
                    ScriptVersion = 3;
                }
                else if(dic[0].Trim() == ";Ver2")
                {
                    ScriptVersion = 2;
                }
            }
            int i = 0;
            foreach(string line in dic)
            {
                if (line.TrimStart()[0] == ';')
                {
                    continue;
                }
                decompress_dic.Add((byte)i, new ScriptOpcode((byte)i, line.Replace("\r", "")));
                compress_dic.Add(decompress_dic[(byte)i].opcode, (byte)i);
                i++;
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
            //fs = new FileStream(name, FileMode.Open);
            //br = new BinaryReader(fs);
            Compile(name);
        }
    }

}
