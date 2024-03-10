Contributing
============

You have the choice between using __Visual Studio__ or __VS Code__. The preffered solution is to use __VS Code__ using devcontainer. The devcontainer install all the necessary extensions and also pull up a mosquitto mqtt brocker for development purposes. 
For a quick view what is happening in you MQTT you can use the VSMQTT extension and subscribe with # to see all the topics on the broker.

## Settings
Use Secrets Manager to create your user settings.
For VS Code the [.NET Core User Secrets](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.user-secrets) is preinstalled so that you can right click the csproj file to change your settings.

The mqtt settings are already present in the appsettings.development.json to connect to the dev MQTT brocker. If you want to use the dev instance don't override the settings in your user secrets file for the MQTT part.

__Using dev MQTT:__

```json
{
  "Lupusec:Url": "e.g. http://192.168.2.3",
  "Lupusec:Password": "e.g. supersecretpassword",
  "Lupusec:Login": "e.g. johndoe"
}
```

__Using your own MQTT:__

```json
{
  "Mqtt:Server": "e.g. 192.168.2.2",
  "Mqtt:Password": "e.g. supersecretpassword",
  "Mqtt:Login": "e.g. johndoe",
  "Lupusec:Url": "e.g. http://192.168.2.3",
  "Lupusec:Password": "e.g. supersecretpassword",
  "Lupusec:Login": "e.g. johndoe"
}
```