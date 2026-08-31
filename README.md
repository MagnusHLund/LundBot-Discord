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
    "FastUpdateGuildIds": [0],
    "WebTrafficChannelId": 0,
    "ShouldRegisterGlobalCommands": false,
    "RoleIdToAutoKick": 0,
    "Roles": {
      "NotPcPlayer": 0,
      "Microsoft": 0,
      "Steam": 0,
      "Bot": 0,
      "Unlocker": 0,
      "NeedsHelp": 0,
      "OfferingHelp": 0,
      "SpeedRunner": 0,
      "ContentCreator": 0,
      "ServerBooster": 0,
      "Moderator": 0,
      "Owner": 0
    }
  },
  "Database": {
    "ConnectionString": "Server=lundbot-mariadb-dev;Port=3306;Database=LundBotDiscord;User=root;Password=password;"
  },
  "DeveloperEnvironment": {
    "GenerateIpAddresses": false
  },
  "Server": {
    "ApiKey": ""
  }
}
```

Note that configs under the `DeveloperEnvironment` object is only available when using `ASPNETCORE_ENVIRONMENT=Development` in the .env file. It can be safely removed from the `appsettings.Production.json` file.
The Database connection string is setup to work with the development docker compose profile.

The `ApiKey` is a value that you decide.
It is a basic layer of security, to prevent anyone from calling the protected HTTP endpoints.

## Run project

1. Docker has to be installed and running on the host machine
2. Write `docker compose up` within the root directory of this project

## EF Core migrations

To run migrations, run `bot/migrations.sh` from the root directory.
You will be prompted for the environment, which is either `dev` or `prod`.
Then you will be prompted for a migration name.
The script will then create a migration.

Note that you must have jq installed.
`https://jqlang.org/download/`

And obviously also dotnet-ef
`dotnet tool install dotnet-ef` or `dotnet tool install --global dotnet-ef`

## Features

### Leaderboards

Creates a leaderboard of the specific type.
There can only be one leaderboard per channel.

Minimum permission: Administrator

Command: `/create_leaderboard`

Parameters:

1. **Text channel** that that he leaderboard will be in.
2. **Type** of leaderboard. For example **Upvote** leaderboard.
3. **Title** of the leaderboard. Max 64 characters.
4. **Message** displayed above the title. Used to be more descriptive of what the leaderboard is for. Max 256 characters.



#### Upvote leaderboard

The upvote leaderboard allows people to upvote other server members.
The upvoter member can only upvote the same target member once.

To upvote, you have to use the upvote command.

#### Upvote command

Minimum permission: Anyone

Command: `/upvote`

Parameters:

1. **Channel** that the leaderboard is located within.
2. **User** to upvote on the leaderboard.

#### Invite leaderboard

The invite leaderboard keeps track of who has invited the most users, that have ended up joining.
So its not purely invites. Its invite and then join from that invite.

The bot has to be active to track the leaderboard, as the data can not be directly pulled from Discord.
It gets tracked by the bot itself.

#### Warning leaderboard

The warn leaderboard is closely related to the upvote leaderboard.
The main difference is that you can warn the user multiple times.

A future feature would be that the warnings leaderboard would also show warn reasons.
Bot that has not yet been implemented and requires a new database table.

#### Warn command

Minimum permission: Administrator

Command: `/warn`

Parameters:

1. **Channel** that the leaderboard is located within.
2. **User** to warn on the leaderboard.
