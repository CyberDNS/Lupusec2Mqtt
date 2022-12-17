using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;

namespace Lupusec2Mqtt.Lupusec
{
    public interface ILupusecService
    {
        Task<SensorList> GetSensorsAsync();
        Task<RecordList> GetRecordsAsync();
        Task<PowerSwitchList> GetPowerSwitches();

        Task<PanelCondition> GetPanelConditionAsync();

        Task<ActionResult> SetAlarmMode(int area, AlarmMode mode);

        Task<ActionResult> SetSwitch(string uniqueId, bool onOff);

        Task<ActionResult> SetCoverPosition(byte area, byte zone, string command);
    }
}