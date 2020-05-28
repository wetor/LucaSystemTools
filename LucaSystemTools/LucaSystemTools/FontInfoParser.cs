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
        //作者：DeQxJ00
        //时间：2019.1.17
        public void Export(string file)
        {
            //解释info12 之类的文件
            string name = file;
            StringBuilder sb = new StringBuilder();
            var fs = File.OpenRead(name);
            var fs_ = File.OpenRead(name);
            //info 文件
            int totallen = (int)fs.Length;
            fs.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[2];
            fs.Read(bytes, 0, 2);
            int fontsize = BitConverter.ToUInt16(bytes, 0);
            sb.AppendLine(fontsize.ToString());
            fs.Read(bytes, 0, 2);
            int fontsize2 = BitConverter.ToUInt16(bytes, 0);
            sb.AppendLine(fontsize2.ToString());
            fs.Seek(2, SeekOrigin.Current);
            fs.Read(bytes, 0, 2);
            int fontcount = BitConverter.ToUInt16(bytes, 0);
            sb.AppendLine(fontcount.ToString());
            // bytes = new byte[1];
            // fs.Read(bytes, 0, 1);
            //int fontunk1 = Convert.ToInt16(bytes[0]);
            // sb.AppendLine(fontunk1.ToString());
            //fs.Read(bytes, 0, 1);
            //int fontunk2 = Convert.ToInt16(bytes[0]);
            //sb.AppendLine(fontunk2.ToString());
            bytes = new byte[3];
            for (int i = 0; i < fontcount; i++)
            {
                fs.Read(bytes, 0, 3);
                foreach (var chr in bytes)
                {
                    sb.Append(String.Format("{0:X2}", Convert.ToInt32(chr)));
                    sb.Append(" ");
                }
                sb.Append("\r\n");
            }

            int pos = (int)fs.Position;
            sb.AppendLine("===========");
            sb.AppendLine("Position:" + pos.ToString());
            sb.AppendLine("Totallen:" + totallen.ToString());
            sb.AppendLine("Totallen-Position:" + (totallen - pos).ToString());
            sb.AppendLine("===========");


            //bytes = new byte[2 * 256 * 256];
            //fs.Read(bytes, 0, bytes.Length);
            //File.WriteAllBytes("fonts-utf16le-jp-trans.bin", bytes);

            //fs.Seek(pos, SeekOrigin.Begin);

            Dictionary<uint, string> listStr = new Dictionary<uint, string>();//
            Dictionary<uint, int> listSize = new Dictionary<uint, int>();
            listStr.Add(0, " ");
            listSize.Add(0, 0);
            //262144


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
                uint size = BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0);

                //unicode
                //var s = Encoding.UTF8.GetString(new byte[2] { (byte)countx, (byte)county });
                var s = Encoding.Unicode.GetString(new byte[2] { (byte)countx, (byte)county });
                if (index != 0 && !listStr.ContainsKey(index))
                {

                    listStr.Add(index, s);
                    listSize.Add(index, (int)size);
                }

                //换行
                countx++;
                if (countx == 256)
                {
                    countx = 0;
                    county++;
                }
            }

            StringBuilder sb2 = new StringBuilder();

            //未排序
            sb2.Clear();
            foreach (var keyValuePair in listStr)
            {
                sb2.AppendLine(keyValuePair.Key.ToString("x4") + "," + keyValuePair.Key.ToString("D4") + "," + keyValuePair.Value);
            }
            File.WriteAllText(name + "_dicStr.txt", sb2.ToString(), Encoding.Unicode);
            sb2.Clear();
            foreach (var keyValuePair in listSize)
            {
                sb2.AppendLine(keyValuePair.Key.ToString("x4") + "," + keyValuePair.Key.ToString("D4") + "," + keyValuePair.Value);
            }
            File.WriteAllText(name + "_dicSize.txt", sb2.ToString(), Encoding.Unicode);


            //已排序
            sb2.Clear();
            foreach (var keyValuePair in listStr.OrderBy(x => x.Key))
            {
                sb2.AppendLine(keyValuePair.Key.ToString("x4") + "," + keyValuePair.Key.ToString("D4") + "," + keyValuePair.Value);
            }
            File.WriteAllText(name + "_dicStr_sort.txt", sb2.ToString(), Encoding.Unicode);
            sb2.Clear();
            foreach (var keyValuePair in listSize.OrderBy(x => x.Key))
            {
                sb2.AppendLine(keyValuePair.Key.ToString("x4") + "," + keyValuePair.Key.ToString("D4") + "," + keyValuePair.Value);
            }
            File.WriteAllText(name + "_dicSize_sort.txt", sb2.ToString(), Encoding.Unicode);



            File.WriteAllText(name + "_", sb.ToString());
        }
    }
}
