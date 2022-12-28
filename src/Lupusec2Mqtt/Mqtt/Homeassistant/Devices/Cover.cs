using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Cover : Device
    {
        public override string Component => "cover";

        public Cover(Sensor shutter)
        {
            DeclareStaticValue("name", shutter.Name);
            DeclareStaticValue("unique_id", shutter.SensorId);
            DeclareStaticValue("position_open", 100);
            DeclareStaticValue("position_closed", 0);

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/state", GetState);
            DeclareQuery("position_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/position", GetPosition);

            DeclareCommand("command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/set", ExecuteCommand);
            DeclareCommand("set_position_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/set-pos", SetPosition);

        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var shutter = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));
            var result = shutter.Level <= 0 ? "closed" : "open";

            return Task.FromResult(result);
        }

        private Task<string> GetPosition(ILogger logger, ILupusecService lupusecService)
        {
            var shutter = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));
            var result = shutter.Level.ToString();

            return Task.FromResult(result);
        }

        public async Task ExecuteCommand(ILogger logger, ILupusecService lupusecService, string command)
        {
            var shutter = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));

            var lupusCommand = string.Empty;
            if (command.Equals("OPEN")) { lupusCommand = "on"; }
            else if (command.Equals("CLOSE")) { lupusCommand = "off"; }
            else if (command.Equals("STOP")) { lupusCommand = "stop"; }

            await lupusecService.SetCoverPosition(shutter.Area, shutter.Zone, lupusCommand);
        }

        public async Task SetPosition(ILogger logger, ILupusecService lupusecService, string command)
        {
            var shutter = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));

            await lupusecService.SetCoverPosition(shutter.Area, shutter.Zone, command);
        }
    }
}




//public class Cover : Device, IDevice, IStateProvider, IPositionProvider
//{
//    protected readonly Sensor _shutter;

//    [JsonProperty("state_topic")]
//    public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

//    [JsonProperty("position_topic")]
//    public string PositionTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/position");

//    [JsonProperty("position_open")]
//    public int PositionOpen => 100;

//    [JsonProperty("position_closed")]
//    public int PositionClosed => 0;

//    [JsonProperty("set_position_topic")]
//    public string PositionCommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set-pos");

//    [JsonProperty("command_topic")]
//    public string StateCommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

//    protected override string _component => "cover";

//    [JsonIgnore]
//    public string State => GetState();

//    [JsonIgnore]
//    public int Position => GetPosition();

//    public Cover(IConfiguration configuration, Sensor shutter)
//        : base(configuration)
//    {
//        _shutter = shutter;

//        UniqueId = shutter.SensorId;
//        Name = GetValue(nameof(Name), shutter.Name);

//        Commands = new Command[]
//        {
//                new Command(StateCommandTopic, ExecuteCommand),
//                new Command(PositionCommandTopic, ExecuteCommand),
//        };

//    }

//    private string GetState()
//    {
//        if (_shutter.Level <= 0) { return "closed"; }
//        else { return "open"; }
//    }

//    private int GetPosition()
//    {
//        return _shutter.Level;
//    }

//    public void ExecuteCommand(string command, ILupusecService lupusecService)
//    {
//        if (command.Equals("OPEN")) { command = "on"; }
//        else if (command.Equals("CLOSE")) { command = "off"; }
//        else if (command.Equals("STOP")) { command = "stop"; }

//        lupusecService.SetCoverPosition(_shutter.Area, _shutter.Zone, command);
//    }
//}
