using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using LucaSystem;
using LucaSystemTools;
using System.Linq;
using Newtonsoft.Json;

namespace ProtImage
{
    /// <summary>
    /// 根据psv flowers 春生成
    /// 确认硬编码部分为 blockcount和每个block的compressedsize
    /// </summary>
    public class CZ1_4bitParser : CZ1Parser
    {
        public byte color256to16(byte b)
        {
            int index = 0;
            byte min = 0xFF;
            for (int i = 0; i < list.Count; i++)
            {
                byte tmp = (byte)Math.Abs(b - list[i]);
                if (tmp < min)
                {
                    min = tmp;
                    index = i;
                }
            }
            return list[index];
        }


        List<byte> list = new List<byte>();
        private int k;

        //作者：Wetor
        //时间：2019.1.18
        public void PngToCZ1(string outfile)
        {
            CZOutputInfo czOutput = new CZOutputInfo();
            string name = outfile.Replace(".png", "").Replace(".Png", "").Replace(".PNG", "") + ".json";
            if (File.Exists(name))
            {
                czOutput = JsonConvert.DeserializeObject<CZOutputInfo>(File.ReadAllText(name));
            }

            Bitmap Picture = new Bitmap(File.Open(outfile, FileMode.Open));
            StructWriter Writer = new StructWriter(File.Open(outfile + ".cz1", FileMode.Create));
            CZ1Header header;
            header.Signature = "CZ1";
            header.HeaderLength = 0x10;
            header.Width = (ushort)Picture.Width;
            header.Heigth = (ushort)Picture.Height;
            header.Colorbits = 4;
            Writer.WriteStruct(ref header);
            Writer.Seek(header.HeaderLength, SeekOrigin.Begin);

            
            Pixel32_BGRA Pixel = new Pixel32_BGRA();
            Pixel.R = 255;
            Pixel.G = 255;
            Pixel.B = 255;
            //FF FF FF 00
            //FF FF FF 18
            //FF FF FF 26
            //FF FF FF 35
            //FF FF FF 45
            //FF FF FF 55
            //FF FF FF 65
            //FF FF FF 75
            //FF FF FF 85
            //FF FF FF 96
            //FF FF FF A7
            //FF FF FF B8
            //FF FF FF C9
            //FF FF FF DB
            //FF FF FF EC
            //FF FF FF FF


            Pixel.A = (byte)0x00; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x18; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x26; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x35; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x45; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x55; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x65; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x75; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x85; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0x96; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xA7; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xB8; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xC9; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xDB; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xEC; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Pixel.A = (byte)0xFF; list.Add(Pixel.A);
            Writer.WriteStruct(ref Pixel);
            Queue<byte> queue = new Queue<byte>();
            int i = 0;
            for (int y = 0; y < Picture.Height; y++)
            {
                for (int x = 0; x < Picture.Width; x++)
                {
                    byte tmp = color256to16(Picture.GetPixel(x, y).A);
                    //bytes11[i] = (byte)list.IndexOf(tmp);
                    queue.Enqueue((byte)list.IndexOf(tmp));
                    i++;
                }
            }

            byte[] bytes2 = new byte[Picture.Height * Picture.Width];

            for (int j = 0; j < bytes2.Length / 2; j++)
            {
                var low4bit = queue.Dequeue();
                var high4bit = queue.Dequeue();
                var b = (uint)(high4bit << 4) + (uint)low4bit;
                bytes2[j] = (byte)b;
            }

        

            var ie = bytes2.ToList().GetEnumerator();
            List<List<int>> out_list = LzwUtil.Compress(ie, 0xFEFD);

            foreach (var item in out_list)
            {
                Console.WriteLine("add compressed count :" + item.Count);
                Console.WriteLine(item.Count);
                Console.WriteLine(LzwUtil.Decompress(item).Length);
            }



        //for (int k = 0; k < out_list.Count; k++)
        //{
        //    Writer.Write(out_list[k].Count);
        //    Writer.Write(LzwUtil.Decompress(out_list[k]).Length);
        //    List<byte> bytes = new List<byte>();
        //    foreach (var item in out_list[k])
        //    {
        //        UInt16 bb = (UInt16)item;
        //        bytes.Add(BitConverter.GetBytes(bb)[0]);
        //        bytes.Add(BitConverter.GetBytes(bb)[1]);
        //    }
        //    //string tmp_str;
        //    System.Diagnostics.Debug.WriteLine("{0}", k);
        //}
        int file_num = out_list.Count;
            Writer.Write(out_list.Count);

            for (int k = 0; k < out_list.Count; k++)
            {
                //if (k >= out_list.Count - 1)
                //{
                //    Writer.Write((UInt32)0);
                //    Writer.Write((UInt32)0);
                //}
                //else
                //{
                    Writer.Write(out_list[k].Count);
                    Writer.Write(LzwUtil.Decompress(out_list[k]).Length);
                    //List<byte> bytes = new List<byte>();
                    //foreach (var item in out_list[k])
                    //{
                    //    UInt16 bb = (UInt16)item;
                    //    bytes.Add(BitConverter.GetBytes(bb)[0]);
                    //    bytes.Add(BitConverter.GetBytes(bb)[1]);
                    //}
                    //string tmp_str;
                    System.Diagnostics.Debug.WriteLine("{0}", k);
                //}
            }

            //Writer.Write(czOutput.filecount);
            //for (int k = 0; k < czOutput.filecount; k++)
            //{
            //    if (k >= out_list.Count - 1)
            //    {
            //        Writer.Write((UInt32)0);
            //        Writer.Write((UInt32)0);
            //    }
            //    else
            //    {
            //        Writer.Write(out_list[k].Count);
            //        Writer.Write(LzwUtil.Decompress(out_list[k]).Length);
            //        //List<byte> bytes = new List<byte>();
            //        //foreach (var item in out_list[k])
            //        //{
            //        //    UInt16 bb = (UInt16)item;
            //        //    bytes.Add(BitConverter.GetBytes(bb)[0]);
            //        //    bytes.Add(BitConverter.GetBytes(bb)[1]);
            //        //}
            //        //string tmp_str;
            //        System.Diagnostics.Debug.WriteLine("{0}", k);
            //    }
            //}
            uint totalsize_new = 0x10 + 4 * 16 + 4 + (uint)file_num * 4 * 2;
            uint totalsize_org = 0x10 + 4 * 16 + 4 + czOutput.filecount * 4 * 2;
            for (int k = 0; k < out_list.Count; k++)
            {
                for (int kk = 0; kk < out_list[k].Count; kk++)
                {
                    Writer.Write((UInt16)out_list[k][kk]);
                    totalsize_new += 2;
                }
            }

            totalsize_org += czOutput.TotalCompressedSize*2;
            int diff = (int) (totalsize_org- totalsize_new);

            if (diff > 0)
            {
                diff = diff / 2;
                for (uint j = 0; j < diff; j++)
                {
                    Writer.Write((UInt16)0);
                }
            }
            else
            {
                Console.WriteLine("超长");
            }

            //int p = 0;
            //foreach (var blockinfo in czOutput.blockinfo)
            //{
            //    if (out_list.Count - 1 >= p)
            //    {
            //        for (int kk = 0; kk < out_list[p].Count; kk++)
            //        {
            //            Writer.Write((UInt16)out_list[p][kk]);
            //        }
            //        if (out_list[p].Count == blockinfo.CompressedSize)
            //        {
            //            Console.WriteLine("符合");
            //        }
            //        else
            //        {
            //            Int64 compressedcount = (Int64)out_list[p].Count - (Int64)blockinfo.CompressedSize;
            //            compressedcount = Math.Abs(compressedcount);
            //            Console.WriteLine("不符合补0-" + compressedcount);
            //            for (Int64 pp = 0; pp < compressedcount; pp++)
            //            {
            //                Writer.Write((UInt16)0);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("不符合补0-" + blockinfo.CompressedSize);
            //        //整块补0
            //        for (Int64 pp = 0; pp < blockinfo.CompressedSize; pp++)
            //        {
            //            Writer.Write((UInt16)0);
            //        }
            //    }
            //    p++;
            //}
            Writer.Close();


        }

        public void CZ1ToPng(string infile)
        {

        }

        public override void FileExport(string infile,string output=null)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length), infile);
            texture.Save(infile + ".png", ImageFormat.Png);
            br.Close();
        }

        public override void FileImport(string name, string output = null)
        {
            PngToCZ1(name);
        }

    }



  




}
