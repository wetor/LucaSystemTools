using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LucaSystem;
using LucaSystemTools;

namespace RealLive
{
    public class PsbScript:AbstractFileParser
    {
        private FileStream fs;
        private BinaryReader br;
     
        public PsbScript()
        {
   

        }
        public void Close()
        {
            br.Close();
            fs.Close();
        }
        private bool CanRead()
        {
            return fs.CanRead && fs.Position < fs.Length;
        }
        public void DeCompress(string path)
        {
            fs.Seek(0x10, SeekOrigin.Begin);//head
            fs.Seek(0x400, SeekOrigin.Current);//null
            fs.Seek(0xA, SeekOrigin.Current);//unknow
            if (!CanRead()) return;

            FileStream tfs = new FileStream(path + ".txt", FileMode.Create);
            StreamWriter tsw = new StreamWriter(tfs, Encoding.UTF8);
            while (fs.Position < fs.Length)
            {
                byte[] tmp = ReadCodeBytes();
                tsw.WriteLine(DeCompressCode(tmp));
            }
            tsw.Close();
            tfs.Close();
        }
        private byte[] ReadCodeBytes()
        {
            if (!CanRead()) return new byte[0];
            List<byte> datas = new List<byte>();
            UInt16 tmp = br.ReadUInt16();
            byte len = 0;
            if (tmp == 240 || tmp == 241) //F000||F100
            {
                datas.AddRange(BitConverter.GetBytes(tmp));
                len = br.ReadByte();
                datas.Add(len);
                datas.AddRange(br.ReadBytes((int)len));
            }
            byte btmp;
            while (true)  //0xF000
            {
                btmp = br.ReadByte();
                if ((btmp == 0xF0 || btmp ==0xF1) && fs.Position + 1 < fs.Length)
                {
                    if (br.ReadByte() == 0x00)
                    {
                        fs.Seek(-2, SeekOrigin.Current);
                        break;
                    }
                    else
                        fs.Seek(-1, SeekOrigin.Current);
                }
                datas.Add(btmp);
                if (!CanRead())
                {
                    break;
                }

            }
            if (Program.debug)
                Console.WriteLine("{0}  {1}", fs.Position, (int)len);
            //datas.AddRange(BitConverter.GetBytes(len));
            return datas.ToArray();

        }
        private string DeCompressCode(byte[] line, bool rec = false)
        {
            if (line.Length < 3)
                return "[" + Byte2Hex(line) + "]";

            // len      flag    code        other
            // 1byte    2byte   (len-2)byte 余下
            MemoryStream ms = new MemoryStream(line);
            BinaryReader mbr = new BinaryReader(ms);
            UInt16 type = mbr.ReadUInt16();
            string retn = "[" + Byte2Hex(BitConverter.GetBytes(type)) + "] ";
            int len = (int)mbr.ReadByte();
            if (len >= 2)
            {
                UInt16 flag = mbr.ReadUInt16();
                len -= 2;
                switch (flag)
                {
                    case 1:
                        retn += "Func" + " [" + Byte2Hex(mbr.ReadByte()) + "]" + " [" + Byte2Hex(mbr.ReadBytes(len - 1)) + "]";
                        break;
                    case 2:
                        int _len = (int)mbr.ReadByte();
                        retn += "Text" + " s\"" + Encoding.GetEncoding("Shift-Jis").GetString(mbr.ReadBytes(_len)) + "\"";
                        break;
                    default:
                        retn += "[" + flag + "]";
                        break;
                }
            }
            
            while (ms.Position + 1 < ms.Length)
            {
                retn += " [" + Byte2Hex(BitConverter.GetBytes(mbr.ReadUInt16())) + "]";
            }
            if (ms.Position < ms.Length)
            {
                retn += " [" + Byte2Hex(mbr.ReadByte()) + "]";
            }

            if (retn != "    " && retn != "" && !rec && Program.debug)
            {
                Console.WriteLine(retn);
            }

            mbr.Close();
            ms.Close();
            return retn;
        }
        //unUse
        private Dictionary<byte[], string> decompress_dic = new Dictionary<byte[], string>
        {
            { new byte[2]{0xF0,0x00}, "END" }
        };
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
                    if (space && i < bytes.Length - 1) returnStr += " ";
                }
            }
            return (head ? "0x" : "") + returnStr;
        }
        private string Byte2Hex(byte bytes)// 字节数组转16进制字符串
        {
            return bytes.ToString("X2");
        }

        public override void FileExport(string name, string outpath = null)
        {
            fs = new FileStream(name, FileMode.Open);
            br = new BinaryReader(fs);
            DeCompress(name);
        }

        public override void FileImport(string name, string outpath = null)
        {
            //Compress();
        }
    }
}
