using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using LucaSystemTools;



namespace ProtScript.Entity
{

    /// <summary>
    /// 脚本实体
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ScriptEntity
    {
        [JsonProperty("ToolVersion")]
        public uint toolVersion { get; set; } = Program.toolVersion;
        [JsonProperty("ScriptVersion")]
        public int version { get; set; } = 3;
        [JsonProperty("codes")]
        public List<CodeLine> lines { get; set; } = new List<CodeLine>();

        /// <summary>
        /// 使用字符串和类型重建ParamData
        /// 重建value和bytes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ParamData ToParamData(string data, DataType type)
        {
            ParamData param = new ParamData();
            byte[] dataBytes = null;
            switch (type)
            {
                case DataType.Byte:
                case DataType.Byte2:
                case DataType.Byte3:
                case DataType.Byte4:
                    dataBytes = ScriptUtil.Hex2Byte(data);
                    param = new ParamData(type, dataBytes, dataBytes, data);
                    break;
                case DataType.UInt16:
                    var dataUint16 = Convert.ToUInt16(data);
                    dataBytes = BitConverter.GetBytes(dataUint16);
                    param = new ParamData(type, dataUint16, dataBytes, data);
                    break;
                case DataType.Int16:
                    var dataInt16 = Convert.ToInt16(data);
                    dataBytes = BitConverter.GetBytes(dataInt16);
                    param = new ParamData(type, dataInt16, dataBytes, data);
                    break;
                case DataType.UInt32:
                    var dataUint32 = Convert.ToUInt32(data);
                    dataBytes = BitConverter.GetBytes(dataUint32);
                    param = new ParamData(type, dataUint32, dataBytes, data);
                    break;
                case DataType.Position:
                    dataBytes = BitConverter.GetBytes((UInt32)0);
                    param = new ParamData(type, (UInt32)0, dataBytes, data);
                    break;
                case DataType.Int32:
                    var dataInt32 = Convert.ToInt32(data);
                    dataBytes = BitConverter.GetBytes(dataInt32);
                    param = new ParamData(type, dataInt32, dataBytes, data);
                    break;
                case DataType.StringUnicode:
                case DataType.StringSJIS:
                case DataType.StringUTF8:
                case DataType.StringCustom:
                case DataType.LenStringUnicode:
                case DataType.LenStringSJIS:
                case DataType.LenStringUTF8:
                case DataType.LenStringCustom:
                    dataBytes = ToEncodingBytes(data, type);
                    param = new ParamData(type, data, dataBytes, data);
                    break;
                default:
                    break;
            }
            return param;
        }
        /// <summary>
        /// 是否为字符串类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(DataType type)
        {
            switch (type)
            {
                case DataType.StringUnicode:
                case DataType.StringSJIS:
                case DataType.StringUTF8:
                case DataType.StringCustom:
                case DataType.LenStringUnicode:
                case DataType.LenStringSJIS:
                case DataType.LenStringUTF8:
                case DataType.LenStringCustom:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 短字符串转类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataType ToDataType(string type)
        {
            switch (type)
            {
                case "short":
                    return DataType.Int16;
                case "uint":
                    return DataType.UInt32;
                case "int":
                    return DataType.Int32;
                case "$u":
                    return DataType.StringUnicode;
                case "$j":
                    return DataType.StringSJIS;
                case "$8":
                    return DataType.StringUTF8;
                case "$c":
                    return DataType.StringCustom;
                case "&u":
                    return DataType.LenStringUnicode;
                case "&j":
                    return DataType.LenStringSJIS;
                case "&8":
                    return DataType.LenStringUTF8;
                case "&c":
                    return DataType.LenStringCustom;
                case "label":
                    return DataType.Position;
                default:
                    return DataType.Null;
            }
        }
        /// <summary>
        /// 字符串按类型转字节，支持自定义编码表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] ToEncodingBytes(string data, DataType type)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<byte> dataBytes = new List<byte>();
            switch (type)
            {
                case DataType.StringUnicode:
                case DataType.StringSJIS:
                case DataType.StringUTF8:
                case DataType.StringCustom:
                    if (type == DataType.StringUnicode)
                    {
                        dataBytes.AddRange(Encoding.Unicode.GetBytes(data));
                        dataBytes.Add(0x00);
                        dataBytes.Add(0x00);
                    }
                    else if (type == DataType.StringSJIS)
                    {
                        dataBytes.AddRange(Encoding.GetEncoding("Shift-Jis").GetBytes(data));
                        dataBytes.Add(0x00);
                    }
                    else if (type == DataType.StringUTF8)
                    {
                        dataBytes.AddRange(Encoding.UTF8.GetBytes(data));
                        dataBytes.Add(0x00);
                    }
                    else if (type == DataType.StringCustom)
                    {
                        dataBytes.AddRange(CustomEncoding.GetBytes(data));
                        dataBytes.Add(0x00);
                    }
                    break;
                case DataType.LenStringUnicode:
                case DataType.LenStringSJIS:
                case DataType.LenStringUTF8:
                case DataType.LenStringCustom:
                    dataBytes.AddRange(BitConverter.GetBytes((UInt16)data.Length));
                    if (type == DataType.LenStringUnicode)
                    {
                        dataBytes.AddRange(Encoding.Unicode.GetBytes(data));
                    }
                    else if (type == DataType.LenStringSJIS)
                    {
                        dataBytes.AddRange(Encoding.GetEncoding("Shift-Jis").GetBytes(data));
                    }
                    else if (type == DataType.LenStringUTF8)
                    {
                        dataBytes.AddRange(Encoding.UTF8.GetBytes(data));
                    }
                    else if (type == DataType.StringCustom)
                    {
                        dataBytes.AddRange(CustomEncoding.GetBytes(data));
                    }
                    break;
                default:
                    break;
            }
            return dataBytes.ToArray();
        }
    }

}
