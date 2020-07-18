using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class PowerSwitchList
    {
        [JsonProperty("pssrows")]
        public PowerSwitch[] PowerSwitches { get; set; }
    }
}
