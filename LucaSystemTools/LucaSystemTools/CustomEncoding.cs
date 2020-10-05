using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProtScript
{
    public class CustomEncoding
    {
        private static Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();
        public static void LoadTbl(string path)
        {
            int count = 0;
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                if (line.IndexOf("=") < 0 || line.Length <= 5) continue;
                string hex = line.Substring(0, 4);
                string word = line.Substring(5, line.Length - 5);
                if (!dict.ContainsKey(word))
                {
                    dict.Add(word, ScriptUtil.Hex2Byte(hex));
                    count++;
                }
                else
                {
                    Console.WriteLine("重复的汉字！跳过。{0}", line);
                }
            }
            sr.Close();
            Console.WriteLine("已加载自定义编码表，有效字符数：{0}", count);
        }
        public static byte[] GetBytes(string str)
        {
            List<byte> data = new List<byte>();
            foreach (var ch in str)
            {
                if (dict.ContainsKey(ch.ToString()))
                {
                    data.AddRange(dict[ch.ToString()]);
                }
                else
                {
                    throw new Exception("不存在的字符！" + ch);
                }
            }
            return data.ToArray();
        }
    }
}
