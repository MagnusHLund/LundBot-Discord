#!/bin/sh

echo "ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "COMPOSE_PROFILES=$COMPOSE_PROFILES"
echo "USE_WATCH=$USE_WATCH"

if [ "$USE_WATCH" = "true" ]; then
    echo "Starting in WATCH mode (Debug)..."
    exec dotnet watch run --no-launch-profile --project src/LundBot.csproj --configuration Debug
else
    if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
        echo "Starting in RUN mode (Debug)..."
        exec dotnet run --no-launch-profile --project src/LundBot.csproj --configuration Debug
    else
        echo "Starting in RUN mode (Release)..."
        exec dotnet run --no-launch-profile --project src/LundBot.csproj --configuration Release
    fi
fi