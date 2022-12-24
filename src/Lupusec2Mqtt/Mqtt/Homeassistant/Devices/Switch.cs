using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Xml;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Switch : Device
    {
        public override string Component => "switch";

        public Switch(PowerSwitch powerSwitch)
        {
            DeclareStaticValue("name", powerSwitch.Name);
            DeclareStaticValue("unique_id", powerSwitch.Id);

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/state", GetState);

            DeclareCommand("command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/set", ExecuteCommand);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var powerSwitch = lupusecService.PowerSwitchList.PowerSwitches.Single(s => s.Id == GetStaticValue("unique_id"));
            var result = powerSwitch.Status.Contains("{WEB_MSG_PSS_ON}") ? "ON" : "OFF";

            return Task.FromResult(result);
        }

        public async Task ExecuteCommand(ILogger logger, ILupusecService lupusecService, string command)
        {
            await lupusecService.SetSwitch(GetStaticValue("unique_id"), command.Equals("on", StringComparison.OrdinalIgnoreCase));
        }
    }
}
