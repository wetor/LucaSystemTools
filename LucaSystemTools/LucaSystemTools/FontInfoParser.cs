/*
 Prot引擎.字库部分
 info文件解析
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LucaSystem;
using System.ComponentModel;

namespace ProtFont
{
    class FontInfoParser : AbstractFileParser
    {
        //info 字库Map构成
        //是用来映射unicode和图片index用的

        //1.Head 4 * 2字节 或者 3 * 2字节
        //fontsize 2字节
        //fontsize2 2字节
        //unk1 2字节//值为100 暂时不明有些没这个 有些没这个
        //fontcount 2字节

        //2.渲染用 渲染字宽 渲染坐标等信息
        //fontcount * 3字节

        //3.unicode 图片顺序映射
        //256x256x2字节
        //256x256 的值为表达unicode
        //读取的2字节为在图片的映射顺序

        //4.大概是表示size的
        //256x256x2字节
        //256x256 的值为表达unicode
        //读取的2字节为size(值和2类似)

        // Encoding.Unicode就是utf - 16le
        // Encoding.BigEndianUnicode是utf - 16be

        //作者：DeQxJ00
        //时间：2019.1.17
        public override void FileExport(string file, string outpath = null)
        {
            
            string name = file;
            StringBuilder sb = new StringBuilder();
            var fs = File.OpenRead(name);
            var fs_ = File.OpenRead(name);

            int totallen = (int)fs.Length;
            fs.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[2];
            fs.Read(bytes, 0, 2);
            int fontsize = BitConverter.ToUInt16(bytes, 0);
            sb.AppendLine(fontsize.ToString());
            fs.Read(bytes, 0, 2);
            int fontsize2 = BitConverter.ToUInt16(bytes, 0);
            sb.AppendLine(fontsize2.ToString());

            //有些head有3个2字节 有些是4个
            fs.Read(bytes, 0, 2);
            int unk1 = BitConverter.ToUInt16(bytes, 0);
            int fontcount = 0;
            if (unk1 == 100)
            {
                sb.AppendLine(unk1.ToString());
                fs.Read(bytes, 0, 2);
                fontcount = BitConverter.ToUInt16(bytes, 0);
            }
            else
            {
                fontcount = unk1;
                unk1 = 0;
            }
            sb.AppendLine(fontcount.ToString());


            Dictionary<uint, string> listWithUnkown = new Dictionary<uint, string>(); //
            bytes = new byte[3];
            for (int i = 0; i < fontcount; i++)
            {
                fs.Read(bytes, 0, 3);
                var str = BitConverter.ToString(bytes).Replace("-", string.Empty);
                sb.AppendLine(str);
                listWithUnkown.Add((uint)i, str);
            }

            int pos = (int)fs.Position;
            sb.AppendLine("===========");
            sb.AppendLine("Position:" + pos.ToString());
            sb.AppendLine("Totallen:" + totallen.ToString());
            sb.AppendLine("Totallen-Position:" + (totallen - pos).ToString());
            sb.AppendLine("===========");

            //fs.Seek(pos, SeekOrigin.Begin);
            Dictionary<uint, string> listStrUnicodeHex = new Dictionary<uint, string>(); //
            Dictionary<uint, string> listStrUnicode = new Dictionary<uint, string>(); //
            Dictionary<uint, string> listSize = new Dictionary<uint, string>();

            //后半部分表示size的
            int pos2 = (int)fs.Position + 256 * 256 * 2;
            fs_.Seek(pos2, SeekOrigin.Begin);


            //前半部分 unicode
            int countx = 0;
            int county = 0;

            bytes = new byte[2];
            for (int i = 0; i < 256 * 256; i++)
            {
                fs.Read(bytes, 0, 2);
                uint index = BitConverter.ToUInt16(bytes, 0);

                fs_.Read(bytes, 0, 2);
                //uint size = BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0);
                var bytesSize = BitConverter.ToString(bytes).Replace("-", string.Empty);

                //unicode
                byte[] bytesUnicode = new byte[2] { (byte)countx, (byte)county };
                string stringUnicodeHex = BitConverter.ToString(bytesUnicode).Replace("-", string.Empty);
                string stringUnicodeStr = Encoding.Unicode.GetString(bytesUnicode);
                if (!listStrUnicodeHex.ContainsKey(index))
                {
                    listStrUnicodeHex.Add(index, stringUnicodeHex);
                    listStrUnicode.Add(index, stringUnicodeStr);
                    listSize.Add(index, bytesSize);
                }

                countx++;
                if (countx == 256)
                {
                    countx = 0;
                    county++;
                }
            }

            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();

            //未排序
            //sb2.Clear();
            //sb2.AppendLine($"-totallen={totallen}\t-fontsize={fontsize}\t-fontsize2={fontsize2}\t-unk1={unk1}\t-fontcount={fontcount}");
            //sb2.AppendLine("Index\tString\tUnicode\tSize\tSize2");
            //foreach (var keyValuePair in listStrUnicode)
            //{
            //    sb2.AppendLine(keyValuePair.Key.ToString("D4") + "\t" +
            //                   keyValuePair.Value + "\t" +
            //                   listStrUnicodeHex[keyValuePair.Key] + "\t" +
            //                   listSize[keyValuePair.Key] + "\t" +
            //                   listWithUnkown[keyValuePair.Key]);

            //}

            //File.WriteAllText(name + "_dicStr.txt", sb2.ToString(), Encoding.Unicode);
            //sb2.Clear();


            //已排序
            //sb2.Clear();
            //sb2.AppendLine($"-totallen={totallen}\t-fontsize={fontsize}\t-fontsize2={fontsize2}\t-unk1={unk1}\t-fontcount={fontcount}");
            //sb2.AppendLine("Index\tString\tUnicode\tSize\tSize2");
            //foreach (var keyValuePair in listStrUnicode.OrderBy(x => x.Key))
            //{
            //    sb2.AppendLine(keyValuePair.Key.ToString("D4") + "\t" +
            //                   keyValuePair.Value + "\t" +
            //                   listStrUnicodeHex[keyValuePair.Key] + "\t" +
            //                   listSize[keyValuePair.Key] + "\t" +
            //                   listWithUnkown[keyValuePair.Key]);
            //    sb3.Append(keyValuePair.Value);
            //}
            //File.WriteAllText(name + "_dicStr_sort.txt", sb2.ToString(), Encoding.Unicode);
            //File.WriteAllText(name + "_string_utf-8.txt", sb3.ToString(), Encoding.UTF8);

            sb2.Clear();
            sb3.Clear();
            sb2.AppendLine($"-totallen={totallen}\t-fontsize={fontsize}\t-fontsize2={fontsize2}\t-unk1={unk1}\t-fontcount={fontcount}");
            sb2.AppendLine("Index\tString\tUnicode\tSize\tSize2");
            for (uint i=0;i<fontcount;i++)
            {
                string unicodestr = " ";
                string unicodehex = "0000";
                string sizehex = "0000";
                if (listStrUnicode.ContainsKey(i))
                {
                    unicodestr = listStrUnicode[i];
                }
                if (listStrUnicodeHex.ContainsKey(i))
                {
                    unicodehex = listStrUnicodeHex[i];
                }
                if (listSize.ContainsKey(i))
                {
                    sizehex = listSize[i];
                }
                sb2.AppendLine(i.ToString("D4") + "\t" +
                               unicodestr + "\t" +
                               unicodehex + "\t" +
                               sizehex + "\t" +
                               listWithUnkown[i]);
                if (i == 0)
                {
                    sb3.Append(" ");
                }
                else
                {
                    sb3.Append(unicodestr);
                }
              
            }
            File.WriteAllText(name + "_dicStr_sort2.txt", sb2.ToString(), Encoding.Unicode);
            File.WriteAllText(name + "_string_sort_utf-8.txt", sb3.ToString(), Encoding.UTF8);
            sb2.Clear();

            File.WriteAllText(name + "_", sb.ToString());
        }

      

        //https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        /// <summary>
        /// 1.读入文件需要使用小端Unicode保存
        /// 2.读取使用1,3,4,5列 第2列不读留空 
        /// 3./t为分隔符
        /// 4.第1行读 fontsize fontsize2 unk1 其他不读
        /// 5.第2行不读
        /// </summary>
        /// <param name="name"></param>
        public override void FileImport(string name, string outpath = null)
        {
           
            UInt16[,] listStrUnicodeHex = new UInt16[256, 256];
            string[,] listSize = new string[256, 256];
            for (UInt16 x = 0; x < 256; x++)
            {
                for (UInt16 y = 0; y < 256; y++)
                {
                    listStrUnicodeHex[x, y] = 0;
                    listSize[x, y] = "0000";
                }
            }

            Dictionary<UInt16, byte[]> listSize2 = new Dictionary<UInt16, byte[]>();
            int linecount = 0;

            UInt16 fontsize = 0;
            UInt16 fontsize2 = 0;
            UInt16 unk1 = 0;
            UInt16 fontcount = 0;

            var ie = File.ReadLines(name, Encoding.Unicode).GetEnumerator();
            while (ie.MoveNext())
            {
                string line = ie.Current;
                if (linecount == 0)
                {
                    string[] tmp = line.Split('\t');
                    fontsize = UInt16.Parse(tmp[1].Split('=')[1]);
                    fontsize2 = UInt16.Parse(tmp[2].Split('=')[1]);
                    unk1 = UInt16.Parse(tmp[3].Split('=')[1]);
                    linecount++;
                    continue;
                }
                if (linecount == 1)
                {
                    linecount++;
                    continue;
                }
                linecount++;
                if (!string.IsNullOrEmpty(line))
                {
                    string[] tmp = line.Split('\t');
                    UInt16 index = UInt16.Parse(tmp[0]);
                    byte[] unicodeHex = StringToByteArray(tmp[2]);
                    //byte[] size = StringToByteArray(tmp[3]);
                    byte[] size2 = StringToByteArray(tmp[4]);

                   
                    if(unicodeHex.SequenceEqual(new byte[2]))
                    {
                        if (index == 0)
                        {
                            listStrUnicodeHex[unicodeHex[0], unicodeHex[1]] = index;
                        }
                        else
                        {
                            listStrUnicodeHex[unicodeHex[0], unicodeHex[1]] = 0;
                            Console.WriteLine("0000空白Unicode,不写入index:" + unicodeHex[0] + "," + unicodeHex[1] + "," + index);
                        }
                     
                    }
                    else
                    {
                        listStrUnicodeHex[unicodeHex[0], unicodeHex[1]] = index;
                    }
                    listSize[unicodeHex[0], unicodeHex[1]] = tmp[3];
                    if (!listSize2.ContainsKey(index))
                    {
                        listSize2.Add(index, size2);
                    }
                    else
                    {
                        Console.WriteLine("重复Unicode:" + line);
                    }

                    if(index > fontcount)
                    {
                        fontcount = index;
                    }
                }
            }

            fontcount++;

            //info 字库Map构成
            //是用来映射unicode和图片index用的

            //1.Head 4 * 2字节 或者 3 * 2字节
            //fontsize 2字节
            //fontsize2 2字节
            //unk1 2字节//值为100 暂时不明有些没这个 有些没这个
            //fontcount 2字节
            var ms = new MemoryStream();
            var bytes = BitConverter.GetBytes(fontsize);
            ms.Write(bytes,0,bytes.Length);
            bytes = BitConverter.GetBytes(fontsize2);
            ms.Write(bytes, 0, bytes.Length);
            if (unk1 == 0)
            {
              
            }
            else
            {
                bytes = BitConverter.GetBytes(unk1);
                ms.Write(bytes, 0, bytes.Length);
            }
            bytes = BitConverter.GetBytes(fontcount);
            ms.Write(bytes, 0, bytes.Length);

            //2.渲染用 渲染字宽 渲染坐标等信息
            //fontcount * 3字节
            for (UInt16 i = 0; i < fontcount; i++)
            {
                if (listSize2.ContainsKey(i))
                {
                    ms.Write(listSize2[i], 0, listSize2[i].Length);
                }
                else
                {
                    Console.WriteLine("以00 00 00代替,未填入Index的Size2:" + i);
                    ms.Write(new byte[3], 0,3);
                }
               
            }

            //3.unicode 图片顺序映射
            //256x256x2字节
            //256x256 的值为表达unicode
            //读取的2字节为在图片的映射顺序
            int countx = 0;
            int county = 0;
            for (UInt16 i = 0; i < 256 * 256; i++)
            {
                //unicode
                bytes = BitConverter.GetBytes(listStrUnicodeHex[countx, county]);
                ms.Write(bytes, 0, bytes.Length);

                countx++;
                if (countx == 256)
                {
                    countx = 0;
                    county++;
                }
                if (county == 256)
                {
                    break;
                }
            }

            //4.大概是表示size的
            //256x256x2字节
            //256x256 的值为表达unicode
            //读取的2字节为size(值和2类似)
            countx = 0;
            county = 0;
            for (UInt16 i = 0; i < 256 * 256; i++)
            {
                //unicode
                //byte[] bytesUnicode = new byte[2] { (byte)countx, (byte)county };
                bytes = StringToByteArray(listSize[countx, county]);
                ms.Write(bytes, 0, bytes.Length);

                countx++;
                if (countx == 256)
                {
                    countx = 0;
                    county++;
                }
                if (county == 256)
                {
                    break;
                }
            }

            File.WriteAllBytes(name + ".info", ms.ToArray());
        }
    }
}
