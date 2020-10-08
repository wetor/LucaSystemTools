using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using LucaSystem;
using LucaSystemTools;
using System.Linq;

namespace ProtImage
{
    public class CZ1_4bitParser : CZ1Parser
    {
        public byte color256to16(byte b)
        {
            int index = 0;
            byte min = 0xFF;
            for (int i = 0; i < list.Count; i++)
            {
                byte tmp=(byte)Math.Abs(b - list[i]);
                if (tmp < min)
                {
                    min = tmp;
                    index = i;
                }
            }
            return list[index];
        }


        List<byte> list = new List<byte>();
        //作者：Wetor
        //时间：2019.1.18
        public void PngToCZ1(string outfile)
        {
            Bitmap Picture = new Bitmap(File.Open(outfile, FileMode.Open));
            StructWriter Writer = new StructWriter(File.Open(outfile + ".cz1", FileMode.Create));
            CZ1Header header;
            header.Signature = "CZ1";
            header.HeaderLength = 0x10;
            header.Width = (ushort)Picture.Width;
            header.Heigth = (ushort)Picture.Height;
            header.Colorbits = 4;
            Writer.WriteStruct(ref header);
            Writer.Seek(header.HeaderLength, SeekOrigin.Begin);


            Pixel32 Pixel = new Pixel32();
            Pixel.R = 255;
            Pixel.G = 255;
            Pixel.B = 255;
            //FF FF FF 00
            //FF FF FF 18
            //FF FF FF 26
            //FF FF FF 35
            //FF FF FF 45
            //FF FF FF 55
            //FF FF FF 65
            //FF FF FF 75
            //FF FF FF 85
            //FF FF FF 96
            //FF FF FF A7
            //FF FF FF B8
            //FF FF FF C9
            //FF FF FF DB
            //FF FF FF EC
            //FF FF FF FF
          

            Pixel.A = (byte)0x00; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x18; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x26; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x35; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x45; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x55; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x65; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x75; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x85; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x96; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xA7; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xB8; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xC9; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xDB; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xEC; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xFF; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Queue<byte> queue = new Queue<byte>();
            int i = 0;
            for (int y = 0; y < Picture.Height; y++)
            {
                for (int x = 0; x < Picture.Width; x++)
                {
                    byte tmp = color256to16(Picture.GetPixel(x, y).A);
                    //bytes11[i] = (byte)list.IndexOf(tmp);
                    queue.Enqueue((byte)list.IndexOf(tmp));
                    i++;
                }
            }

            byte[] bytes2 = new byte[Picture.Height * Picture.Width/2];
           
            for (int j = 0; j < bytes2.Length; j++)
            {
                var low4bit = queue.Dequeue();
                var high4bit = queue.Dequeue();
                var b =  (uint)(high4bit << 4)+ (uint)low4bit;
                bytes2[j] = (byte)b;
            }
            //foreach (var b in bytes2)
            //{
            //    int low4bit = b & 0x0F;
            //    int high4bit = (b & 0xF0) >> 4;
            //    queue.Enqueue(low4bit);
            //    queue.Enqueue(high4bit);
            //}

            int file_num = bytes2.Length / 130554 + 1;
            //System.Diagnostics.Debug.WriteLine("{0} {1} {2}", file_num, listBytes.Count, 130554);
            List<int[]> out_list = new List<int[]>();
            Writer.Write(file_num);
            for (int k = 0; k < file_num; k++)
            {
                List<int> listBytes = new List<int>();
                StringBuilder decompressed = new StringBuilder();
                byte[] tmp_bytes = new byte[130554];
                if (k == file_num - 1)
                    Array.Copy(bytes2, k * 130554, tmp_bytes, 0, bytes2.Length - k * 130554);
                else
                    Array.Copy(bytes2, k * 130554, tmp_bytes, 0, 130554);


                foreach (char kk in tmp_bytes)
                {
                    decompressed.Append(kk);
                }
                listBytes = LzwUtil.Compress(decompressed.ToString());
                out_list.Add(listBytes.ToArray());
                Writer.Write(listBytes.Count);
                //string tmp_str;
                System.Diagnostics.Debug.WriteLine("{0}", k);
                /*if (k== file_num - 1)
                {
                    tmp_list.AddRange(listBytes.GetRange(130554 / 2 * k, listBytes.Count - 130554 / 2 * k));
                    tmp_list.Insert(0, 0);
                    tmp_str = Decompress(tmp_list);
                }
                else
                {
                    tmp_list.AddRange(listBytes.GetRange(130554 / 2 * k, 130554 / 2));
                    tmp_list.Insert(0, 0);
                    tmp_str = Decompress(tmp_list);
                }*/
                if (k == file_num - 1)
                {
                    Writer.Write(bytes2.Length - k * 130554);
                }
                else
                {
                    Writer.Write(130554);
                }
                //Writer.Write(0xFFFD);

            }
            /*List<byte> output = new List<byte>();
            string str = Decompress(listBytes);
            foreach (var c in str)
            {
                output.Add((byte)c);
            }
           // Writer.Write(output.ToArray());
            System.Diagnostics.Debug.WriteLine(output.Count);*/

            for (int k = 0; k < out_list.Count; k++)
            {
                for (int kk = 0; kk < out_list[k].Length; kk++)
                {
                    Writer.Write((UInt16)out_list[k][kk]);
                }

            }
            Writer.Close();


        }

        public override void FileExport(string name, string outpath = null)
        {
            CZ1ToPng(name);
        }

        public override void FileImport(string name, string outpath = null)
        {
            PngToCZ1(name);
        }

    }



  




}
