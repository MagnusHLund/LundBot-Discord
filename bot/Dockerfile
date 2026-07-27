FROM mcr.microsoft.com/dotnet/sdk:10.0

# The compose volume mounts the bot source tree to /app, so run from the project folder.
WORKDIR /app

# Ensure appsettings.json exists
RUN mkdir -p /app/src && \
    if [ ! -f /app/src/appsettings.json ]; then \
      echo '{}' > /app/src/appsettings.json; \
    fi

ENV DOTNET_USE_POLLING_FILE_WATCHER=1 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://0.0.0.0:65535 \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=1 \
    ASPNETCORE_ENVIRONMENT=Development

RUN apt-get update && apt-get install -y unzip curl && rm -rf /var/lib/apt/lists/* && \
    mkdir -p /vsdbg && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Use dotnet watch on the API project in /app/src.
CMD ["dotnet", "watch", "run", "--no-launch-profile", "--project", "src/LundBot.csproj", "--configuration", "Debug"]