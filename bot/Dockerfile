FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY src/ /app/src/

ENV DOTNET_USE_POLLING_FILE_WATCHER=1 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=1

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]