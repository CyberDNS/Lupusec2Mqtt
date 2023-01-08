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

        private string _temporaryState = null;
        private DateTime _lastCommand = DateTime.MinValue;
        private readonly TimeSpan _temporaryTimeout = TimeSpan.FromSeconds(4);

        public Switch(PowerSwitch powerSwitch)
        {
            DeclareStaticValue("name", powerSwitch.Name);
            DeclareStaticValue("unique_id", powerSwitch.Id);

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/state", GetState);

            DeclareCommand("command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/set", ExecuteCommand);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            if (DateTime.UtcNow - _lastCommand > _temporaryTimeout)
            {
                var powerSwitch = lupusecService.PowerSwitchList.PowerSwitches.Single(s => s.Id == GetStaticValue<string>("unique_id"));
                var result = powerSwitch.Status.Contains("{WEB_MSG_PSS_ON}") ? "ON" : "OFF";

                return Task.FromResult(result);
            }

            return Task.FromResult(_temporaryState);
        }

        public async Task ExecuteCommand(ILogger logger, ILupusecService lupusecService, string command)
        {
            _temporaryState = command.Equals("on", StringComparison.OrdinalIgnoreCase) ? "ON" : "OFF";
            await lupusecService.SetSwitch(GetStaticValue<string>("unique_id"), command.Equals("on", StringComparison.OrdinalIgnoreCase));
            _lastCommand = DateTime.UtcNow;
        }
    }
}
