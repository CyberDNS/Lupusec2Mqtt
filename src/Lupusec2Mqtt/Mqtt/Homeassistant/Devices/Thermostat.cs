using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Thermostat : Device
    {
        public override string Component => "climate";

        private const string mode_off = "off";
        private const string mode_auto = "auto";
        private const string mode_heat = "heat";

        public Thermostat(Sensor thermostat)
        {
            DeclareStaticValue("name", thermostat.Name);
            DeclareStaticValue("unique_id", thermostat.SensorId);                        
            DeclareStaticValue("temp_step", "0.1");            
                
            DeclareStaticValue("modes", new List<string>{mode_off, mode_heat, mode_auto});                       

            DeclareQuery("current_temperature_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/current_temperature", GetCurrentTemp);
            DeclareQuery("temperature_state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/temperature_state", GetDestinationTemp);      
            DeclareQuery("mode_state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/mode_state", GetModeState);                  

            DeclareCommand("mode_command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/mode_command_topic", SetMode);
            DeclareCommand("temperature_command_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/temperature_command_topic", SetDestinationTemperature);
        }

        public Task<string> GetCurrentTemp(ILogger logger, ILupusecService lupusecService) {
            var thermostat = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));
            var match = Regex.Match(thermostat.Status, @"{WEB_MSG_TS_DEGREE}\s*(?'temperature'\d+\.?\d*)");

            if (match.Success) { return Task.FromResult(match.Groups["temperature"].Value); }

            return Task.FromResult("0");
        }

        public Task<string> GetDestinationTemp(ILogger logger, ILupusecService lupusecService) {
            var thermostat = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));
            var match = Regex.Match(thermostat.Status, @"{WEB_MSG_TRV_SETPOINT}\s*(?'temperature'\d+\.?\d*)");

            if (match.Success) { 
                float temperature = 0f;
                if(float.TryParse(match.Groups["temperature"].Value, out temperature)) {
                    // if the result returns 17.79 than HA will show 17.8 but the thermostat shows 17.7.
                    return Task.FromResult(Math.Round(temperature, 1, MidpointRounding.ToZero).ToString());
                }                
            }

            return Task.FromResult("0");
        }

        private async Task SetMode(ILogger logger, ILupusecService lupusecService, string mode)
        {
            string unique_id = GetStaticValue<string>("unique_id");
            try
            {
                logger.LogInformation("Set device {Device} to {Mode}", unique_id, mode);
                
                ThermostateMode thermostateMode = ThermostateMode.Off;
                if(mode.Equals(mode_heat, StringComparison.OrdinalIgnoreCase)) {
                    thermostateMode = ThermostateMode.Heat;
                }
                else if(mode.Equals(mode_auto, StringComparison.OrdinalIgnoreCase)) {
                    thermostateMode = ThermostateMode.Auto;
                }

                await lupusecService.SetThermostatMode(unique_id, thermostateMode);                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while setting thermostat mode for {Area} set to {Mode}", unique_id, mode);
            }
        }


        private async Task SetDestinationTemperature(ILogger logger, ILupusecService lupusecService, string destinationTemperature)
        {
            string unique_id = GetStaticValue<string>("unique_id");
            try
            {
                float dest = float.Parse(destinationTemperature);

                logger.LogInformation("Set device {device} to {destinationTemperature} temperature", unique_id, destinationTemperature);
                
                await lupusecService.SetThermostatTemperature(unique_id, (int)(dest * 100));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while setting thermostat mode for {device} set to {destinationTemperature}", unique_id, destinationTemperature);
            }
        }

        public Task<string> GetModeState(ILogger logger, ILupusecService lupusecService)
        {
            var thermostat = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue<string>("unique_id"));
            if(thermostat.Status.Contains("{WEB_MSG_TRV_OFF}")) {
                return Task.FromResult(mode_off);
            }

            if(thermostat.Status.Contains("{WEB_MSG_TRV_MANUAL}")) {
                return Task.FromResult(mode_heat);
            }
                   
            if(thermostat.Status.Contains("{WEB_MSG_TRV_AUTO}")) {
                return Task.FromResult(mode_auto);
            }

            return Task.FromResult(string.Empty);
        }
    }
}