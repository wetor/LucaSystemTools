using AdvancedBinary;
using LucaSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace LucaSystemTools
{
    public class CZParserBase: AbstractFileParser
    {
        public override void FileExport(string path, string outpath = null)
        {
            throw new NotImplementedException();
        }

        public override void FileImport(string path, string outpath = null)
        {
            throw new NotImplementedException();
        }

        public class CZOutputInfo
        {
            public uint TotalRawSize;
            public uint TotalCompressedSize;
            public uint filecount;
            public List<CZBlockInfo> blockinfo=new List<CZBlockInfo>();
        }

        public class CZBlockInfo
        {
            public int BlockIndex;
            public uint RawSize;
            public uint CompressedSize;
        }

        public IEnumerable<byte> Decompress(StructReader Reader,string filename="")
        {
            List<byte> output = new List<byte>();
            uint fileCount = Reader.ReadUInt32();
            Dictionary<int, uint> rawSizeList = new Dictionary<int, uint>();
            Dictionary<int, uint> compressedSizeList = new Dictionary<int, uint>();
            CZOutputInfo czOutputInfo = new CZOutputInfo();
            czOutputInfo.filecount = fileCount;
            czOutputInfo.TotalRawSize = 0;
            czOutputInfo.TotalCompressedSize = 0;
            for (int i = 0; i < fileCount; i++)
            {
                uint fileCompressedSize = Reader.ReadUInt32();
                uint fileRawSize = Reader.ReadUInt32();
                rawSizeList.Add(i, fileRawSize);
                compressedSizeList.Add(i, fileCompressedSize);
                czOutputInfo.TotalRawSize += fileRawSize;
                czOutputInfo.TotalCompressedSize += fileCompressedSize;
                czOutputInfo.blockinfo.Add(new CZBlockInfo() {BlockIndex=i, RawSize = fileRawSize, CompressedSize = fileCompressedSize });
            }
            if (!string.IsNullOrEmpty(filename))
            {
                File.WriteAllText(filename+".json", JsonConvert.SerializeObject(czOutputInfo, Formatting.Indented));
            }

            for (int i = 0; i < fileCount; i++)
            {
                List<int> lmzBytes = new List<int>();
                int totalcount = (int)compressedSizeList[i];
                for (int j = 0; j < totalcount; j++)
                {
                    lmzBytes.Add(Reader.ReadUInt16());
                }

                //debug====
                //List<byte> bytes = new List<byte>();
                //foreach (var item in lmzBytes)
                //{
                //    UInt16 bb = (UInt16)item;
                //    bytes.Add(BitConverter.GetBytes(bb)[0]);
                //    bytes.Add(BitConverter.GetBytes(bb)[1]);
                //}
                //File.WriteAllBytes(i + "lzw.bin", bytes.ToArray());
                //List<byte> list = new List<byte>();


                string str = LzwUtil.Decompress(lmzBytes);
                foreach (var c in str)
                {
                    output.Add((byte)c);
                    //list.Add((byte)c);
                }

                //debug====
                //File.WriteAllBytes(i + "lzwde.bin", list.ToArray());
            }
            return output;
        }
    }
}
