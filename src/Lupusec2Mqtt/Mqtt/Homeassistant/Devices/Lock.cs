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
    public class Lock : Device
    {
        public override string Component => "lock";

        public Lock(PowerSwitch powerSwitch)
        {
            DeclareStaticValue("name", powerSwitch.Name);
            DeclareStaticValue("unique_id", powerSwitch.Id);

            DeclareLupusXT1PlusDevice();

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/state", GetState);

            DeclareCommand("command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/set", ExecuteCommand);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var powerSwitch = lupusecService.PowerSwitchList.PowerSwitches.Single(s => s.Id == GetStaticValue<string>("unique_id"));
            var result = powerSwitch.Status.Contains("{WEB_MSG_DL_LOCKED}") ? "LOCKED" : "UNLOCKED";

            return Task.FromResult(result);
        }

        public async Task ExecuteCommand(ILogger logger, ILupusecService lupusecService, string command)
        {
            await lupusecService.SetSwitch(GetStaticValue<string>("unique_id"), command.Equals("LOCK", StringComparison.OrdinalIgnoreCase));
        }
    }
}
