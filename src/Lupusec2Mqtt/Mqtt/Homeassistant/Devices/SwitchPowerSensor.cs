using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class SwitchPowerSensor : Device
    {
        private readonly string _id;

        public override string Component => "sensor";

        public SwitchPowerSensor(PowerSwitch powerSwitch)
        {
            _id = powerSwitch.Id;

            DeclareStaticValue("name", $"{powerSwitch.Name} - Power");
            DeclareStaticValue("unique_id", $"{powerSwitch.Id}_power");
            DeclareStaticValue("unit_of_measurement", "W");

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/state", GetState);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var sensor = lupusecService.PowerSwitchList.PowerSwitches.Single(s => s.Id == _id);
            var match = Regex.Match(sensor.Status, @"{WEB_MSG_PSM_POWER}\s*(?'power'\d+\.?\d*)");

            if (match.Success) { return Task.FromResult(match.Groups["power"].Value); }
            return Task.FromResult("0");
        }
    }
}
