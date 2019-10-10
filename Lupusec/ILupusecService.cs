using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;

namespace Lupusec2Mqtt.Lupusec
{
    public interface ILupusecService
    {
        Task<SensorList> GetSensorsAsync();
    }
}