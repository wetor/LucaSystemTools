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

namespace ProtImage
{
//注：部分代码有参考现存代码，来源忘了 2020.5.24
    public class CZ4Parser: CZParserBase
    {

        //作者：Wetor, Devseed
        //时间：2020.9.8
        private Bitmap Export(byte[] Texture)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ4Header Header = new CZ4Header();
            Reader.ReadStruct(ref Header);
            Header.Blockh = (ushort)Math.Ceiling((float)Header.Heigth / (float)Header.Colorblock);
            if (Header.Signature != "CZ4\x0")
                throw new BadImageFormatException();

            Reader.Seek(Header.HeaderLength, SeekOrigin.Begin);
            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
            if (Header.Colorbits == 4)//4bit
            {
                
            }
            else if (Header.Colorbits == 8)//8bit
            {
               

            }
            else if (Header.Colorbits == 32)
            {

                var bytes = Decompress(Reader);
                byte[] data = bytes.ToArray();

                byte[] curr_line = new byte[Header.Width * 3];
                byte[] pre_line = new byte[Header.Width * 3];
                byte[] curr_line_alpha = new byte[Header.Width];
                byte[] pre_line_alpha = new byte[Header.Width];
                int alpha_start = Header.Width * Header.Heigth * 3; 
                int i = 0, i2= alpha_start; //i2 alpha

                //RGB888 delta, the alpha delta is in the last
                for (int y = 0; y < Header.Heigth; y++) 
                {

                    Buffer.BlockCopy(data, i, curr_line, 0, Header.Width * 3);
                    Buffer.BlockCopy(data, i2, curr_line_alpha, 0, Header.Width);

                    if (y % Header.Blockh != 0)
                    {
                        for (int x = 0; x < Header.Width * 3; x++)
                        {
                            curr_line[x] += pre_line[x];
                        }
                        for (int x = 0; x < Header.Width; x++)
                        {
                            curr_line_alpha[x] += pre_line_alpha[x];
                        }
                    }
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Picture.SetPixel(x, y, Color.FromArgb(curr_line_alpha[x], curr_line[x * 3 + 0], curr_line[x * 3 + 1], curr_line[x * 3 + 2]));
                    }
                    curr_line.CopyTo(pre_line, 0);
                    curr_line_alpha.CopyTo(pre_line_alpha, 0);
                    i += Header.Width * 3;
                    i2 += Header.Width;
                }
            }
            Reader.Close();
            return Picture;
        }
     
        //作者：Wetor
        //时间：2019.1.18
        public void PngToCZ4(string outfile)
        {
            
        }
        //作者：Wetor
        //时间：2019.7.25
        public void CZ4ToPng(string infile)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length));
            texture.Save(infile + ".png", ImageFormat.Png);
            br.Close();
        }

        public override void FileExport(string name)
        {
            CZ4ToPng(name);
        }

        public override void FileImport(string name)
        {
            PngToCZ4(name);
        }
    }



    public struct CZ4Header
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
