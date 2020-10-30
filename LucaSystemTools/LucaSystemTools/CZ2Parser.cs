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
    /// <summary>
    /// 仅供参考未完成
    /// </summary>
    public class CZ2Parser : CZParserBase
    {
        public IEnumerable<byte> Decompress(StructReader Reader)
        {
            List<byte> output = new List<byte>();
            uint fileCount = Reader.ReadUInt32();
            Dictionary<int, uint> rawSizeList = new Dictionary<int, uint>();
            Dictionary<int, uint> compressedSizeList = new Dictionary<int, uint>();
            for (int i = 0; i < fileCount; i++)
            {
                uint fileCompressedSize = Reader.ReadUInt32();
                uint fileRawSize = Reader.ReadUInt32();
                rawSizeList.Add(i, fileRawSize);
                compressedSizeList.Add(i, fileCompressedSize);
            }

            for (int i = 0; i < fileCount; i++)
            {
                List<UInt16> lmzBytes = new List<UInt16>();
                int totalcount = (int)compressedSizeList[i];
                for (int j = 0; j < totalcount/2; j++)
                {
                    lmzBytes.Add(Reader.ReadUInt16());
                }
                //解压lzw
                /*byte[] re = unlzw(lmzBytes.ToArray());
                output.AddRange(re);*/
                string str = Lzw2Util.Decompress(lmzBytes);
                foreach (var c in str)
                {
                    output.Add((byte)c);
                }
            }
            return output;
        }


        private Bitmap Export(byte[] Texture)
        {
            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ2Header Header = new CZ2Header();
            Reader.ReadStruct(ref Header);

            if (Header.Signature != "CZ2\x0")
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
                var bytes = Decompress(Reader);

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
                bool isError = false;
                // var ie = bytes.GetEnumerator();
                for (int y = 0; y < Header.Heigth; y++)
                {
                    for (int x = 0; x < Header.Width; x++)
                    {
                        try
                        {
                            int index = queue.Dequeue();
                            //int index = BitConverter.ToInt16(new byte[] { ie.Current, 0x00 }, 0);
                            Picture.SetPixel(x, y, Color.FromArgb(ColorPanel[index].A, ColorPanel[index].R, ColorPanel[index].G, ColorPanel[index].B));

                        }
                        catch
                        {
                            Console.WriteLine(isError);
                            isError = true;
                            break;
                        }
                    }
                    if (isError)
                    {
                        Console.WriteLine(isError);
                        break;
                    }
                }

            }
            //else if (Header.Colorbits == 24)
            //{

            //    for (int y = 0; y < Header.Heigth; y++)
            //    for (int x = 0; x < Header.Width; x++)
            //    {
            //        Pixel24 Pixel = new Pixel24();
            //        Reader.ReadStruct(ref Pixel);
            //        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
            //    }
            //}
            //else if (Header.Colorbits == 32)//32
            //{

            //    for (int y = 0; y < Header.Heigth; y++)
            //    for (int x = 0; x < Header.Width; x++)
            //    {
            //        Pixel32 Pixel = new Pixel32();
            //        Reader.ReadStruct(ref Pixel);
            //        Picture.SetPixel(x, y, Color.FromArgb(Pixel.A, Pixel.R, Pixel.G, Pixel.B));
            //    }
            //}
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

        public override void FileExport(string path, string outpath = null)
        {
            CZ1ToPng(path);
        }

        public override void FileImport(string path, string outpath = null)
        {
            PngToCZ1(path);
        }
    }



    public struct CZ2Header
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
