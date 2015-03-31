using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Overlord.Storage
{
    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            {
                return typeof(Guid) == objectType;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Null:
                        return Guid.Empty;
                    case JsonToken.String:
                        string str = reader.Value as string;
                        if (string.IsNullOrEmpty(str))
                        {
                            return Guid.Empty;
                        }
                        else
                        {
                            return str.UrnToGuid();
                        }
                    default:
                        throw new ArgumentException("Invalid token type");
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (Guid.Empty.Equals(value))
                {
                    writer.WriteValue("");
                }
                else
                {
                    var guid = (Guid)value;
		            writer.WriteValue(guid.ToUrn());
                }
            }
        }    
}
