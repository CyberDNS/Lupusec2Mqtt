using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class RecordList
    {
        [JsonPropertyName("logrows")]
        public List<Logrow> Logrows { get; set; } = new List<Logrow>();
    }

    public class Logrow
    {
        [JsonPropertyName("uid")]
        public int Uid { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        public DateTime UtcDateTime
        {
            get
            {
                double timestamp = Convert.ToDouble(Time);
                DateTimeOffset dateTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
                return dateTime.AddSeconds(timestamp).UtcDateTime;
            }
        }

        [JsonPropertyName("area")]
        public string Area { get; set; }

        [JsonPropertyName("zone")]
        public string Zone { get; set; }

        [JsonPropertyName("sid")]
        public string Sid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type_f")]
        public string TypeF { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("mark_read")]
        public int MarkRead { get; set; }
    }

 
}
