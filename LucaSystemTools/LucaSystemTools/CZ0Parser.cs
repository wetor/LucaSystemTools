using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ProtImage
{
    //作者：marcussacana
    //时间：2018.1
    //https://github.com/marcussacana/LucaSystem
    public class CZ0Parser
    {
        byte[] Texture;
        public CZ0Parser(byte[] Texture)
        {
            this.Texture = Texture;
        }

        public Bitmap Import()
        {
            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ0Header Header = new CZ0Header();
            Reader.ReadStruct(ref Header);

            if (Header.Signature != "CZ0\x0")
                throw new BadImageFormatException();

            Reader.Seek(Header.HeaderLength, SeekOrigin.Begin);

            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int y = 0; y < Header.Heigth; y++)
                for (int x = 0; x < Header.Width; x++)
                {
                    Pixel Pixel = new Pixel();
                    Reader.ReadStruct(ref Pixel);
                    Picture.SetPixel(x, y, Color.FromArgb(Pixel.A, Pixel.R, Pixel.G, Pixel.B));
                }

            Reader.Close();

            return Picture;
        }



    }
    public struct CZ0Header
    {
        [FString(Length = 4)]
        public string Signature;
        public uint HeaderLength;
        public ushort Width;
        public ushort Heigth;
        //dynamic length
    }
    public struct Pixel32
    {
        public byte B, G, R, A;
    }
    public struct Pixel
    {
        public byte R, G, B, A;
    }
}
