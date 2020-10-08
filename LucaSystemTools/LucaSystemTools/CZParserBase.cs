using AdvancedBinary;
using LucaSystem;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IEnumerable<byte> Decompress(StructReader Reader)
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
                string str = LzwUtil.Decompress(lmzBytes);
                foreach (var c in str)
                {
                    output.Add((byte)c);
                }
            }
            return output;
        }
    }
}
