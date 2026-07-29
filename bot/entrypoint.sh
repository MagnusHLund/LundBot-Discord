#!/bin/sh

echo "ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "COMPOSE_PROFILES=$COMPOSE_PROFILES"
echo "USE_WATCH=$USE_WATCH"

if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ] || [ "$USE_WATCH" = "true" ]; then
    if [ ! -d "/vsdbg" ]; then
        echo "Installing vsdbg debugger..."
        apt-get update && apt-get install -y unzip curl && rm -rf /var/lib/apt/lists/*
        mkdir -p /vsdbg
        curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
    else
        echo "vsdbg already installed."
    fi
else
    echo "Skipping vsdbg installation (Release mode)."
fi

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