using Lupusec2Mqtt.Lupusec;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public abstract class DeviceFactory : IDeviceFactory
    {
        protected IConfiguration _configuration { get; }
        protected ILupusecService _lupusecService { get; }

        public DeviceFactory(IConfiguration configuration, ILupusecService lupusecService)
        {
            _configuration = configuration;
            _lupusecService = lupusecService;
        }

        public abstract Task<IEnumerable<Device>> GenerateDevicesAsync();
    }
}
