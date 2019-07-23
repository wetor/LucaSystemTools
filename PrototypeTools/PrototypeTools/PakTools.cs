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
        public static Entry[] Unpack(Stream Packget, string coding = "UTF-8")
        {
            var Files = new Entry[0];
            PAKHeader Header = new PAKHeader();
            StructReader Reader = new StructReader(Packget, Encoding: Encoding.UTF8);
            Reader.ReadStruct(ref Header);
            Reader.Seek(0x24, SeekOrigin.Begin);

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
                    Debug.WriteLine("{0}  {1}", Names[i], i);
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
