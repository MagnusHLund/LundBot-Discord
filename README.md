# LundBot Discord

## Setup

### Env

1. Create a .env file in the root directory (together with the docker-compose file).
2. Within the .env file, add `ASPNETCORE_ENVIRONMENT=Production` or `ASPNETCORE_ENVIRONMENT=Development`.
3. Within the .env file, add `COMPOSE_PROFILES=Production` or `COMPOSE_PROFILES=Development`. Ideally it should be the same value as `ASPNETCORE_ENVIRONMENT`.
4. Within the .env file, add `USE_WATCH=true` or `USE_WATCH=false` depending on if you want to use dotnet watch

### appsettings

1. Within the bot `/bot/src/` directory, add `appsettings.Development.json` or `appsettings.Production.json` depending on the value you set in the .env file for `ASPNETCORE_ENVIRONMENT`.
2. You can now mess with the following values, within the json file that you just created:

```json
{
  "Discord": {
    "Token": "",
    "FastUpdateGuildIds": [""],
    "WebTrafficChannelId": "",
    "ShouldRegisterGlobalCommands": false,
    "RoleIdToAutoKick": ""
  },
  "Database": {
    "ConnectionString": "Server=x;Port=x;Database=x;User=x;Password=x;"
  },
  "DeveloperEnvironment": {
    "GenerateIpAddresses": false
  }
}
```

Note that configs under the `DeveloperEnvironment` object is only available when using `ASPNETCORE_ENVIRONMENT=Development` in the .env file. It can be safely removed from the `appsettings.Production.json` file.

## Run project

1. Docker has to be installed and running on the host machine
2. Write `docker compose up` within the root directory of this project
