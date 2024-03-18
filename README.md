# Lupusec2Mqtt

Make your Lupusec XT2 (and compatible) Alarm System available on an MQTT Broker (compatible with Home Assistant).

### Tested with
- XT1 Plus
- XT2
- XT2 Plus

### What is already working

- Alarm panel (config, state, set)
- Burglar Alarm state (config, state)
- Open/Close contacts (config, state)
- Movement detector (config, state)
- Power switch (config, state, set, power, energy)
- Lock (config, state, set)
- Temperature/Humidity sensor (config, state)
- Smoke detector (config, state)
- Vibration detector (config, state)
- Cover (config, state, position, set, set position)

## Installation

## Home Assistant Addon

It is available as Addon that can be directly installed by the Supervisor tab.  
The repository is: https://github.com/CyberDNS/hassio-addons-repository

## Manual Docker Configuration

### Alternative 1: Environment variables
For users who want to run Lupusec2Mqtt as a standalone Docker container (not as a Home Assistant addon), you can use the following `docker run` command as an example for the amd64 architecture:
```bash
docker run -e "Mqtt__Server=127.0.0.1" -e "Mqtt__Login=" -e "Mqtt__Password=" -e "Lupusec__Url=http://192.168.2.50" -e "Lupusec__Login=admin" -e "Lupusec__Password=lupusecpassword" -e "MotionSensor__DetectionDuration=120" -e "Serilog__MinimumLevel__Default=Information" cyberdns/lupusec2mqtt_hassaddon_amd64:3.1.2
```
Replace the architecture in the last part of the image name based on your system:
```json
"arch": [
    "aarch64",
    "amd64",
    "armhf",
    "armv7"
]
```

### Alternative 2: Volumes
An alternative approach is to use Docker volumes. The settings are located in the container at `/data/options.json`. The file should look like this:
```json
{
"Mqtt:Server": "127.0.0.1",
"Mqtt:Login": "",
"Mqtt:Password": "",
"Lupusec:Url": "http://192.168.2.50",
"Lupusec:Login": "admin",
"Lupusec:Password": "lupusecpassword",
"Serilog:MinimumLevel:Default": "Information",
"MotionSensor:DetectionDuration": 120
}
```


