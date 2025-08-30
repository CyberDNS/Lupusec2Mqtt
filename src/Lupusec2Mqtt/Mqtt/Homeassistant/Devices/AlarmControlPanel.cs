using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class AlarmControlPanel : Device
    {
        private readonly int _area;

        public override string Component => "alarm_control_panel";

        public AlarmControlPanel(IConfiguration configuration, Pcondform panelForm, int area)
        {
            _area = area;

            DeclareStaticValue("name", $"Area {area}");
            DeclareStaticValue("unique_id", $"lupusec_alarm_area{area}");

            DeclareLupusXT1PlusDevice();

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/state", GetState);

            DeclareCommand("command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/set", SetAlarm);
        }

        private Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            Pcondform pcondform = null;
            if (_area == 1) { pcondform = lupusecService.PanelCondition.forms.pcondform1; }
            else if (_area == 2) { pcondform = lupusecService.PanelCondition.forms.pcondform2; }

            switch (pcondform.mode)
            {
                case AlarmMode.Disarmed:
                    return Task.FromResult("disarmed");
                case AlarmMode.FullArm:
                    return Task.FromResult("armed_away");
                case AlarmMode.HomeArm1:
                    return Task.FromResult("armed_night");
                case AlarmMode.HomeArm2:
                    return Task.FromResult("armed_home");
                case AlarmMode.HomeArm3:
                    return Task.FromResult("armed_vacation");
                default:
                    logger.LogError("Unknown mode {Mode} provided for device {Device}", pcondform.mode, this);
                    return Task.FromResult((string)null);
            }
        }

        private async Task SetAlarm(ILogger logger, ILupusecService lupusecService, string mode)
        {
            try
            {
                logger.LogInformation("Area {Area} set to {Mode}", _area, mode);
                await lupusecService.SetAlarmMode(_area, (AlarmMode)Enum.Parse(typeof(AlarmModeAction), mode));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while setting alarm mode to Area {Area} set to {Mode}", _area, mode);
            }
        }

    }
}
