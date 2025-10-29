using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class Sensor
    {
        [JsonProperty("area")]
        public byte Area;

        [JsonProperty("zone")]
        public byte Zone;

        [JsonProperty("type")]
        public byte TypeId;

        [JsonProperty("type_f")]
        public string TypeName;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("cond")]
        public string StateText;

        [JsonProperty("cond_ok")]
        public string StateOk;

        [JsonProperty("battery")]
        public string BatteryText;

        [JsonProperty("battery_ok")]
        public byte BatteryOk;

        [JsonProperty("tamper")]
        public string TamperText;

        [JsonProperty("tamper_ok")]
        public byte TamperOk;

        [JsonProperty("bypass")]
        public string Bypassed;

        [JsonProperty("rssi")]
        public string SignalStrength;

        [JsonProperty("resp_mode")]
        public byte[] ResponseMode;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("level")]
        public string Level;

        [JsonProperty("sid")]
        public string SensorId;

        [JsonProperty("su")]
        public byte Su;

        [JsonProperty("alarm_status")]
        public string AlarmStatus;

        [JsonProperty("alarm_status_ex")]
        public bool AlarmStatusEx;

        [JsonProperty("status_ex")]
        public byte StatusEx;
    }
}
