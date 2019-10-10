using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class PowerSwitch
    {
        [JsonProperty("area")]
        public long Area { get; set; }

        [JsonProperty("zone")]
        public long Zone { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("type_f")]
        public string TypeF { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("consumer_id")]
        public long ConsumerId { get; set; }

        [JsonProperty("ammeter")]
        public long Ammeter { get; set; }

        [JsonProperty("always_off")]
        public long AlwaysOff { get; set; }

        [JsonProperty("hue")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Hue { get; set; }

        [JsonProperty("sat")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Sat { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
