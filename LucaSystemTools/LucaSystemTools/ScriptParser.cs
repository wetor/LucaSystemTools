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
    public class ScriptParser:AbstractFileParser
    {
        private FileStream fs;
        private BinaryReader br;
        private enum Type
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
            Script
        }

        public ScriptParser(string dicname)
        {
            InitDic(dicname);
        }
        //public void DeCompressISLAND2(string path)
        //{
        //    if (!CanRead()) return;

        //    FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
        //    StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);
        //    int index = 0;
        //    while (fs.Position < fs.Length)
        //    {
               
        //        //tsw.WriteLine(Byte2Hex(tmp, true));
        //        tsw.WriteLine(DeCompressCodeISLAND(ref index));
        //    }
        //    tsw.Close();
        //    tfs.Close();
        //}
        //private byte[] ReadCodeBytesISLAND()
        //{
        //    //[0x00 / null] [len:UInt16] [bytes]
        //    //              |<-     len      ->|
        //    if (!CanRead()) return new byte[0];
        //    List<byte> datas = new List<byte>();
        //    byte tmp;
        //    while (true)
        //    {
        //        if (!CanRead())
        //            break;
        //        tmp = br.ReadByte();
        //        datas.Add(tmp);
        //        if (tmp == 0x00) //0x00 xx
        //        {
        //            byte tmp2 = br.ReadByte();
        //            if (tmp2 == 0x02 || tmp2 ==0x05 || tmp2 == 0xFF)
        //            {                        
        //                datas.Add(tmp2);
        //                break;
        //            }else
        //                fs.Seek(-1, SeekOrigin.Current);
        //        }
        //        if (tmp == 0xFF) //0xFF 00
        //        {
        //            byte tmp2 = br.ReadByte();
        //            if (tmp2 == 0x00)
        //            {
        //                datas.Add(tmp2);
        //                break;
        //            }
        //            else
        //                fs.Seek(-1, SeekOrigin.Current);
        //        }
        //        if (tmp == 0x01 ) //0x xx 00
        //        {
        //            byte tmp2 = br.ReadByte();
        //            if (tmp2 == 0x00 && !(datas[0]==0x18 && datas.Count<=6))
        //            {
        //                datas.Add(tmp2);
        //                break;
        //            }
        //            else
        //                fs.Seek(-1, SeekOrigin.Current);
        //        }
        //    }
        //    if (CanRead())
        //    {
        //        tmp = br.ReadByte();
        //        while (tmp == 0xFF || tmp == 0x00)
        //        {
        //            datas.Add(tmp);
        //            tmp = br.ReadByte();
        //        }
        //        fs.Seek(-1, SeekOrigin.Current);
        //    }
            
        //    if (Program.debug)
        //        Console.WriteLine("{0}  {1}", fs.Position, datas.Count);
        //    //datas.AddRange(BitConverter.GetBytes(len));
        //    return datas.ToArray();
        //}
        //private string DeCompressCodeISLAND(ref int index)
        //{
        //    string retn = "";
        //    int end = -1;
        //    int ch = 0;//读取字符串临时变量
        //    List<byte> datas = new List<byte>();
        //    byte scr_index = br.ReadByte();
        //    if (!decompress_dic.ContainsKey(scr_index))
        //    {
        //        return "[" + Byte2Hex(scr_index) + "]"; ;
        //    }
        //    else
        //    {
        //        string flag = decompress_dic[scr_index];
                
        //        switch (flag)
        //        {
        //            case "MESSAGE":
        //                datas.Clear();
        //                br.ReadBytes(2);//序号01，从XX XX 80开始递增
        //                if (br.ReadByte() < 0x80)
        //                {
        //                    fs.Seek(-2, SeekOrigin.Current);
        //                    break;
        //                }
        //                ushort text_index = br.ReadUInt16();//文本序号
        //                if (text_index != index)
        //                {
        //                    throw new Exception("index不匹配！true_idx:" + index + "  idx:" + text_index + "  pos:" + fs.Position);
        //                }
        //                ushort voice_index = br.ReadUInt16();//序号02，可能是语音序号
        //                retn += "(" + text_index + ") (" + voice_index + ")";
                       
        //                while (true)
        //                {
        //                    end = br.ReadUInt16();
        //                    if (end == 0x0500 || end == 0x0400 || end == 0x0300 || end == 0x0200 || end == 0x0100)
        //                        break; 
        //                    else
        //                        fs.Seek(-2, SeekOrigin.Current);
        //                    datas.Add(br.ReadByte());
        //                }
        //                string str_line = Encoding.GetEncoding("Shift-JIS").GetString(datas.ToArray());
        //                retn += " \"" + str_line + "\"";
        //                index++;
        //                break;
        //            case "IFN":
        //                byte ifn_type = br.ReadByte();
        //                retn += "[" + Byte2Hex(ifn_type) + "]";
        //                if (ifn_type == 0)
        //                {
        //                    retn += " [" + Byte2Hex(br.ReadBytes(4)) + "]";
        //                }
        //                else
        //                {
        //                    retn += " (" + br.ReadUInt16() + ")";
        //                    retn += " \"";
        //                    ch = br.ReadChar();
        //                    while (ch != 0)
        //                    {
        //                        retn += (char)ch;
        //                        ch = br.ReadChar();
        //                    }
        //                    retn += "\" {" + br.ReadUInt32() + "}";
        //                    retn += " [" + Byte2Hex(br.ReadBytes(2)) + "]";
        //                }
                        
        //                break;
        //            case "JUMP":
        //                byte jump_type = br.ReadByte();
        //                retn += "[" + Byte2Hex(jump_type) + "]";
        //                if(jump_type == 3)
        //                {
        //                    retn += " "+Byte2Hex(br.ReadBytes(4), true);
        //                }
        //                else if(jump_type == 0xC)
        //                {
        //                    retn += " " + Byte2Hex(br.ReadBytes(2), true);
        //                }
        //                else
        //                {
        //                    ushort jump = br.ReadUInt16();
        //                    retn += " (" + jump + ")";
        //                    if (jump != 0)
        //                    {
        //                        retn += " \"";
        //                        ch = br.ReadChar();
        //                        while (ch != 0)
        //                        {
        //                            retn += (char)ch;
        //                            ch = br.ReadChar();
        //                        }
        //                        retn += "\" " + Byte2Hex(br.ReadBytes(4), true);
        //                    }
        //                    else
        //                    {
        //                        retn += " "+Byte2Hex(br.ReadBytes(2), true);
        //                    }
        //                }
        //                break;
        //            case "GOTO":
        //                byte goto_type = br.ReadByte();
        //                if (goto_type == 0xC)
        //                {
        //                    retn += Byte2Hex(br.ReadBytes(2), true);
        //                }
        //                else
        //                {
        //                    retn += Byte2Hex(br.ReadBytes(7), true);
        //                }
                        
        //                break;
        //            case "MANPU":
        //                retn += Byte2Hex(br.ReadBytes(5+8+3), true);
        //                break;
        //            case "VOLUME":
        //                retn += Byte2Hex(br.ReadBytes(7), true);
        //                break;
        //            case "FADE":
        //                retn += Byte2Hex(br.ReadBytes(7), true);
        //                break;
        //            case "EQUV":
        //                retn += Byte2Hex(br.ReadBytes(7), true);
        //                break;
        //            case "EQU":
        //                break;
        //            case "LOG_END":
        //                retn += Byte2Hex(br.ReadBytes(1), true);
        //                break;
        //            case "PLAYMUSIC":
        //                retn += Byte2Hex(br.ReadBytes(7), true);
        //                break;
        //            case "PLAYSE":
        //                retn += Byte2Hex(br.ReadBytes(5), true);
        //                break;
        //            case "ARFLAGSET":
        //                retn += Byte2Hex(br.ReadBytes(1), true);
        //                break;
        //            case "SUB":
        //                retn += Byte2Hex(br.ReadBytes(2), true);
        //                break;
        //            case "WAIT":
        //                retn += Byte2Hex(br.ReadBytes(7), true);
        //                break;
        //            case "INIT":
        //                retn += Byte2Hex(br.ReadBytes(5), true);
        //                break;
        //            case "CLOSE_WINDOW":
        //                retn += Byte2Hex(br.ReadBytes(3), true);
        //                break;
        //            case "IMAGELOAD":
        //                retn += Byte2Hex(br.ReadBytes(11), true);
        //                break;
        //            default:
        //                datas.Clear();
        //                while (true)
        //                {
                            
        //                    end = br.ReadUInt16();
        //                    if (end == 0xFF00 || end == 0x00FF || end == 0xFFFF || end == 0x0000 || end ==0xFF01 || end == 0x0014)
        //                        break;
        //                    else
        //                        fs.Seek(-2, SeekOrigin.Current);
        //                    datas.Add(br.ReadByte());
        //                }
        //                retn += Byte2Hex(datas.ToArray(), true);
        //                break;

        //        }
        //        if (end >= 0)
        //        {
        //            retn += " [" + Byte2Hex(BitConverter.GetBytes((ushort)end)) + "]";
        //        }
        //        /*ch = br.ReadByte();
        //        while (ch == 0)
        //        {
        //            retn += " [00]";
        //            ch = br.ReadByte();
        //        }*/
        //        /*while (fs.Position + 1 < fs.Length)
        //        {
        //            retn += " [" + Byte2Hex(BitConverter.GetBytes(br.ReadUInt16())) + "]";
        //        }
        //        if (fs.Position < fs.Length)
        //        {
        //            retn += " [" + Byte2Hex(br.ReadByte()) + "]";
        //        }*/
        //        retn = flag + " "+ retn;

        //    }


        //    if (retn != "    " && retn != ""  && Program.debug)
        //    {
        //        Console.WriteLine(retn);
        //    }

        //    return retn;
        //}
        //public void DeCompressISLAND()
        //{
        //    List<string> ctrl_str = new List<string>();
        //    StreamWriter sw_ctrl = new StreamWriter(path + "..ctl");
        //    //Test01(pathnames[ss], save_path);
        //    /*if (scr_file.IndexOf("FLOWJUMP") >= 0)
        //    return;*/
        //    //List<byte> bytes = new List<byte>();
        //    List<string> strs = new List<string>();

        //    //bytes.AddRange(br.ReadBytes((int)fs.Length));


        //    //int i = 0;
        //    bool spker = false;//说话者标记
        //    int line_num = 0;
        //    while (fs.Position < fs.Length)
        //    {
        //        byte type = br.ReadByte();
        //        if (type == 0x18)//文本开头
        //        {
        //            if (fs.Position >= fs.Length) break;
                    
        //            //Console.WriteLine("{0}", fs.Position);
        //            br.ReadBytes(3);//序号01，从XX XX 80开始递增
        //            if (fs.Position >= fs.Length) break;
        //            if (br.ReadUInt16() == line_num)//文本序号
        //            {
        //                if (Program.debug)
        //                    Console.WriteLine("{0} {1}", fs.Position, line_num);
        //                if (fs.Position >= fs.Length) break;
        //                br.ReadBytes(2);//序号02，可能是语音序号
        //                spker = false;
        //                if (br.ReadByte() == 0x60)//存在说话者
        //                    spker = true;
        //                else
        //                    fs.Seek(-1, SeekOrigin.Current);
        //                List<byte> str_tmp = new List<byte>();
        //                while (true)
        //                {
        //                    str_tmp.Add(br.ReadByte());
        //                    UInt16 str_end = br.ReadUInt16();
        //                    if (str_end == 0x0500 || str_end == 0x0400 || str_end == 0x0300 || str_end == 0x0200 || str_end == 0x0100)
        //                        break;
        //                    else
        //                        fs.Seek(-2, SeekOrigin.Current);
        //                }
        //                string str_line = Encoding.GetEncoding("Shift-JIS").GetString(str_tmp.ToArray());
        //                strs.Add(str_line);
        //                line_num++;
        //            }
        //            else
        //            {
        //                fs.Seek(-5, SeekOrigin.Current);
        //            }
        //        }
        //        if (type == 0x71)//选项开头01
        //        {
        //            if (br.ReadByte() == 0x17)//选项开头02
        //            {
        //                br.ReadBytes(2);//未知
        //                if (br.ReadUInt32() == 0x40000000)//@
        //                {
        //                    List<byte> str_tmp = new List<byte>();
        //                    while (true)
        //                    {
        //                        str_tmp.Add(br.ReadByte());
        //                        UInt16 str_end = br.ReadUInt16();
        //                        if (str_end == 0x0000)
        //                            break;
        //                        else
        //                            fs.Seek(-2, SeekOrigin.Current);
        //                    }
        //                    string str_line = Encoding.GetEncoding("Shift-JIS").GetString(str_tmp.ToArray());
        //                    strs.Add(str_line);
        //                }
        //                else
        //                {
        //                    fs.Seek(-7, SeekOrigin.Current);
        //                }

        //            }
        //            else
        //            {
        //                fs.Seek(-1, SeekOrigin.Current);
        //            }
        //        }
        //    }

        //    br.Close();
        //    fs.Close();
        //    StreamWriter sw = new StreamWriter(path + ".txt");

        //    Console.WriteLine("filename:{0}  num:{1}", Path.GetFileName(path), strs.Count);
        //    foreach (string line in strs)
        //    {
        //        if (line == "　" || line == "")
        //            continue;
        //        int find_index = line.IndexOf("$");
        //        while (find_index >= 0)
        //        {
        //            string tmp = "";//Path.GetFileName(filename) + "," + i.ToString() + ",";
        //            do
        //            {
        //                tmp += line[find_index];
        //                if (find_index + 1 < line.Length)
        //                    find_index++;
        //                else
        //                    break;
        //            } while (line[find_index] < 256 && line[find_index] != '$');
        //            if (ctrl_str.IndexOf(tmp) < 0)
        //                ctrl_str.Add(tmp);
        //            find_index = line.IndexOf("$", find_index + 1);
        //        }
        //        //Console.WriteLine(line.Replace("\n",@"\n"));

        //        sw.WriteLine(line);
        //        //sw.WriteLine();
        //    }
        //    //Console.ReadKey();
        //    sw.Close();


        //    foreach (string str in ctrl_str)
        //    {
        //        sw_ctrl.WriteLine(str);
        //    }
        //    sw_ctrl.Close();
        //    Console.ReadKey();
        //}
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
        private byte[] ReadCodeBytes()
        {
            //[0x00 / null] [len:UInt16] [bytes]
            //              |<-     len      ->|
            if (!CanRead()) return new byte[0];
            List<byte> datas = new List<byte>();
            byte tmp = br.ReadByte();
            while(tmp == 0x00)
            {
                datas.Add(0xFF);        
                tmp = br.ReadByte();
            }
            fs.Seek(-1, SeekOrigin.Current);
           
            UInt16 len = br.ReadUInt16();
            if (Program.debug)
                Console.WriteLine("{0}  {1}", fs.Position,len);
            //datas.AddRange(BitConverter.GetBytes(len));
            datas.AddRange(br.ReadBytes(len - 2));
            return datas.ToArray();
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
                                while (btmp[0] != 0x00 || btmp[1] != 0x00)
                                {
                                    buff.AddRange(btmp);
                                    btmp = tbr.ReadBytes(2);
                                }
                                tmp = "u\"" + Encoding.Unicode.GetString(buff.ToArray()) + "\"";
                            }
                            else if (values[i] == Type.StringSJIS)
                            {
                                byte btmp = tbr.ReadByte();
                                while (btmp != 0x00)
                                {
                                    buff.Add(btmp);
                                    btmp = tbr.ReadByte();
                                }
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
                    case Type.Script:
                        byte scr_index = tbr.ReadByte();
                        if (decompress_dic.ContainsKey(scr_index))
                            tmp = decompress_dic[scr_index];
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
        private string DeCompressCode(byte[] line, bool rec = false)
        {
            MemoryStream ms = new MemoryStream(line);
            BinaryReader mbr = new BinaryReader(ms);
            string retn = "";
            byte scr_index = mbr.ReadByte();
            if (!decompress_dic.ContainsKey(scr_index))
            {
                if(scr_index == 0xFF) //0x00
                    retn = "    " + DeCompressCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                else
                {
                    retn = "[" + Byte2Hex(scr_index) + "]";
                    if (ms.Length - ms.Position > 0)
                        retn += " " + DeCompressCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                }
            }
            else
            {
                string flag = decompress_dic[scr_index];
                byte args = mbr.ReadByte();
                switch (flag)
                {
                    case "MESSAGE":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16,Type.PString);
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
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16,  Type.Byte,  Type.StringSJIS, Type.UInt16, Type.UInt16);
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
#if false
                switch (flag)
                {
                    //case "EQUV"://02
                    //    {
                    //        retn = flag + " " + flag2;
                    //        ms.Seek(-1, SeekOrigin.Current);
                    //        retn += " EQUV " + DeCompressCode(mbr.ReadBytes((int)ms.Length - 1), true);
                    //        break;
                    //    }
                    case "MESSAGE"://0x22
                        {
                            retn = flag + " " + flag2;
                            int num = mbr.ReadUInt16();
                            int index = mbr.ReadUInt16();
                            int unknow0 = mbr.ReadUInt16();
                            int len = mbr.ReadUInt16();
                            string str = Encoding.Unicode.GetString(mbr.ReadBytes(len * 2));
                            int end = mbr.ReadUInt16();
                            string end2 = script_list[mbr.ReadByte()];
                            retn += " " + num.ToString() + " " + index.ToString() + " " + unknow0.ToString() + " " + str + " " + end.ToString() + " " + end2;

                            break;
                        }
                    case "SELECT"://0x25
                        {
                            retn = flag + " " + flag2;
                            for (int i = 0; i < 5; i++)
                            {
                                retn += " " + mbr.ReadUInt16().ToString();
                            }
                            int len = mbr.ReadUInt16();
                            string str = Encoding.Unicode.GetString(mbr.ReadBytes(len * 2));
                            retn += " " + str;
                            for (int i = 0; i < 4; i++)
                            {
                                retn += " " + mbr.ReadUInt16().ToString();
                            }

                            break;
                        }
                    case "LOG"://0x27
                        {
                            retn = flag + " " + flag2;
                            retn += " " + script_list[mbr.ReadByte()];
                            for (int i = 0; i < 3; i++)
                            {
                                retn += " " + mbr.ReadUInt16().ToString();
                            }
                            int len = mbr.ReadUInt16();
                            string str = Encoding.Unicode.GetString(mbr.ReadBytes(len * 2));
                            retn += " " + str;
                            retn += " " + mbr.ReadUInt16().ToString();
                            retn += " " + script_list[mbr.ReadByte()];
                            break;
                        }
                    case "TASK"://0x6E
                        {
                            retn = flag + " " + flag2;
                            for (int i = 0; i < 8; i++)
                            {
                                retn += " " + mbr.ReadUInt16().ToString();
                            }
                            int len = mbr.ReadUInt16();
                            string str = Encoding.Unicode.GetString(mbr.ReadBytes(len * 2));
                            retn += " " + str;
                            retn += " " + mbr.ReadUInt16().ToString();
                            break;
                        }
                    case "FARCALL"://0x16
                        {
                            retn = flag + " " + flag2;
                            for (int i = 0; i < 2; i++)
                            {
                                retn += " " + mbr.ReadUInt16().ToString();
                            }
                            List<byte> buff = new List<byte>();
                            byte tmp = mbr.ReadByte();
                            while (tmp != 0x00)
                            {
                                buff.Add(tmp);
                                tmp = mbr.ReadByte();
                            }
                            string str = Encoding.GetEncoding("Shift-Jis").GetString(buff.ToArray());
                            retn += " " + str;
                            retn += " " + mbr.ReadUInt16().ToString();
                            retn += " " + mbr.ReadUInt16().ToString();
                            break;
                        }
                    case "VARSTR"://0x0B
                        {
                            retn = flag + " " + flag2;
                            for (int i = 0; i < 3; i++)
                            {
                                retn += " " + mbr.ReadUInt16().ToString();
                            }
                            List<byte> buff = new List<byte>();
                            byte[] tmp = mbr.ReadBytes(2);
                            while (tmp[0] != 0x00 || tmp[1] != 0x00)
                            {
                                buff.AddRange(tmp);
                                tmp = mbr.ReadBytes(2);
                            }

                            string str = Encoding.Unicode.GetString(buff.ToArray());
                            retn += " " + str;

                            break;
                        }

                    case "IFN"://0x13
                        {
                            retn = flag + " " + flag2;
                            int unknow0 = mbr.ReadUInt16();
                            string str0 = "";
                            byte tmp = mbr.ReadByte();
                            while (tmp != 0x00)
                            {
                                str0 += (char)tmp;
                                tmp = mbr.ReadByte();
                            }
                            int unknow1 = mbr.ReadUInt16();
                            retn += " " + unknow0.ToString() + " " + str0 + " " + unknow1.ToString();

                            break;
                        }
                    default:
                        retn = flag + " " + flag2;


                        break;
                }
                while (ms.Position + 1 < ms.Length)
                {
                    retn += " " + mbr.ReadUInt16().ToString();
                }
                if (ms.Position < ms.Length)
                {
                    retn += " " + "[" + mbr.ReadByte().ToString() + "]";
                }
#endif
            }
            
            
            if (retn != "    " && retn != "" && !rec && Program.debug)
            {
               Console.WriteLine(retn);
            }
                


            mbr.Close();
            ms.Close();
            return retn;
        }
        public void Close()
        {

            br.Close();

            fs.Close();
        }
        // 默认是SP的
        private Dictionary<byte, string> decompress_dic = new Dictionary<byte, string>();
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
                decompress_dic.Add((byte)i, dic[i].Replace("\r", ""));
                compress_dic.Add( dic[i].Replace("\r", ""), (byte)i);
            }
        }
        private byte[] Hex2Byte(string hexString)// 字符串转16进制字节数组
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
        private string Byte2Hex(byte[] bytes, bool space = false, bool head = false)// 字节数组转16进制字符串
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
        private string Byte2Hex(byte bytes)// 字节数组转16进制字符串
        {
            return bytes.ToString("X2");
        }

        public override void FileExport(string name)
        {
            fs = new FileStream(name, FileMode.Open);
            br = new BinaryReader(fs);
            DeCompress(name);
        }

        public override void FileImport(string name)
        {
            fs = new FileStream(name, FileMode.Open);
            br = new BinaryReader(fs);
            Compress(name);
        }
    }

}
