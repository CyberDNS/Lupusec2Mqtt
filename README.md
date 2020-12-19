# Lupusec2Mqtt

Make your Lupusec XT2 (and compatible) Alarm System available on an MQTT Broker (compatible with Home Assistant).

**For Homeassistant Users:** It is available as Addon that can be directly installed by the Supervisor tab.  
The repository is: https://github.com/CyberDNS/hassio-addons-repository

The docker image isn't anymore maintained, you can create your own docker image if needed (Dockerfile is still present in this project), otherwise if you are using Homeassistant, please migrate to my addon repository (link above).

## Tested with
- XT2
- XT2 Plus

## What is already working

- Alarm panel (config, state, set)
- Burglar Alarm state (config, state)
- Open/Close contacts (config, state)
- Movement detector (config, state)
- Power switch (config, state, set, consumption)
- Smoke detector (config)
