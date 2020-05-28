using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ProtImage
{

    public class DatParser
    {



        public static string Decompress(List<int> compressed)
        {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256 + 1; i++)
                dictionary.Add(i, ((char)i).ToString());

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                // new sequence; add it to the dictionary
                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
        }

        private Bitmap Export(byte[] Texture)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            DatHeader Header = new DatHeader();
            Reader.ReadStruct(ref Header);
            //Header.Blockh = (ushort)Math.Ceiling((float)Header.Heigth / (float)Header.Colorblock);
            if (Header.Signature != 2097410) //0x02012000
                throw new BadImageFormatException();


            List<byte> output = new List<byte>();

            Dictionary<int, uint> rawSizeList = new Dictionary<int, uint>();
            Dictionary<int, uint> compressedSizeList = new Dictionary<int, uint>();
            List<uint> color_line = new List<uint>();
            int i;
            float sum = 0;
            uint sum2 = 0;
            if (Header.ColorbitsType == 0x02A80101) //24
                Header.PixivBytes = 3;
            else if (Header.ColorbitsType == 0x02800101) //32
                Header.PixivBytes = 4;
            for (i = 0; i < Header.FileCount; i++)
            {

                //st.Read(bytArray4Count, 0, 4); //
                uint fileCompressedSize = Reader.ReadUInt32();
                //st.Read(bytArray4Count, 0, 4); //
                uint fileRawSize = Reader.ReadUInt32();
                rawSizeList.Add(i, fileRawSize);
                compressedSizeList.Add(i, fileCompressedSize);
                //int n = (int)Math.Ceiling((float)fileRawSize / (float)Header.Heigth / 4);
                //Console.WriteLine("{0} {1} {2}", Math.Ceiling(sum), (float)fileRawSize / (float)Header.Heigth / 4, n);
                color_line.Add((uint)Math.Ceiling(sum));
                uint pre_sum = (uint)Math.Ceiling(sum);

                sum += (float)fileRawSize / (float)Header.Width / (float)Header.PixivBytes;
                uint a = (uint)Math.Ceiling(sum) - pre_sum;
                sum2 += (uint)(a * 960 * Header.PixivBytes);
                Console.WriteLine("{0} {1} {2}  {3} {4}", a * 960 * Header.PixivBytes, fileRawSize, a, sum2, sum);

            }
            //290880 289141
            //273600 274891
            //233280 231527

            //204480 
            color_line.Add((uint)Math.Ceiling(sum));
            Console.WriteLine("{0} {1} {2}", (uint)Math.Ceiling(sum), sum2, Header.Width * Header.Heigth * 3);

            List<byte> nextBytes = new List<byte>();
            for (i = 0; i < Header.FileCount; i++)
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
                string str = Decompress(lmzBytes);


                foreach (var c in str)
                {
                    unBytes.Add((byte)c);
                }
                //处理颜色拆分
                int pixivNum = unBytes.Count / Header.PixivBytes;

                int lineBytesNum = Header.Width * Header.PixivBytes;
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
                output.AddRange(unBytes);

            }
            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, PixelFormat.Format32bppArgb);
            Pixel32 Pixel = new Pixel32();
            MemoryStream ms = new MemoryStream(output.ToArray());
            byte[] bytArray4Count = new byte[4];
            ms.Seek(0, SeekOrigin.Begin);
            if (Header.ColorbitsType == 0x00800101 || Header.ColorbitsType == 0x02800101)//32
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

            else if (Header.ColorbitsType == 0x00A80101 || Header.ColorbitsType == 0x02A80101)//24 rgb
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
        public uint ColorbitsType;
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

        [Ignore]
        public ushort Blockh;
        [Ignore]
        public int PixivBytes;
        //dynamic length
    }




}
