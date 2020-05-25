using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using LucaSystem;

namespace ProtImage
{
    //https://vndb.org/r?fil=engine-RealLive
    public class DatParser
    {
        private Bitmap Export(byte[] Texture)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            DatHeader Header = new DatHeader();
            Reader.ReadStruct(ref Header);
            //Header.Blockh = (ushort)Math.Ceiling((float)Header.Heigth / (float)Header.Colorblock);
            if (Header.Signature != 2097410) //0x02012000
                throw new BadImageFormatException();

            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);

            List<byte> output = new List<byte>();

            Dictionary<int, uint> rawSizeList = new Dictionary<int, uint>();
            Dictionary<int, uint> compressedSizeList = new Dictionary<int, uint>();
            int i;
            for (i = 0; i < Header.FileCount; i++)
            {
                
                //st.Read(bytArray4Count, 0, 4); //
                uint fileCompressedSize = Reader.ReadUInt32();
                //st.Read(bytArray4Count, 0, 4); //
                uint fileRawSize = Reader.ReadUInt32();
                rawSizeList.Add(i, fileRawSize);
                compressedSizeList.Add(i, fileCompressedSize);
            }

            for (i = 0; i < Header.FileCount; i++)
            {
                List<int> lmzBytes = new List<int>();
                int totalcount = (int)compressedSizeList[i];
                for (int j = 0; j < totalcount / 2; j++)
                {
                    //st.Read(bytArray2Count, 0, 2);//
                    
                    lmzBytes.Add(Reader.ReadUInt16());
                }
                //解压lzw
                string str = LzwUtil.Decompress2(lmzBytes);
                foreach (var c in str)
                {
                    output.Add((byte)c);
                }
            }

            Pixel32 Pixel = new Pixel32();
            MemoryStream ms = new MemoryStream(output.ToArray());
            byte[] bytArray4Count = new byte[4];
            ms.Seek(0, SeekOrigin.Begin);
            for (int y = 0; y < Header.Heigth; y++)
                for (int x = 0; x < Header.Width; x++)
                {

                    ms.Read(bytArray4Count, 0, 4); //

                    Pixel.R = bytArray4Count[0] == 0 ? (byte)255 : bytArray4Count[0];
                    Pixel.G = bytArray4Count[1] == 0 ? (byte)255 : bytArray4Count[1];
                    Pixel.B = bytArray4Count[2] == 0 ? (byte)255 : bytArray4Count[2];
                    Pixel.A = bytArray4Count[3] == 0 ? (byte)255 : bytArray4Count[3];

                    Picture.SetPixel(x, y, Color.FromArgb(Pixel.A, Pixel.R, Pixel.G, Pixel.B));
                }

            Reader.Close();
            return Picture;
        }


        public void DatToPng(string infile)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length));
            texture.Save(infile + ".png", ImageFormat.Png);
            br.Close();
        }

    }

    public struct DatHeader
    {
        public uint Signature;
        public ushort Width;
        public ushort Heigth;
        public ushort DictionaryNum;
        public ushort UnKnow1;
        public uint UnKnow2;
        public ushort Width2;
        public ushort Heigth2;
        public uint UnKnow3; //00 00 00 00
        public ushort Width3;
        public ushort Heigth3;

        public uint UnKnow4;// 00 00 00 00
        public uint UnKnow5;// 10 00 00 00
        public uint UnKnow6;// 30 00 00 00
        public uint UnKnow7;//total len next bytes
        
        public ushort Width4;
        public ushort Heigth4;

        public ushort UnKnow8;
        public ushort FileCount;//block count
        public uint DecompressedLength;
        public uint CompressedLength;
   
        //dynamic length
    }




}
