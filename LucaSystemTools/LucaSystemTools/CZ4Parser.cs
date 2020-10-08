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
                int pcount = Header.Width * Header.Heigth;
                byte[] data = bytes.ToArray();
                byte[] data2 = new byte[pcount];
                Buffer.BlockCopy(data, pcount * 3, data2, 0, pcount);

                int PixelByteCount = 3;
                ImageFillPartten.LineDiffPattern(ref Picture, Header.Colorblock, PixelByteCount, data,null);

                PixelByteCount = 1;
                ImageFillPartten.LineDiffPattern(ref Picture, Header.Colorblock, PixelByteCount, data2,
                    delegate (int x, int y, byte[] curr_line)
                    {
                        var pixel = Picture.GetPixel(x, y);
                        Picture.SetPixel(x, y, Color.FromArgb(curr_line[x], pixel.R, pixel.G, pixel.B));

                    });
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

        public override void FileExport(string name, string outpath = null)
        {
            CZ4ToPng(name);
        }

        public override void FileImport(string name, string outpath = null)
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
