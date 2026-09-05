# LundBot Discord

## Introduction

This bot is written for the [Infinite Warfare PC](https://discord.gg/FycARMT2YR) (IWPC) discord server.<br>
The functionality of this bot is therefore focused on that server's needs.<br>
However the functionality is mostly generic enough, to be used on any discord server.

You can read more about the discord server on our [website](https://infinitewarfarecommunity.com).

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

Note that configs under the `DeveloperEnvironment` object is only available when using `ASPNETCORE_ENVIRONMENT=Development` in the .env file. It can be safely removed from the `appsettings.Production.json` file.<br>
The Database connection string is setup to work with the development docker compose profile.

The `ApiKey` is a value that you decide.
It is a basic layer of security, to prevent anyone from calling the protected HTTP endpoints.

## Run project

1. Docker has to be installed and running on the host machine
2. Write `docker compose up` within the root directory of this project

## EF Core migrations

To run migrations, run `bot/migrations.sh` from the root directory.<br>
You will be prompted for the environment, which is either `dev` or `prod`.<br>
Then you will be prompted for a migration name.<br>
The script will then create a migration.<br>

Note that you must have jq installed.
`https://jqlang.org/download/`

And obviously also dotnet-ef
`dotnet tool install dotnet-ef` or `dotnet tool install --global dotnet-ef`

## Features

1. Upvote leaderboard so members can upvote members of their choosing, depending on the purpose of the leaderboard
2. Invite leaderboards, tracks who has brought in the most new members
3. Warnings leaderboard, track who has had warnings.
4. Custom IW inspired join messages, when new members join the discord server.
5. Possibility to kick users if they get a specific role (Useful in the onboarding flow, for filtering specific users)
6. Remove welcome message for new user, if they were kicked in the onboarding flow
7. Get random IW zombies map
8. Basic telemetry for website navigation, through endpoints
9. Protected endpoints for administration purposes of commands, leaderboards and server info.

## Commands

### Leaderboards

Creates a leaderboard of the specific type.
There can only be one leaderboard per channel.
Creating a leaderboard returns a message only visible to the one that sent the command, along with a message created by the bot, which creates the actual leaderboard message in a channel.

Minimum permission: Administrator

Command: `/create_leaderboard`

Parameters:

1. Text **channel** that that he leaderboard will be in.
2. **Type** of leaderboard. For example **Upvote** leaderboard.
3. **Title** of the leaderboard. Max 64 characters.
4. **Message** displayed above the title. Used to be more descriptive of what the leaderboard is for. Max 256 characters.

You can also remove a leaderboard.
This will remove the message(s) that the leaderboard uses in its channel, along with all related data in the database.
The response message is only visible to the one that sent the command.

Minimum permission: Administrator

Command: `/remove_leaderboard`

Parameters:

1. **Channel** that the leaderboard is located within.
2. **Confirm** is a boolean value, which ensures you got a little bit of extra time before you might accidentally remove the wrong leaderboard.

#### Upvote leaderboard

The upvote leaderboard allows people to upvote other server members.
The upvoter member can only upvote the same target member once.

To upvote, you have to use the upvote command.

##### Upvote command

Upvotes another person in an upvote leaderboard.

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

#### Warnings leaderboard

The warn leaderboard is closely related to the upvote leaderboard.
The main difference is that you can warn the user multiple times.
Ideally a warnings leaderboard should be in a channel, only accessible to admins.

A future feature would be that the warnings leaderboard would also show warn reasons.
Bot that has not yet been implemented and requires a new database table.

##### Warn command

Used to register a warning for a warnings leaderboard.

Minimum permission: Administrator

Command: `/warn`

Parameters:

1. **Channel** that the leaderboard is located within.
2. **User** to warn on the leaderboard.

### Random map

You can get the bot to return a random Infinite Warfare zombies map.
The response is only visible to the person that sent the command.

Minimum permission: Anyone

Command: `/random_map`

Parameters: none

### Ping

The ping command is the most basic.
It basically just checks if the bot is accessible through commands.
The bot will return the response "pong". The response is only visible to the person that wrote the command.

Minimum permission: Administrator

Command: `/ping`

Parameters: none

## Endpoints

The endpoints are meant to be used only by bot administrators.
It is not something that a normal user or administrator should be using.
This is with the exception of the traffic endpoints, as they are not used for bot management.

Endpoints gets authenticated with an api-key, which is set in `appsettings.Production.json` and `appsettings.Development.json`.
When calling the endpoint, you must add the `Authorization` header.

[Refer to the http files](/bot/http/). To make production API calls, create a file called `name.prod.http`. Replace the "name" part with the actual name.
For example `health.prod.http`. These files are gitignored.

The endpoint sections are just short descriptions. To see how to actually call the API, refer to the http files linked above.

### Command endpoints

The command endpoints are used for managing the bot's Discord application commands.

#### Unregister command endpoint

Unregisters a Discord application command using its ID.
The endpoint can be used to unregister either a global command or a guild-specific command. It can also be used to unregister all commands at once.

#### Sync commands endpoint

Synchronizes the bot's Discord application commands.
This refreshes the registered commands to ensure that Discord has the commands currently configured by the bot.

### Leaderboard endpoints

The leaderboard endpoints are used to manage the bot's Discord leaderboards.

#### Refresh leaderboard endpoint

Refreshes a leaderboard for a specified Discord guild and channel.
The guild ID and channel ID must be provided to identify which leaderboard should be refreshed.

### Health endpoints

The health endpoints are used to check the current status of the bot API.
The endpoint returns the bot's health status and current version.

### Traffic endpoints

These endpoints are related to events that happen on the [community's website](https://infinitewarfarecommunity.com).

When either of the 2 endpoints are being executed, a discord message is being updated, to display the data for administrators.
The channel to use is configured in the `appsettings.Production.json` and `appsettings.Development.json` files.

#### Visit endpoint

Registers a visit to the community's website.
The requester's IP address is used to track the visit, to avoid repeat visitors.
The ip address is hashed, so it is not possible to reverse which ip's have visited the website.

#### Discord invite click endpoint

Registers a click on the Discord invite link on the community's website.
The requester's IP address is recorded to allow the invite-link click to be tracked.
