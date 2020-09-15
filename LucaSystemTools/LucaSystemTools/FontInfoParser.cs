/*
 Prot引擎.字库部分
 info文件解析
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProtFont
{
    class FontInfoParser
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
        public void Export(string file)
        {
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
                //fs.Read(bytes, 0, 2);
                //int unk1 = BitConverter.ToUInt16(bytes, 0);
                //sb.AppendLine(unk1.ToString());

                fs.Read(bytes, 0, 2);
                int fontcount = BitConverter.ToUInt16(bytes, 0);
                sb.AppendLine(fontcount.ToString());

         
                Dictionary<uint, string> listWithUnkown = new Dictionary<uint, string>();//
                bytes = new byte[3];
                for (int i = 0; i < fontcount; i++)
                {
                    fs.Read(bytes, 0, 3);
                    StringBuilder sb1 =new StringBuilder();
                    foreach (var chr in bytes)
                    {
                        sb.Append(String.Format("{0:X2}", Convert.ToInt32(chr)));
                        sb.Append(" ");
                        sb1.Append(String.Format("{0:X2}", Convert.ToInt32(chr)));
                        sb1.Append(" ");
                    }
                    listWithUnkown.Add((uint)i, sb1.ToString());
                    sb.Append("\r\n");
                }

                int pos = (int)fs.Position;
                sb.AppendLine("===========");
                sb.AppendLine("Position:"+pos.ToString());
                sb.AppendLine("Totallen:"+totallen.ToString());
                sb.AppendLine("Totallen-Position:" + (totallen-pos).ToString());
                sb.AppendLine("===========");

                //fs.Seek(pos, SeekOrigin.Begin);
                Dictionary<uint, string> listStrUnicodeHex = new Dictionary<uint, string>();//
                Dictionary<uint,string> listStrUnicode= new Dictionary<uint, string>();//
                Dictionary<uint, string> listSize = new Dictionary<uint, string>();

                //后半部分表示size的
                int pos2 = (int)fs.Position+256*256*2;
                fs_.Seek(pos2, SeekOrigin.Begin);

                
                //前半部分 unicode
                int countx = 0;
                int county = 0;
               
                bytes = new byte[2];
                for (int i = 0; i < 256*256 ; i++)
                {
                    fs.Read(bytes, 0, 2);
                    uint index = BitConverter.ToUInt16(bytes, 0);

                    fs_.Read(bytes, 0, 2);
                    //uint size = BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0);
                    var bytesSize = BitConverter.ToString(bytes).Replace("-", string.Empty);

                   //unicode
                    byte[] bytesUnicode = new byte[2] {(byte) countx, (byte) county};
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

                StringBuilder sb2=new StringBuilder();
            
                //未排序
                sb2.Clear();
                sb2.AppendLine("Index\tString\tUnicode\tSize\tSize2");
                foreach (var keyValuePair in listStrUnicode)
                {
                    sb2.AppendLine(keyValuePair.Key.ToString("D4") + "\t" +
                                   keyValuePair.Value + "\t" +
                                   listStrUnicodeHex[keyValuePair.Key] + "\t" + 
                                   listSize[keyValuePair.Key] + "\t" +
                                   listWithUnkown[keyValuePair.Key]);
                }
                File.WriteAllText(name + "_dicStr.txt", sb2.ToString(), Encoding.Unicode);
                    sb2.Clear();
               

                //已排序
                sb2.Clear();
                sb2.AppendLine("Index\tString\tUnicode\tSize\tSize2");
                foreach (var keyValuePair in listStrUnicode.OrderBy(x=>x.Key))
                {
                    sb2.AppendLine(keyValuePair.Key.ToString("D4") + "\t" +
                                   keyValuePair.Value + "\t" +
                                   listStrUnicodeHex[keyValuePair.Key] + "\t" +
                                   listSize[keyValuePair.Key] + "\t" +
                                   listWithUnkown[keyValuePair.Key]);
                }
                File.WriteAllText(name+ "_dicStr_sort.txt", sb2.ToString(), Encoding.Unicode);
                sb2.Clear();

            }

            File.WriteAllText(name+"_",sb.ToString());

        }
    }
}
