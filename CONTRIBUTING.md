Contributing
============

Editor used: Visual Studio Code

Use Secrets Manager to create your user settings.
To do this you can create a usersettings.json file (it will not be pushed because it is included in .gitignore),
and then execute the command: type .\usersettings.json | dotnet user-secrets set

__Example usersettings.json:__

```json
{
    "Mqtt": {
        "Server": "e.g. 192.168.2.2",
        "Login": "e.g. johndoe",
        "Password": "e.g. supersecretpassword"
    },
    "Lupusec": {
        "Url": "e.g. http://192.168.2.3",
        "Login": "e.g. johndoe",
        "Password": "e.g. supersecretpassword"
    }
}
```
