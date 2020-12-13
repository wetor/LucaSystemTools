using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ProtScript
{
    public class ScriptUtil
    {
        public static byte[] Hex2Byte(string hexString)// 字符串转16进制字节数组
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Substring(0, 2).ToLower() == "0x")
                hexString = hexString.Remove(0, 2);
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public static string Byte2Hex(byte[] bytes, bool space = false, bool head = false)// 字节数组转16进制字符串
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                    if (space && i < bytes.Length - 1) returnStr += " ";
                }
            }
            return (head ? "0x" : "") + returnStr;
        }
        public static string Byte2Hex(byte bytes)// 字节数组转16进制字符串
        {
            return bytes.ToString("X2");
        }
        public static int InitOpcodeDict(string path,
            ref Dictionary<byte, ScriptOpcode> bytesToOpcodeDict,
            ref Dictionary<string, byte> opcodeToBytesDict)//SP CL
        {
            string[] dic;
            int scriptVersion = 3;
            if (File.Exists(path))
            {
                dic = File.ReadAllLines(path);
            }
            else
            {
                throw new Exception("未找到指定游戏");
            }
            bytesToOpcodeDict.Clear();
            opcodeToBytesDict.Clear();
            if (dic.Length > 0)
            {
                if (dic[0].Trim() == ";Ver3")
                {
                    scriptVersion = 3;
                }
                else if (dic[0].Trim() == ";Ver2")
                {
                    scriptVersion = 2;
                }
            }
            int i = 0;
            foreach (string line in dic)
            {
                if (line.TrimStart()[0] == ';')
                {
                    continue;
                }
                bytesToOpcodeDict.Add((byte)i, new ScriptOpcode((byte)i, line.Replace("\r", "")));
                opcodeToBytesDict.Add(bytesToOpcodeDict[(byte)i].opcode, (byte)i);
                i++;
            }
            return scriptVersion;
        }
        /// <summary>
        /// 得到一个对象的克隆
        /// </summary>
        public static object Clone(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return formatter.Deserialize(memoryStream);
        }

    }
}
