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

    public class DatParser:AbstractFileParser
    {
        private Bitmap Export(byte[] Texture,string infile)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            DatHeader Header = new DatHeader();
            Reader.ReadStruct(ref Header);
            //Header.Blockh = (ushort)Math.Ceiling((float)Header.Heigth / (float)Header.Colorblock);
            Pixel32_BGRA[] ColorPanel = new Pixel32_BGRA[0];

            uint Signature = Header.Signature;
            int PixivBytes = 0;
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
                Console.WriteLine("Unsupport Format Signature:" + Header.Signature);
                File.AppendAllText( "ErrOutput.txt", "Unsupport Format Signature:" + Header.Signature+"\r\n"+ infile + "\r\n");
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
            else
            {
                Console.WriteLine("Unsupport Format ColorbitsType:" + Header.ColorbitsType);
                File.AppendAllText("ErrOutput.txt", "Unsupport Format ColorbitsType:" + Header.ColorbitsType + "\r\n" + infile + "\r\n");
                return null;
            }

            if (DatType == 2)
            {
                Pixel32_BGRA tmpPixel=new Pixel32_BGRA();
                ColorPanel = new Pixel32_BGRA[256];
                for (int j = 0; j < ColorPanel.Length; j++)
                {
                    //TODO 颜色表的 颜色还有错误
                    tmpPixel.R = (byte)(Reader.ReadByte());
                    tmpPixel.G = (byte)(Reader.ReadByte());
                    tmpPixel.B = (byte)(Reader.ReadByte());
                    tmpPixel.A = (byte)(Reader.ReadByte());
                    //tmpPixel.R = (byte)(tmpPixel.R == 0 ? 0xFF : tmpPixel.R);
                    //tmpPixel.G = (byte)(tmpPixel.G == 0 ? 0xFF : tmpPixel.G);
                    //tmpPixel.B = (byte)(tmpPixel.B == 0 ? 0xFF : tmpPixel.B);
                    //tmpPixel.A = (byte)(tmpPixel.A == 0 ? 0xFF : tmpPixel.A);
                    ColorPanel[j] = tmpPixel;
                }

                //256*4
                Reader.ReadUInt32(); //10 00 00 00  //16 Unknow
                Reader.ReadUInt32();   //38 04 00 00  //1080 Unknow
            }
            Reader.ReadUInt32();//totallennextbytes=blocksize_byteslen+compressedLen
            Reader.ReadUInt32();//w h
            Reader.ReadUInt16();//blocksize_byteslen=2+2+4+4+blockcount*8
            int BlockCount = Reader.ReadUInt16();//block count

            uint decompressedLen = Reader.ReadUInt32();
            Console.WriteLine("decompressedTotalLen :" + decompressedLen);
            uint compressedLen = Reader.ReadUInt32();
            Console.WriteLine("compressedTotalLen :" + compressedLen);


            List<byte> output = new List<byte>();
            Dictionary<int, uint> rawSizeList = new Dictionary<int, uint>();
            Dictionary<int, uint> compressedSizeList = new Dictionary<int, uint>();
            int i;
            for (i = 0; i < BlockCount; i++)
            {
                uint fileCompressedSize = Reader.ReadUInt32();
                uint fileRawSize = Reader.ReadUInt32();
                rawSizeList.Add(i, fileRawSize);
                compressedSizeList.Add(i, fileCompressedSize);
            }

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
                string str = LzwUtil.Decompress2(lmzBytes);
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
            Pixel32_BGRA Pixel = new Pixel32_BGRA();
            MemoryStream ms = new MemoryStream(output.ToArray());
            byte[] bytArray4Count = new byte[4];
            ms.Seek(0, SeekOrigin.Begin);
            if (PixivBytes==4)//32 argb
            {
                Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {

                        ms.Read(bytArray4Count, 0, 4); //

                        Pixel.R = --bytArray4Count[0];// == 0 ? (byte)0xFF : bytArray4Count[0];
                        Pixel.G = --bytArray4Count[1];// == 0 ? (byte)0xFF : bytArray4Count[1];
                        Pixel.B = --bytArray4Count[2];// == 0 ? (byte)0xFF : bytArray4Count[2];
                        Pixel.A = --bytArray4Count[3];// == 0 ? (byte)0xFF : bytArray4Count[3];

                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.A, Pixel.R, Pixel.G, Pixel.B));
                    }
            }

            else if (PixivBytes==3)//24 rgb
            {
                Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format24bppRgb);
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {

                        ms.Read(bytArray4Count, 0, 3); //

                        Pixel.R = --bytArray4Count[0];// == 0 ? (byte)0xFF : bytArray4Count[0];
                        Pixel.G = --bytArray4Count[1];// == 0 ? (byte)0xFF : bytArray4Count[1];
                        Pixel.B = --bytArray4Count[2];// == 0 ? (byte)0xFF : bytArray4Count[2];
                        //Pixel.A = bytArray4Count[3] == 0 ? (byte)0xFF : bytArray4Count[3];

                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                    }
            }

            else if (PixivBytes == 1)//8 argb
            {
                Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
                for (int y = 0; y < Header.Heigth; y++)
                for (int x = 0; x < Header.Width; x++)
                {
                    byte b = (byte)(ms.ReadByte()-1); //
                    Pixel.R = ColorPanel[b].R;
                    Pixel.G = ColorPanel[b].G;
                    Pixel.B = ColorPanel[b].B;
                    Pixel.A = ColorPanel[b].A;

                    Picture.SetPixel(x, y, Color.FromArgb(Pixel.A,Pixel.R, Pixel.G, Pixel.B));
                }
            }


            Reader.Close();
            return Picture;
        }


        public void DatToPng(string infile)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length), infile);
            if(texture!=null)
            texture.Save(infile + ".png", ImageFormat.Png);
            br.Close();
        }

        public override void FileExport(string path, string outpath = null)
        {
            DatToPng(path);
        }

        public override void FileImport(string path, string outpath = null)
        {
            throw new NotImplementedException();
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
