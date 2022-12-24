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
    public class SwitchEnergySensor : Device
    {
        private readonly string _id;

        public override string Component => "sensor";

        public SwitchEnergySensor(PowerSwitch powerSwitch)
        {
            _id = powerSwitch.Id;

            DeclareStaticValue("name", $"{powerSwitch.Name} - Energy");
            DeclareStaticValue("unique_id", $"{powerSwitch.Id}_energy");
            DeclareStaticValue("unit_of_measurement", "kWh");

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/state", GetState);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var sensor = lupusecService.PowerSwitchList.PowerSwitches.Single(s => s.Id == _id);
            var match = Regex.Match(sensor.Status, @"{WEB_MSG_POWER_METER_ENERGY}\s*(?'energy'\d+\.?\d*)");

            if (match.Success) { return Task.FromResult(match.Groups["energy"].Value); }
            return Task.FromResult("0");
        }
    }
}
