using Lupusec2Mqtt.Lupusec.Dtos;

namespace Lupusec2Mqtt.Lupusec
{
    public class LupusecCache
    {
        public SensorList SensorList { get; private set; }
        public SensorList SensorList2 { get; private set; }

        public RecordList RecordList { get; private set; }

        public PowerSwitchList PowerSwitchList { get; private set; }

        public PanelCondition PanelCondition { get; private set; }

        public void UpdateSensorList(SensorList sensorList) => SensorList = sensorList;
        public void UpdateSensorList2(SensorList sensorList2) => SensorList2 = sensorList2;
        public void UpdateRecordList(RecordList recordList) => RecordList = recordList;
        public void UpdatePowerSwitchList(PowerSwitchList powerSwitchList) => PowerSwitchList = powerSwitchList;
        public void UpdatePanelCondition(PanelCondition panelCondition) => PanelCondition = panelCondition;
    }
}
