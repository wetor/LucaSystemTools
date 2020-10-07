using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtScript
{
    public class BytesConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            if (value.GetType().Name == "Byte[]")
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
