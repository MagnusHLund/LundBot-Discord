FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY src/ /app/src/

ENV DOTNET_USE_POLLING_FILE_WATCHER=1 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=1

RUN apt-get update && apt-get install -y unzip curl && rm -rf /var/lib/apt/lists/* && \
    mkdir -p /vsdbg && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]