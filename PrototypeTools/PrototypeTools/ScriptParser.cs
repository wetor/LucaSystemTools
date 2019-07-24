using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AdvancedBinary;

namespace ProtScript
{
    public class ScriptParser
    {
        private string path;
        private FileStream fs;
        private BinaryReader br;
        private Dictionary<byte[], string> dic_encode = new Dictionary<byte[], string>();
        enum Type
        {
            Byte,
            Byte2,
            Byte3,
            Byte4,
            UInt16,
            UInt32,
            String,
            StringUTF8,
            StringSJIS,
            PString,
            PStringUTF8,
            PStringSJIS,
            Script
        }

        private void DicInit()
        {
            dic_encode.Add(new byte[] { 0x22, 0x03 }, "VARSTR");
            dic_encode.Add(new byte[] { 0x00, 0x00, 0x0B }, "VARSTR_END");
            //VARSTR [num] [index] [unknow] [string] 
        }
        public ScriptParser(string file)
        {
            path = file;
            fs = new FileStream(path, FileMode.Open);
            br = new BinaryReader(fs);
            DicInit();
        }
        public void Decompress()
        {
            if (!CanRead()) return;
            FileStream ffs = new FileStream(path + ".out", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(ffs);
            FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
            StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);
            while (fs.Position < fs.Length)
            {
                byte[] tmp = ReadCodeBytes();
               
                tsw.WriteLine(DeCompressCode(tmp));
                bw.Write(tmp);
                while (ffs.Position % 16 != 0)
                {
                    bw.Write((byte)0xff);
                }
                bw.Write(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
            }
            tsw.Close();
            tfs.Close();
            bw.Close();
            ffs.Close();


            ;
        }
        private bool CanRead()
        {
            return fs.CanRead && fs.Position < fs.Length;
        }
        private byte[] ReadCodeBytes()
        {
            if (!CanRead()) return new byte[0];
            List<byte> datas = new List<byte>();
            byte tmp = br.ReadByte();
            while(tmp == 0x00)
            {
                datas.Add(0xFF);//00:FF  //01:FE  02:FD
                tmp = br.ReadByte();
            }
            fs.Seek(-1, SeekOrigin.Current);
           
            UInt16 len = br.ReadUInt16();
            Console.WriteLine("{0}  {1}", fs.Position,len);
            //datas.AddRange(BitConverter.GetBytes(len));
            datas.AddRange(br.ReadBytes(len - 2));
            return datas.ToArray();
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
                        tmp = tbr.ReadUInt16().ToString();
                        break;
                    case Type.UInt32:
                        tmp = tbr.ReadUInt32().ToString();
                        break;
                    case Type.String:
                        {
                            List<byte> buff = new List<byte>();
                            byte[] btmp = tbr.ReadBytes(2);
                            while (btmp[0] != 0x00 || btmp[1] != 0x00)
                            {
                                buff.AddRange(btmp);
                                btmp = tbr.ReadBytes(2);
                            }
                            tmp = "\"" + Encoding.Unicode.GetString(buff.ToArray()) + "\"";
                            break;
                        }
                    case Type.StringUTF8:
                    case Type.StringSJIS:
                        {
                            List<byte> buff = new List<byte>();
                            byte btmp = tbr.ReadByte();
                            while (btmp != 0x00)
                            {
                                buff.Add(btmp);
                                btmp = tbr.ReadByte();
                            }
                            if(values[i]== Type.StringUTF8)
                                tmp = Encoding.UTF8.GetString(buff.ToArray()) ;
                            else if(values[i] == Type.StringSJIS)
                                tmp = Encoding.GetEncoding("Shift-Jis").GetString(buff.ToArray());
                            tmp = "\"" + tmp + "\"";
                            break;
                        }
                    case Type.PString:
                    case Type.PStringUTF8:
                    case Type.PStringSJIS:
                        {
                            int len = tbr.ReadUInt16();
                            if(values[i] == Type.PString)
                                tmp = Encoding.Unicode.GetString(tbr.ReadBytes(len * 2)) ;
                            else if (values[i] == Type.PStringUTF8)
                                tmp = Encoding.UTF8.GetString(tbr.ReadBytes(len * 2));
                            else if (values[i] == Type.PStringSJIS)
                                tmp = Encoding.GetEncoding("Shift-Jis").GetString(tbr.ReadBytes(len * 2));
                            tmp = "\"" + tmp + "\"";
                            break;
                        }
                    case Type.Script:
                        byte scr_index = tbr.ReadByte();
                        if (script_list.ContainsKey(scr_index))
                            tmp = script_list[scr_index];
                        else
                            tmp = "[" + Byte2Hex(tbr.ReadBytes(1)) + "]";
                        break;
                    default:
                        break;
                }

                retn += tmp + ((i != values.Length - 1 || end_flag) ? " " : "");
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
            if (!script_list.ContainsKey(scr_index))
            {
                if(scr_index == 0xFF) //0x00
                    retn = "    " + DeCompressCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                else
                {
                    retn = "[" + scr_index.ToString("X2") + "]";
                    if (ms.Length - ms.Position > 0)
                        retn += " " + DeCompressCode(mbr.ReadBytes((int)(ms.Length - ms.Position)), true);
                }
            }
            else
            {
                string flag = script_list[scr_index];
                byte flag2 = mbr.ReadByte();
                switch (flag)
                {
                    case "MESSAGE":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16,Type.PString);
                        break;
                    case "SELECT":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        break;
                    case "LOG":
                        retn += " " + DeCompressFunc(ref mbr, Type.Script, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        break;
                    case "IFN":
                        retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.StringSJIS, Type.UInt32);
                        break;
                    case "TASK":
                        if (flag2 == 0x03)
                        {
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16);
                            UInt16 tmp = mbr.ReadUInt16();
                            retn += " " + tmp.ToString();
                            if (tmp == 1)
                                retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.UInt16, Type.PString);
                        }
                        if (flag2 == 0x01)
                        {
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.UInt16);
                        }

                        break;
                    case "FARCALL":
                        if (flag2 == 0x00)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16,  Type.Byte,  Type.StringSJIS);
                        if (flag2 == 0x01)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16, Type.StringSJIS);
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
                        if (flag2 == 0x00)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt16);
                        if (flag2 == 0x01)
                            retn += " " + DeCompressFunc(ref mbr, Type.UInt16, Type.UInt32);
                        break;
                    default:
                        break;

                }
                while (ms.Position + 1 < ms.Length)
                {
                    retn += " [" + Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
                }
                if (ms.Position < ms.Length)
                {
                    retn += " [" + mbr.ReadByte().ToString("X2") + "]";
                }
                retn = flag + " " + "[" + flag2.ToString("X2") + "]" + retn;
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
            
            
            if (retn != "    " && retn != "" && !rec)
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
        Dictionary<byte, string> script_list = new Dictionary<byte, string>
        {
            {0x00,"EQU"                           },
            {0x01,"EQUN"                          },
            {0x02,"EQUV"                          },
            {0x03,"ADD"                           },
            {0x04,"SUB"                           },
            {0x05,"MUL"                           },
            {0x06,"DIV"                           },
            {0x07,"MOD"                           },
            {0x08,"AND"                           },
            {0x09,"OR"                            },
            {0x0A,"RANDOM"                        },
            {0x0B,"VARSTR"                        },
            {0x0C,"VARSTR_ADD"                    },
            {0x0D,"SET"                           },
            {0x0E,"FLAGCLR"                       },
            {0x0F,"GOTO"                          },
            {0x10,"ONGOTO"                        },
            {0x11,"GOSUB"                         },
            {0x12,"IFY"                           },
            {0x13,"IFN"                           },
            {0x14,"RETURN"                        },
            {0x15,"JUMP"                          },
            {0x16,"FARCALL"                       },
            {0x17,"FARRETURN"                     },
            {0x18,"JUMPPOINT"                     },
            {0x19,"END"                           },
            {0x1A,"VARSTR_SET"                    },
            {0x1B,"VARSTR_ALLOC"                  },
            {0x1C,"TALKNAME_SET"                  },
            {0x1D,"ARFLAGSET"                     },
            {0x1E,"COLORBG_SET"                   },
            {0x1F,"SPLINE_SET"                    },
            {0x20,"SHAKELIST_SET"                 },
            {0x21,"SCISSOR_TRIANGLELIST_SET"      },
            {0x22,"MESSAGE"                       },//Message Add
            {0x23,"MESSAGE_CLEAR"                 },
            {0x24,"MESSAGE_WAIT"                  },
            {0x25,"SELECT"                        },
            {0x26,"CLOSE_WINDOW"                  },
            {0x27,"LOG"                           },
            {0x28,"LOG_PAUSE"                     },
            {0x29,"LOG_END"                       },
            {0x2A,"VOICE"                         },
            {0x2B,"WAIT_COUNT"                    },
            {0x2C,"WAIT_TIME"                     },
            {0x2D,"WAIT_TEXTFEED"                 },
            {0x2E,"FFSTOP"                        },
            {0x2F,"INIT"                          },
            {0x30,"STOP"                          },
            {0x31,"IMAGELOAD"                     },
            {0x32,"IMAGEUPADTE"                   },
            {0x33,"ARC"                           },
            {0x34,"MOVE"                          },
            {0x35,"MOVE2"                         },
            {0x36,"ROT"                           },
            {0x37,"PEND"                          },
            {0x38,"FADE"                          },
            {0x39,"SCALE"                         },
            {0x3A,"SHAKE"                         },
            {0x3B,"SHAKELIST"                     },
            {0x3C,"BASE"                          },
            {0x3D,"MCMOVE"                        },
            {0x3E,"MCARC"                         },
            {0x3F,"MCROT"                         },
            {0x40,"MCSHAKE"                       },
            {0x41,"MCFADE"                        },
            {0x42,"WAIT"                          },
            {0x43,"DRAW"                          },
            {0x44,"WIPE"                          },
            {0x45,"FRAMEON"                       },
            {0x46,"FRAMEOFF"                      },
            {0x47,"FW"                            },
            {0x48,"SCISSOR"                       },
            {0x49,"DELAY"                         },
            {0x4A,"RASTER"                        },
            {0x4B,"TONE"                          },
            {0x4C,"SCALECOSSIN"                   },
            {0x4D,"BMODE"                         },
            {0x4E,"SIZE"                          },
            {0x4F,"SPLINE"                        },
            {0x50,"DISP"                          },
            {0x51,"MASK"                          },
            {0x52,"FACE"                          },
            {0x53,"SEPIA"                         },
            {0x54,"SEPIA_COLOR"                   },
            {0x55,"CUSTOMMOVE"                    },
            {0x56,"SWAP"                          },
            {0x57,"ADDCOLOR"                      },
            {0x58,"SUBCOLOR"                      },
            {0x59,"SATURATION"                    },
            {0x5A,"PRIORITY"                      },
            {0x5B,"UVWH"                          },
            {0x5C,"EVSCROLL"                      },
            {0x5D,"COLORLEVEL"                    },
            {0x5E,"QUAKE"                         },
            {0x5F,"BGM"                           },
            {0x60,"BGM_WAITSTART"                 },
            {0x61,"BGM_WAITFADE"                  },
            {0x62,"BGM_PUSH"                      },
            {0x63,"BGM_POP"                       },
            {0x64,"SE"                            },
            {0x65,"SE_STOP"                       },
            {0x66,"SE_WAIT"                       },
            {0x67,"SE_WAIT_COUNT"                 },
            {0x68,"VOLUME"                        },
            {0x69,"MOVIE"                         },
            {0x6A,"SETCGFLAG"                     },
            {0x6B,"EX"                            },
            {0x6C,"TROPHY"                        },
            {0x6D,"SETBGMFLAG"                    },
            {0x6E,"TASK"                          },
            {0x6F,"PRINTF"                        },
            {0x70,"WAIT_FADE"                     },
            {0x71,"MYSCALE"                       },
            {0x72,"MYSCALE_CLEAR"                 },
            {0x73,"ENROLL_WAIT"                   },
            {0x74,"ENROLL_BGSTART"                },
            {0x75,"ENROLL_FRAMEENABLE"            },
            {0x76,"DATEEYECATCH"                  },
            {0x77,"MAPSELECT"                     },
            {0x78,"UNKNOWN"                       }
        };
        public byte[] Hex2Byte(string hexString)// 字符串转16进制字节数组
        {
            
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public string Byte2Hex(byte[] bytes)// 字节数组转16进制字符串
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }

}
