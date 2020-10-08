using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucaSystem
{
    //from http://rosettacode.org/wiki/LZW_compression#C.23
    public class LzwUtil
    {
        //island flower start with 0
        public static string Decompress(List<int> compressed)
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

        public static List<int> Compress(string uncompressed)
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

        public static List<List<int>> Compress(IEnumerator<byte> ie,uint maxcount)
        {
            List<List<int>> out_list = new List<List<int>>();
            string lastw = "";
            int lastdicw ;
            while (true)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                int count = 0;
                for (int i = 0; i < 256; i++)
                    dictionary.Add(((char)i).ToString(), i);
                dictionary.Add(((char)256).ToString(), 0);
                string w = string.Empty;
                if (lastw != string.Empty)
                {
                    w = lastw;
                }
                List<int> compressed = new List<int>();
                while (ie.MoveNext())
                {
                    char c = (char)ie.Current;
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

                    count++;

                    if (compressed.Count == maxcount)
                    {
                        break;
                    }

                }
                Console.WriteLine("Stream End");

               if (!string.IsNullOrEmpty(w))
                {
                    lastw = w;
                }
                else
                {
                    lastw = "";
                }


                if (compressed.Count == 0)
                {
                    if (!string.IsNullOrEmpty(w))
                    {
                        foreach (char c in w)
                        {
                            out_list[out_list.Count - 1].Add(dictionary[c.ToString()]);
                        }
                    }
                    break;
                }
                else
                {
                    out_list.Add(compressed);
                    Console.WriteLine("add compressed count :" + out_list.Count);
                    Console.WriteLine(compressed.Count);
                    Console.WriteLine(LzwUtil.Decompress(compressed).Length);

                }
            }

            return out_list;
        }

        //air cl rewrite start with 256
        public static string Decompress2(List<int> compressed)
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

    }
}
