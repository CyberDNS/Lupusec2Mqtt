# Lupusec2Mqtt

Make your Lupusec XT2 (and compatible) Alarm System available on an MQTT Broker (compatible with Home Assistant)

## Tested with
- XT2
- XT2 Plus

## What is already working

- Alarm panel (config, state, set)
- Open/Close contacts (config, state)
- Smoke detector (config)
- Movement detector (config)

## Getting started

1. Docker should be up and running (Linux container), docker-compose is useful too.
2. Docker Image on [DockerHub](https://hub.docker.com/r/cyberdns/lupusec2mqtt): docker pull cyberdns/lupusec2mqtt 
3. Docker Compose file example to show the needed environment variables and volume
This example includes an MQTT broker and Home Assistant

```
version: '2'
services:
  mosquitto:
    container_name: mosquitto
    image: "eclipse-mosquitto:latest"
    ports:
      - "1883:1883"
      - "9001:9001"
  hass:
    container_name: hass
    image: homeassistant/home-assistant:latest
    network_mode: host
    restart: always
    ports: 
      - "8123:8123"
    depends_on:
      - "mosquitto"
    volumes:
      - ./homeassistant:/config
      - /etc/localtime:/etc/localtime:ro
      - /usr/bin:/usr/bin
  lupusec2mqtt:
    container_name: lupuse2mqtt
    image: cyberdns/lupusec2mqtt:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DOTNET_MQTT__SERVER=192.168.2.2
      - DOTNET_MQTT__LOGIN=
      - DOTNET_MQTT__PASSWORD=
      - DOTNET_LUPUSEC__URL=http://192.168.2.3
      - DOTNET_LUPUSEC__LOGIN=admin
      - DOTNET_LUPUSEC__PASSWORD=supersecret   
    depends_on:
      - "hass"        
    volumes:
      - ./lupusec2mqtt:/config
```
4. Add a configuration file in the mapped folder (e.g. volume in docker compose: `lupusec2mqtt`)

This maps for example the Open/Close sensors to an appropriate DeviceClass in Home Assistant.
If no mapping is provided the default is DeviceClass: window
```
{
    "Mappings": {
      "RF:01985f10": {
        "DeviceClass": "door"
      },
      "RF:01a0d110": {
        "DeviceClass": "door"
      },
      "RF:01928510": {
        "DeviceClass": "door"
      },
      "RF:01999310": {
        "DeviceClass": "door"
      },
      "RF:01a25810": {
        "DeviceClass": "door"
      },
      "RF:01a11110": {
        "DeviceClass": "door"
      },
      "RF:01a91510": {
        "DeviceClass": "garage_door"
      },
      "RF:01a6e112": {
        "DeviceClass": "garage_door"
      }
    }
}
```



