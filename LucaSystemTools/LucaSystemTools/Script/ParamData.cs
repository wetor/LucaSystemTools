using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtScript.Entity
{
    /// <summary>
    /// 指令，参数数据结构
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ParamData
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public DataType type { get; set; }
        /// <summary>
        /// 设置value的set
        /// </summary>
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
                else
                {
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
    }

}
