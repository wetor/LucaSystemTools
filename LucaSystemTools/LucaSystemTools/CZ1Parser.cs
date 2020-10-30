using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using LucaSystem;
using LucaSystemTools;

namespace ProtImage
{
    public class CZ1Parser : CZParserBase
    {
        public Bitmap Export(byte[] Texture,string name="")
        {
            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ1Header Header = new CZ1Header();
            Reader.ReadStruct(ref Header);

            if (Header.Signature != "CZ1\x0")
                throw new BadImageFormatException();

            Reader.Seek(Header.HeaderLength, SeekOrigin.Begin);
            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);

            if (Header.Colorbits == 4)//4bit
            {
                //字库格式
                //System.Diagnostics.Debug.WriteLine(4);
                //表
                Pixel32_BGRA[] ColorPanel = new Pixel32_BGRA[16];
                Pixel32_BGRA Pixel = new Pixel32_BGRA();
                for (int i = 0; i < ColorPanel.Length; i++)
                {
                    Reader.ReadStruct(ref Pixel);
                    ColorPanel[i] = Pixel;
                }

                //lmz解压
                var bytes = Decompress(Reader,name);

                //解压后的像素
                Queue<int> queue = new Queue<int>();
                foreach (var b in bytes)
                {
                    int low4bit = b & 0x0F;
                    int high4bit = (b & 0xF0) >> 4;
                    queue.Enqueue(low4bit);
                    queue.Enqueue(high4bit);
                }

                for (int y = 0; y < Header.Heigth; y++)
                {
                    for (int x = 0; x < Header.Width; x++)
                    {
                        int index = queue.Dequeue();
                        Picture.SetPixel(x, y, Color.FromArgb(ColorPanel[index].A, ColorPanel[index].R, ColorPanel[index].G, ColorPanel[index].B));
                    }
                }
            }
            else if (Header.Colorbits == 8)//8bit
            {
                System.Diagnostics.Debug.WriteLine(8);
                Pixel32_BGRA[] ColorPanel = new Pixel32_BGRA[256];
                Pixel32_BGRA Pixel = new Pixel32_BGRA();
                for (int i = 0; i < ColorPanel.Length; i++)
                {
                    Reader.ReadStruct(ref Pixel);
                    ColorPanel[i] = Pixel;
                }

                var bytes = Decompress(Reader);
                Queue<int> queue = new Queue<int>();
                foreach (var b in bytes)
                {
                    queue.Enqueue(b);
                }
                // var ie = bytes.GetEnumerator();
                for (int y = 0; y < Header.Heigth; y++)
                {
                    for (int x = 0; x < Header.Width; x++)
                    {
                        int index = queue.Dequeue();
                        //int index = BitConverter.ToInt16(new byte[] { ie.Current, 0x00 }, 0);
                        Picture.SetPixel(x, y, Color.FromArgb(ColorPanel[index].A, ColorPanel[index].R, ColorPanel[index].G, ColorPanel[index].B));
                    }
                }

            }
            else if (Header.Colorbits == 24)
            {
                System.Diagnostics.Debug.WriteLine(24);
                List<byte> bytes = (List<byte>)Decompress(Reader);
                Reader = new StructReader(new MemoryStream(bytes.ToArray()));

                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Pixel24_RGB Pixel = new Pixel24_RGB();
                        Reader.ReadStruct(ref Pixel);
                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                    }
            }
            else if (Header.Colorbits == 32)//32
            {
                System.Diagnostics.Debug.WriteLine(32);
                List<byte> bytes = (List<byte>)Decompress(Reader);
                Reader = new StructReader(new MemoryStream(bytes.ToArray()));


                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Pixel32_RGBA Pixel = new Pixel32_RGBA();
                        Reader.ReadStruct(ref Pixel);
                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.A, Pixel.R, Pixel.G, Pixel.B));
                    }
            }
            Reader.Close();
            return Picture;
        }

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
            header.Colorbits = 8;
            Writer.WriteStruct(ref header);
            Writer.Seek(header.HeaderLength, SeekOrigin.Begin);


            Pixel32_BGRA Pixel = new Pixel32_BGRA();
            Pixel.R = 255;
            Pixel.G = 255;
            Pixel.B = 255;
            for (int k = 0; k < 256; k++)
            {
                Pixel.A = (byte)k;
                Writer.WriteStruct(ref Pixel);
            }

            byte[] bytes = new byte[Picture.Height * Picture.Width];
            int i = 0;
            for (int y = 0; y < Picture.Height; y++)
            {
                for (int x = 0; x < Picture.Width; x++)
                {
                    bytes[i] = Picture.GetPixel(x, y).A;
                    i++;
                }
            }


            int file_num = bytes.Length / 130554 + 1;
            //System.Diagnostics.Debug.WriteLine("{0} {1} {2}", file_num, listBytes.Count, 130554);
            List<int[]> out_list = new List<int[]>();
            Writer.Write(file_num);
            for (int k = 0; k < file_num; k++)
            {
                List<int> listBytes = new List<int>();
                StringBuilder decompressed = new StringBuilder();
                byte[] tmp_bytes = new byte[130554];
                if (k == file_num - 1)
                    Array.Copy(bytes, k * 130554, tmp_bytes, 0, bytes.Length - k * 130554);
                else
                    Array.Copy(bytes, k * 130554, tmp_bytes, 0, 130554);


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
                    Writer.Write(bytes.Length - k * 130554);
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
   
        public void CZ1ToPng(string infile)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length));
            texture.Save(infile + ".png", ImageFormat.Png);
            br.Close();
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



    public struct CZ1Header
    {
        [FString(Length = 4)]
        public string Signature;
        public uint HeaderLength;
        public ushort Width;
        public ushort Heigth;
        public byte Colorbits;
        //dynamic length
    }




}
