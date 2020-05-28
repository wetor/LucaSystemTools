using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using LucaSystem;

namespace ProtImage
{

    public class DatParser
    {
        private Bitmap Export(byte[] Texture)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            DatHeader Header = new DatHeader();
            Reader.ReadStruct(ref Header);
            //Header.Blockh = (ushort)Math.Ceiling((float)Header.Heigth / (float)Header.Colorblock);
            Pixel32[] ColorPanel = new Pixel32[0];

            uint Signature = Header.Signature;
            int PixivBytes = 4;
            int DatType = 1;

            if (Signature == 0x00200102)
            {
                Console.WriteLine("DatType 1");//no colorpanel //32 or 24bit
            }
            else if (Signature == 0x04280102)
            {
                Console.WriteLine("DatType 2");//has colorpanel 256色 8bit
                DatType = 2;
            }
            else
            {
                //throw new BadImageFormatException();
            }

            if (Header.ColorbitsType == 0x80)
            {
                PixivBytes = 4;
            }
            else if (Header.ColorbitsType == 0xA8)
            {
                PixivBytes = 3;
            }
            else if (Header.ColorbitsType == 0x81)
            {
                PixivBytes = 1;
            }

            if (DatType == 2)
            {
                Pixel32 tmpPixel=new Pixel32();
                ColorPanel = new Pixel32[256];
                for (int j = 0; j < ColorPanel.Length; j++)
                {
                    //TODO 颜色表的 颜色还有错误
                    tmpPixel.B = Reader.ReadByte();
                    tmpPixel.G = Reader.ReadByte();
                    tmpPixel.R = Reader.ReadByte();
                    tmpPixel.A = Reader.ReadByte();
                    ColorPanel[j] = tmpPixel;
                }
                //256*4
                Reader.ReadUInt32(); //10 00 00 00  //16 Unknow
                Reader.ReadUInt32();   //38 04 00 00  //1080 Unknow
            }
            Reader.ReadUInt32();//total len next bytes
            Reader.ReadUInt32();//w h
            Reader.ReadUInt16();//unknow ushort
            int BlockCount = Reader.ReadUInt16();//block count

            uint decompressedLen = Reader.ReadUInt32();
            Console.WriteLine("decompressedTotalLen :" + decompressedLen);
            uint compressedLen = Reader.ReadUInt32();
            Console.WriteLine("compressedTotalLen :" + compressedLen);


            List<byte> output = new List<byte>();
            Dictionary<int, uint> rawSizeList = new Dictionary<int, uint>();
            Dictionary<int, uint> compressedSizeList = new Dictionary<int, uint>();
            List<uint> color_line = new List<uint>();
            int i;
            float sum = 0;
            uint sum2 = 0;
            for (i = 0; i < BlockCount; i++)
            {
                uint fileCompressedSize = Reader.ReadUInt32();
                uint fileRawSize = Reader.ReadUInt32();
                rawSizeList.Add(i, fileRawSize);
                compressedSizeList.Add(i, fileCompressedSize);
                color_line.Add((uint)Math.Ceiling(sum));
                uint pre_sum = (uint)Math.Ceiling(sum);
                sum += (float)fileRawSize / (float)Header.Width / (float)PixivBytes;
                uint a = (uint)Math.Ceiling(sum) - pre_sum;
                sum2 += (uint)(a * 960 * PixivBytes);
                Console.WriteLine("{0} {1} {2}  {3} {4}", a * 960 * PixivBytes, fileRawSize, a, sum2, sum);

            }
            color_line.Add((uint)Math.Ceiling(sum));
            Console.WriteLine("{0} {1} {2}", (uint)Math.Ceiling(sum), sum2, Header.Width * Header.Heigth * 3);

            //List<byte> nextBytes = new List<byte>();
            for (i = 0; i < BlockCount; i++)
            {
                List<int> lmzBytes = new List<int>();
                List<byte> unBytes = new List<byte>();
                int totalcount = (int)compressedSizeList[i];
                for (int j = 0; j < totalcount / 2; j++)
                {
                    //st.Read(bytArray2Count, 0, 2);//

                    lmzBytes.Add(Reader.ReadUInt16());
                }
                //解压lzw
                string str = LzwUtil2.Decompress(lmzBytes);
                foreach (var c in str)
                {
                    unBytes.Add((byte)c);
                }

                //颜色填充方式
                if (Header.ColorbitsAlgorithm == 0x00)
                {
                    //raw

                }
                else if (Header.ColorbitsAlgorithm == 0x02)
                {
                    //diff
                    int pixivNum = unBytes.Count / PixivBytes;

                    int lineBytesNum = Header.Width * PixivBytes;
                    byte[] pre_line = new byte[lineBytesNum];
                    byte[] curr_line = new byte[lineBytesNum];
                    for (int idx = 0; idx < Math.Ceiling((double)pixivNum / (double)Header.Width); idx++)
                    {
                        unBytes.CopyTo(idx * lineBytesNum, curr_line, 0, (idx + 1) * lineBytesNum < unBytes.Count ? lineBytesNum : unBytes.Count % lineBytesNum);
                        if (idx != 0)
                        {
                            for (int x = 0; x < lineBytesNum; x++)
                            {
                                curr_line[x] += pre_line[x];
                                curr_line[x]--;
                            }
                        }
                        for (int x = 0; x < lineBytesNum && idx * lineBytesNum + x < unBytes.Count; x++)
                        {
                            unBytes[idx * lineBytesNum + x] = curr_line[x];
                        }
                        curr_line.CopyTo(pre_line, 0);
                    }
                }
                output.AddRange(unBytes);



            }
            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
            Pixel32 Pixel = new Pixel32();
            MemoryStream ms = new MemoryStream(output.ToArray());
            byte[] bytArray4Count = new byte[4];
            ms.Seek(0, SeekOrigin.Begin);
            if (PixivBytes==4)//32
            {
                Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {

                        ms.Read(bytArray4Count, 0, 4); //

                        Pixel.R = bytArray4Count[0] == 0 ? (byte)0xFF : bytArray4Count[0];
                        Pixel.G = bytArray4Count[1] == 0 ? (byte)0xFF : bytArray4Count[1];
                        Pixel.B = bytArray4Count[2] == 0 ? (byte)0xFF : bytArray4Count[2];
                        Pixel.A = bytArray4Count[3] == 0 ? (byte)0xFF : bytArray4Count[3];

                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                    }
            }

            else if (PixivBytes==3)//24 rgb
            {
                Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format24bppRgb);
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {

                        ms.Read(bytArray4Count, 0, 3); //

                        Pixel.R = bytArray4Count[0] == 0 ? (byte)0xFF : bytArray4Count[0];
                        Pixel.G = bytArray4Count[1] == 0 ? (byte)0xFF : bytArray4Count[1];
                        Pixel.B = bytArray4Count[2] == 0 ? (byte)0xFF : bytArray4Count[2];
                        //Pixel.A = bytArray4Count[3] == 0 ? (byte)0xFF : bytArray4Count[3];

                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                    }
            }

            else if (PixivBytes == 1)//8 argb
            {
                //Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format24bppRgb);
                //for (int y = 0; y < Header.Heigth; y++)
                //for (int x = 0; x < Header.Width; x++)
                //{

                //    ms.Read(bytArray4Count, 0, 3); //

                //    Pixel.R = bytArray4Count[0] == 0 ? (byte)0xFF : bytArray4Count[0];
                //    Pixel.G = bytArray4Count[1] == 0 ? (byte)0xFF : bytArray4Count[1];
                //    Pixel.B = bytArray4Count[2] == 0 ? (byte)0xFF : bytArray4Count[2];
                //    //Pixel.A = bytArray4Count[3] == 0 ? (byte)0xFF : bytArray4Count[3];

                //    Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                //}
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
        public ushort UnKnow;//01 01
        public byte ColorbitsType;
        public byte ColorbitsAlgorithm;
        public uint UnKnow2;
        public ushort Width2;
        public ushort Heigth2;
        public uint UnKnow3; //00 00 00 00
        public ushort Width3;
        public ushort Heigth3;

        public uint UnKnow4;// 00 00 00 00
        public uint UnKnow5;// 10 00 00 00
        public uint UnKnow6;// 30 00 00 00

    }




}
