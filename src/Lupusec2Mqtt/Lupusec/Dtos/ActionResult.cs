using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class ActionResult
    {
        [JsonProperty("result")]
        public int Result;

        [JsonProperty("message")]
        public string Message;
    }
}
