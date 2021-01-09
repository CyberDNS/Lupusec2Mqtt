using Lupusec2Mqtt.Lupusec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public interface ISettable : IDevice, IStateProvider
    {
        string CommandTopic { get; }

        void SetState(string state, ILupusecService lupusecService);
    }
}
