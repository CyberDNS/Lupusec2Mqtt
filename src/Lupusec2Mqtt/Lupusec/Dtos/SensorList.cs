using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class SensorList
    {
        [JsonProperty("senrows")]
        public IList<Sensor> Sensors { get; set; } = new List<Sensor>();
    }
}
