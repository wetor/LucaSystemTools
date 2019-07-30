using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using PbvCompressor;

namespace ProtImage
{
    public class CZ3Parser
    {


        //作者：DeQxJ00
        //时间：2019.1.17
        private string Decompress(List<int> compressed)
        {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            string w = dictionary[0];
            //compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder();

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

        //作者：DeQxJ00
        //时间：2019.1.17
        private IEnumerable<byte> Decompress(StructReader Reader)
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
                List<int> lmzBytes = new List<int>();
                int totalcount = (int)compressedSizeList[i];
                for (int j = 0; j < totalcount; j++)
                {
                    lmzBytes.Add(Reader.ReadUInt16());
                }
                //解压lzw
                /*byte[] re = unlzw(lmzBytes.ToArray());
                output.AddRange(re);*/
                string str = Decompress(lmzBytes);
                foreach (var c in str)
                {
                    output.Add((byte)c);
                }
            }
            return output;
        }
        //作者：DeQxJ00
        //时间：2019.1.17
        private Bitmap Export(byte[] Texture)
        {

            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ3Header Header = new CZ3Header();
            Reader.ReadStruct(ref Header);
            Header.Blockh = (ushort)Math.Ceiling((float)Header.Heigth / (float)Header.Colorblock);
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

 
                byte[] curr_line = new byte[Header.Width * 4];
                byte[] pre_line = new byte[Header.Width * 4];
                int i = 0;
                for (int y = 0; y < Header.Heigth; y++)
                {

                    Buffer.BlockCopy(data, i, curr_line, 0, Header.Width * 4);


                    if (y % Header.Blockh != 0)
                        for (int x = 0; x < Header.Width * 4; x++)
                            curr_line[x] += pre_line[x];
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Picture.SetPixel(x, y, Color.FromArgb(curr_line[x * 4 + 3], curr_line[x * 4 + 0], curr_line[x * 4 + 1], curr_line[x * 4 + 2]));
                    }
                    curr_line.CopyTo(pre_line, 0);
                    i += Header.Width * 4;

                }
            }
            Reader.Close();
            return Picture;
        }
        //作者：Wetor
        //时间：2019.1.18
        private List<int> Compress(string uncompressed)
        {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);
            dictionary.Add(((char)256).ToString(), 0);
            string w = string.Empty;
            List<int> compressed = new List<int>();
            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            return compressed;
        }
        //作者：Wetor
        //时间：2019.1.18
        private List<int> Compress(byte[] uncompressed)
        {
            // build the dictionary
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            for (int i = 0; i < 256; i++)
                dictionary.Add((i + 256).ToString(), i);
            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (byte c in uncompressed)
            {

                string wc = w + (c + 256).ToString();
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = (c + 256).ToString();
                }
            }

            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            return compressed;
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


            Pixel32 Pixel = new Pixel32();
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
                listBytes = Compress(decompressed.ToString());
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
