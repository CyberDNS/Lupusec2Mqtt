using Lupusec2Mqtt.Lupusec;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public interface IDeviceFactory
    {
        Task<IEnumerable<Device>> GenerateDevicesAsync();
    }
}