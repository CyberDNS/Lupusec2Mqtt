Contributing
============

Editor used: Visual Studio 2019

Use Secrets Manager to create your user settings.

To do this you right click on the project in Visual Studio and select "Manage User Secrets".
Then add something like:

__Example:__

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

To test in docker you can use Docker Desptop in combination with WSL 2. A launch profile is available. 
