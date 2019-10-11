using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class AlarmControlPanel : Device, IDevice, IStateProvider
    {
        protected override string _component => "alarm_control_panel";


        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("command_topic")]
        public string CommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

        [JsonIgnore]
        public string State => GetState();

        public AlarmControlPanel(IConfiguration configuration, PanelCondition panelCondition, int area)
        : base(configuration)
        {
            Name = $"Area {area}";
            UniqueId = $"lupusec_alarm_area{area}";
        }

        private string GetState()
        {
            // switch (_sensor.TypeId)
            // {
            //     case 4: // Opener contact
            //         return _sensor.Status == "{WEB_MSG_DC_OPEN}" ? "ON" : "OFF";
            //     case 9: // Motion detector
            //         return "Off";
            //     case 11: // Smoke detector
            //         return _sensor.Status == "{RPT_CID_111}" ? "ON" : "OFF";
            //     case 5: // Water detector
            //         return "Off";
            //     default:
            //         return null;
            // }

            return "DISARMED";
        }
    }
}