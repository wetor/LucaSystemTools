using System;
using System.Collections.Generic;
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

    }
}
