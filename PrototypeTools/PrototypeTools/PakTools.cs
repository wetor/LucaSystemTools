using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ProtPak
{
    public class PAKManager
    {
        public static void Unpack(string file,string name_coding = "UTF-8")
        {
            string OutDir = file + "_unpacked\\";
            Stream Packget = new StreamReader(file).BaseStream;
            uint header_len;
            var Files = PAKManager.Unpack(Packget, out header_len, name_coding);
            FileStream fs = new FileStream(file + ".pakhead", FileMode.Create);
            Packget.Seek(0, SeekOrigin.Begin);
            byte[] head = new byte[header_len];
            Packget.Read(head, 0, head.Length);
            fs.Write(head, 0, head.Length);

            if (Directory.Exists(OutDir))
                Directory.Delete(OutDir, true);
            if (!Directory.Exists(OutDir))
                Directory.CreateDirectory(OutDir);
            int index = 0;
            foreach (var File in Files)
            {
                string FN = OutDir + File.FileName;
                int ID = 2;
                while (System.IO.File.Exists(FN))
                    FN = OutDir + File.FileName + "." + ID++;

                Stream Output = new StreamWriter(FN).BaseStream;
                File.Content.CopyTo(Output);
                Output.Close();
                Console.WriteLine("{0} {1} {2}", index++, File.FileName, File.Length);
 
            }
            fs.Close();
            Packget.Close();
        }
        public static Entry[] Unpack(Stream Packget, out uint data_pos , string coding = "UTF-8")
        {
            var Files = new Entry[0];
            PAKHeader Header = new PAKHeader();
            StructReader Reader = new StructReader(Packget, Encoding: Encoding.GetEncoding(coding));
            Reader.ReadStruct(ref Header);
            Reader.Seek(0x24, SeekOrigin.Begin);
            data_pos = Header.HeaderLength;
            //Search for the First Offset
            while (Reader.PeekInt() != Header.HeaderLength / Header.BlockSize)
                Reader.Seek(0x4, SeekOrigin.Current);


            bool Named = (Header.Flags & (uint)PackgetFlags.NamedFiles) != 0;
            string[] Names = new string[Header.FileCount];
            if (Named)
            {
                long Off = Reader.BaseStream.Position;
                Reader.Seek(-0x4, SeekOrigin.Current);
                Reader.Seek(Reader.ReadUInt32(), SeekOrigin.Begin);

                for (uint i = 0; i < Names.Length; i++)
                {
                    List<byte> strArr = new List<byte> { };
                    byte tmp = Reader.ReadByte();
                    while (tmp != 0x00)
                    {
                        strArr.Add(tmp);
                        tmp = Reader.ReadByte();
                    }

                    Names[i] = Encoding.GetEncoding(coding).GetString(strArr.ToArray());
                    //Console.WriteLine("{0}  {1}", Names[i], i);
                    //Names[i] = Reader.ReadString(StringStyle.CString);
                }
                Reader.Seek(Off, SeekOrigin.Begin);
            }
            else
            {
                for (uint i = 0; i < Names.Length; i++)
                {
                    Names[i] = i.ToString("X8");
                }
            }


            for (uint i = 0; i < Header.FileCount; i++)
            {
                var File = new Entry();
                Reader.ReadStruct(ref File);

                File.Offset *= Header.BlockSize;
                File.FileName = Names[Files.LongLength];
                File.Content = new VirtStream(Packget, File.Offset, File.Length);

                AppendArray(ref Files, File);
            }

            return Files;
        }

        public static void AppendArray<T>(ref T[] Arr, T Val)
        {
            T[] NArr = new T[Arr.Length + 1];
            Arr.CopyTo(NArr, 0);
            NArr[Arr.Length] = Val;
            Arr = NArr;
        }
    }

    public enum PackgetFlags : uint
    {
        NamedFiles = 1 << 9
    }

    public struct PAKHeader
    {
        public uint HeaderLength;
        public uint FileCount;
        uint Unk1;//Flags?
        public uint BlockSize;
        uint Unk2;
        uint Unk3;
        uint Unk4;
        uint Unk5;
        public uint Flags;
        //More Random Unk Data, I will Ignore.
    }

    public struct Entry
    {
        public uint Offset;
        public uint Length;

        [Ignore]
        public Stream Content;

        [Ignore]
        public string FileName;
    }
}
