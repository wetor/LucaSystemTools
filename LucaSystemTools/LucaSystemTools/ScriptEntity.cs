using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;



namespace ProtScript
{
    /// <summary>
    /// 指令行，信息参数
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
        public override string ToString()
        {
            string retn = "{";
            foreach (var num in data)
            {
                retn += num.ToString() + ",";
            }
            if (count > 0)
            {
                retn = retn.Remove(retn.Length - 1);
            }
            retn += "}";
            return retn;
        }
    }
    /// <summary>
    /// 指令，参数数据结构
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ParamData
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public DataType type { get; set; }
        [JsonProperty("value")]
        [JsonConverter(typeof(BytesConverter))]
        public object valueOp
        {
            get { return this.value; }
            set
            {
                this.value = value;
                if ((int)type <= 4)
                {
                    this.valueString = ScriptUtil.Byte2Hex((byte[])value, false, true);
                }
                else {
                    this.valueString = value.ToString();
                }
            }
        }
        public object value;
        public string valueString { get; set; }
        public byte[] bytes { get; set; }
        public ParamData()
        {

        }
        public ParamData(DataType type, object value, string valueString)
        {
            this.type = type;
            this.value = value;
            this.valueString = valueString;
        }
        public ParamData(DataType type, object value, byte[] bytes, string valueString)
        {
            this.type = type;
            this.value = value;
            this.bytes = bytes;
            this.valueString = valueString;
        }
        public void setValue(object value)
        {
            this.value = value;
            this.valueString = value.ToString();
        }
    }

    /// <summary>
    /// 指令行
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CodeLine
    {
        // 行号
        public int index { get; set; }
        // 所在脚本位置
        public int position { get; set; }
        // opcode
        public byte opcodeIndex { get; set; }
        [JsonProperty]
        public string opcode { get; set; }

        // 资源引用信息（待定）
        [JsonProperty]
        public CodeInfo info { get; set; }
        // 参数的已知类型
        public ParamType[] paramTypes { get; set; }
        // 实际参数列表
        [JsonProperty]
        public List<ParamData> paramDatas { get; set; }
        // 是否存在Position参数
        [JsonProperty]
        public bool isGoto { get; set; } = false;
        public int gotoParamIndex { get; set; }
        // 是否为跳转目标
        [JsonProperty]
        public bool isTarget { get; set; } = false;
        [JsonProperty]
        public int targetId { get; set; }

        public CodeLine(int index, int position)
        {
            this.index = index;
            this.position = position;

        }
        /// <summary>
        /// 此语句包含跳转，设置跳转参数的下标
        /// </summary>
        /// <param name="paramIndex"></param>
        public void setGoto(int paramIndex)
        {
            isGoto = true;
            gotoParamIndex = paramIndex;
        }
        /// <summary>
        /// 此语句为跳转目标，设置唯一id
        /// </summary>
        /// <param name="id"></param>
        public void setTarget(int id)
        {
            isTarget = true;
            targetId = id;
        }
        public ParamData getGoto()
        {
            return paramDatas[gotoParamIndex];
        }
        /// <summary>
        /// 修改跳转参数值
        /// </summary>
        /// <param name="value"></param>
        public void setGotoValue(int value)
        {
            paramDatas[gotoParamIndex].setValue((object)value);
        }
        public override string ToString()
        {
            string retn = "";
            foreach (var param in paramDatas)
            {
                if (ScriptEntity.IsString(param.type))
                {
                    retn += "," + "\"" + param.valueString + "\"";
                }
                else
                {
                    retn += "," + param.valueString;
                }
                
            }
            return (isTarget ? "[" + targetId + "]>>" : "") + opcode + "(" + info.ToString() + retn + ")" + (isGoto ? ">>" : "");
        }
    }
    /// <summary>
    /// 脚本实体
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ScriptEntity
    {
        [JsonProperty]
        public int version { get; set; } = 3;

        [JsonProperty("codes")]
        public List<CodeLine> lines { get; set; } = new List<CodeLine>();

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
                case DataType.Position:
                    var dataUint32 = Convert.ToUInt16(data);
                    dataBytes = BitConverter.GetBytes(dataUint32);
                    param = new ParamData(type, dataUint32, dataBytes, data);
                    break;
                case DataType.Int32:
                    var dataInt32 = Convert.ToInt16(data);
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
                    var dataStr = data.Replace(@"{\n}", "\n");
                    dataBytes = ToEncodingBytes(dataStr, type);
                    param = new ParamData(type, dataStr, dataBytes, data);
                    break;
                default:
                    break;
            }
            return param;
        }
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
        public static byte[] ToEncodingBytes(string data, DataType type)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<byte> dataBytes = new List<byte>();
            switch (type)
            {
                case DataType.StringUnicode:
                case DataType.StringSJIS:
                case DataType.StringUTF8:
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
    public class BytesConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            if(value.GetType().Name == "Byte[]")
            {
                byte[] bytes = ((byte[])value);
                if (bytes.Length == 0)
                {
                    writer.WriteNull();
                    return;
                }
                writer.WriteValue(ScriptUtil.Byte2Hex(bytes, false, true));
            }
            else
            {
                writer.WriteValue(value);
            }
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType == null)
                {
                    throw new Exception(string.Format("不能转换null value to {0}.", objectType));
                }
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                string hex = (string)reader.Value;
                if (hex.Length >= 4 && hex.Substring(0, 2).Equals("0x", StringComparison.OrdinalIgnoreCase)) 
                {
                    return ScriptUtil.Hex2Byte(hex);
                }
            }

            return reader.Value;

        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.Name == "Byte[]")
                return true;
            else
                return false;
        }
    }
}
