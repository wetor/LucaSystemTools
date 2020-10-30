using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LucaSystem;
using LucaSystemTools;
using System.Drawing.Imaging;

namespace ProtImage
{
    //作者：marcussacana
    //时间：2018.1
    //https://github.com/marcussacana/LucaSystem
    public class CZ0Parser: CZParserBase

    {

        public override void FileExport(string infile, string outpath = null)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length));
            if (outpath != null)
            {
                texture.Save(outpath, ImageFormat.Png);
            }
            else
            {
                texture.Save(infile + ".png", ImageFormat.Png);
            }
            br.Close();
        }

        public override void FileImport(string path, string outpath = null)
        {
            throw new NotImplementedException();
        }
        byte[] Texture;
        public CZ0Parser(byte[] Texture)
        {
            this.Texture = Texture;
        }
        public Bitmap Export(byte[] Texture, string name = "")
        {
            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ0Header Header = new CZ0Header();
            Reader.ReadStruct(ref Header);

            if (Header.Signature != "CZ0\x0")
                throw new BadImageFormatException();

            Reader.Seek(Header.HeaderLength, SeekOrigin.Begin);

            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            if (Header.Colorbits == 8)
            {
                System.Diagnostics.Debug.WriteLine(8);
                Pixel32_BGRA[] ColorPanel = new Pixel32_BGRA[256];
                Pixel32_BGRA Pixel = new Pixel32_BGRA();
                for (int i = 0; i < ColorPanel.Length; i++)
                {
                    Reader.ReadStruct(ref Pixel);
                    ColorPanel[i] = Pixel;
                }
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        byte tmp = 0;
                        Reader.ReadStruct(ref tmp);
                        uint index = tmp;
                        Picture.SetPixel(x, y, Color.FromArgb(ColorPanel[index].A, ColorPanel[index].R, ColorPanel[index].G, ColorPanel[index].B));
                    }

            }
            else if (Header.Colorbits == 24)
            {
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Pixel24_RGB Pixel = new Pixel24_RGB();
                        Reader.ReadStruct(ref Pixel);
                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                    }
            }
            else if (Header.Colorbits == 32)
            {
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


        public CZ0Parser()
        {

        }

    }
    public struct CZ0Header
    {
        [FString(Length = 4)]
        public string Signature;
        public uint HeaderLength;
        public ushort Width;
        public ushort Heigth;
        public byte Colorbits;
        //dynamic length
    }
    public struct Pixel24_RGB
    {
        public byte R, G, B;
    }
    public struct Pixel32_BGRA
    {
        public byte B, G, R, A;
    }
    public struct Pixel32_RGBA
    {
        public byte R, G, B, A;
    }
}
