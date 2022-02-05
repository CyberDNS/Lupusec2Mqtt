namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public enum AlarmMode : byte
    {
        Disarmed = 0,
        FullArm = 1,
        HomeArm1 = 2,
        HomeArm2 = 3,
        HomeArm3 = 4

    }

        public enum AlarmModeAction : byte
    {
        DISARM = 0,
        ARM_AWAY = 1,
        ARM_NIGHT = 2,
        ARM_HOME = 3,
        ARM_VACATION = 4
    }
}