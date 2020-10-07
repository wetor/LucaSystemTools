using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtScript.Entity
{
    /// <summary>
    /// 指令行，信息参数：行数（推测）、文本序号、语音序号等
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CodeInfo
    {
        [JsonProperty]
        public int count { get; set; }
        [JsonProperty]
        public UInt16[] data { get; set; }

        public CodeInfo(int count)
        {
            this.count = count;
            data = null;
        }
        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)count);
            foreach (var num in data)
            {
                bytes.AddRange(BitConverter.GetBytes(num));
            }
            return bytes.ToArray();
        }
        public override string ToString()
        {
            string retn = "{";
            if (data != null)
            {
                foreach (var num in data)
                {
                    retn += num.ToString() + ", ";
                }
                if (count > 0)
                {
                    retn = retn.Remove(retn.Length - 2);
                }
            }
            retn += "}";
            return retn;
        }
    }
}
