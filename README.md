# StreamerConnect
The 1 day coding for create batch and use in streamer.bot using HIDmacros

Requierd [***net. (code) 6.0***](https://dotnet.microsoft.com/en-us/download/dotnet) and above

### How to use:
- Call this program for first time `sbotac.exe`
- Open [Streamer.bot](https://streamer.bot/) with [http server enabled](https://wiki.streamer.bot/en/Servers-Clients/HTTP-Server)
- Genererate file using `sbotac.exe --create`

### Other Command:
- `sbotac.exe --create --group="group name"` Generate file only for selected action group
- `sbotac.exe --create --enabled-only` Generate file only enabled action
- `sbotac.exe --create --group="group name --enabled-only"` Generate file only for selected action group and enabled action
- `sbotac.exe --name="name action"` Execude action using name instead id
