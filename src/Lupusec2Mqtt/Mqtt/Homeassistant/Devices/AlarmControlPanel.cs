using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class AlarmControlPanel : Device, IDevice, IStateProvider
    {
        protected override string _component => "alarm_control_panel";

        private readonly PanelCondition _panelCondition;
        private readonly Pcondform _panelConditionForm;



        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("command_topic")]
        public string CommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

        [JsonIgnore]
        public string State => GetState();

        public AlarmControlPanel(IConfiguration configuration, PanelCondition panelCondition, int area)
        : base(configuration)
        {
            _panelCondition = panelCondition;

            switch (area)
            {
                case 1:
                    _panelConditionForm = _panelCondition.forms.pcondform1;
                    break;
                case 2:
                    _panelConditionForm = _panelCondition.forms.pcondform2;
                    break;
            }

            Name = $"Area {area}";
            UniqueId = $"lupusec_alarm_area{area}";
        }

        private string GetState()
        {
            switch (_panelConditionForm.mode)
            {
                case AlarmMode.Disarmed: 
                    return "disarmed";
                case AlarmMode.FullArm: 
                    return "armed_away";
                case AlarmMode.HomeArm1:
                    return "armed_night";
                case AlarmMode.HomeArm2:
                    return "armed_home";
                case AlarmMode.HomeArm3:
                    return "armed_vacation";
                default:
                    return null;
            }
        }
    }
}