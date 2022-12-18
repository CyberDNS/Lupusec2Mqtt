using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.DevicesNew
{
    public class AlarmControlPanelFactory : DeviceFactory
    {
        public AlarmControlPanelFactory(IConfiguration configuration, ILupusecService lupusecService)
            :base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var panelCondition = _lupusecService.PanelCondition;

            var alarmControlPanels = new AlarmControlPanel[]
            {
                new AlarmControlPanel(_configuration, panelCondition.forms.pcondform1, 1),
                new AlarmControlPanel(_configuration, panelCondition.forms.pcondform2, 2),
            };

            return Task.FromResult<IEnumerable<Device>>(alarmControlPanels);
        }
    }
}
