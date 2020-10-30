using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using LucaSystem;
using LucaSystemTools;
using LucaSystemTools.Utils;

namespace ProtImage
{
    //注：部分代码有参考现存代码，来源忘了 2020.5.24
    public class CZ3Parser: CZParserBase
    {

 
        private Bitmap Export(byte[] Texture)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ3Header Header = new CZ3Header();
            Reader.ReadStruct(ref Header);
         
            if (Header.Signature != "CZ3\x0")
                throw new BadImageFormatException();

            Reader.Seek(Header.HeaderLength, SeekOrigin.Begin);
            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
            if (Header.Colorbits == 4)//4bit
            {
                
            }
            else if (Header.Colorbits == 8)//8bit
            {
               

            }
            else if (Header.Colorbits == 32)//32
            {
                var bytes = Decompress(Reader);
                byte[] data = bytes.ToArray();
                int PixelByteCount = 4;
                ImageFillPartten.LineDiffPattern(ref Picture, Header.Colorblock, PixelByteCount, data,null);
            }
            Reader.Close();
            return Picture;
        }



        //作者：Wetor
        //时间：2019.1.18
        public void PngToCZ3(string outfile)
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
        //作者：Wetor
        //时间：2019.7.25
        public void CZ3ToPng(string infile)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length));
            texture.Save(infile + ".png", ImageFormat.Png);
            br.Close();
        }

        public override void FileExport(string name, string outpath = null)
        {
            CZ3ToPng(name);
        }

        public override void FileImport(string name, string outpath = null)
        {
            PngToCZ3(name);
        }
    }



    public struct CZ3Header
    {
        [FString(Length = 4)]
        public string Signature;
        public uint HeaderLength;
        public ushort Width;
        public ushort Heigth;
        public ushort Colorbits;
        public ushort Colorblock;
        [Ignore]
        public ushort Blockh;
        //dynamic length
    }




}
