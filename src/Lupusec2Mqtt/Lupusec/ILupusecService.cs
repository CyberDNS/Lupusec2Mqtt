using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;

namespace Lupusec2Mqtt.Lupusec
{
    public interface ILupusecService
    {
        SensorList SensorList { get; }
        RecordList RecordList { get; }
        PowerSwitchList PowerSwitchList { get; }
        PanelCondition PanelCondition { get; }

        Task<SensorList> GetSensorsAsync();
        Task<RecordList> GetRecordsAsync();
        Task<PowerSwitchList> GetPowerSwitches();
        Task<PanelCondition> GetPanelConditionAsync();

        Task PollAllAsync();

        Task<LupusecResponseBody> SetAlarmMode(int area, AlarmMode mode);
        Task<LupusecResponseBody> SetSwitch(string uniqueId, bool onOff);
        Task<LupusecResponseBody> SetCoverPosition(byte area, byte zone, string command);
    }
}