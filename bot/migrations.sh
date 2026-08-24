#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

ENVIRONMENT=""
MIGRATION_NAME=""
SETTINGS_FILE=""
DB_CONTAINER=""
PROFILE=""
CONNECTION_STRING=""
STARTED_BY_SCRIPT=false


check_dependencies() {
    if ! command -v jq >/dev/null 2>&1; then
        echo "ERROR: jq is not installed. Install jq to continue."
        exit 1
    fi

    if ! dotnet ef --version >/dev/null 2>&1; then
        echo "ERROR: dotnet-ef is not installed."
        echo "Install it with:"
        echo "  dotnet tool install --global dotnet-ef"
        echo "or:"
        echo "  dotnet tool install dotnet-ef"
        exit 1
    fi
}


read_input() {
    echo "Choose environment (dev/prod): "
    read ENVIRONMENT

    echo "Migration name: "
    read MIGRATION_NAME
}


configure_environment() {
    if [ "$ENVIRONMENT" = "dev" ]; then
        SETTINGS_FILE="$SCRIPT_DIR/src/appsettings.Development.json"
        DB_CONTAINER="lundbot-mariadb-dev"
        PROFILE="Development"
    else
        SETTINGS_FILE="$SCRIPT_DIR/src/appsettings.Production.json"
        PROFILE="Production"
    fi
}


load_connection_string() {
    CONNECTION_STRING=$(jq -r '.Database.ConnectionString' "$SETTINGS_FILE")

    if [ -z "$CONNECTION_STRING" ] || [ "$CONNECTION_STRING" = "null" ]; then
        echo "ERROR: Connection string is missing or empty in $SETTINGS_FILE"
        exit 1
    fi

    if [ "$ENVIRONMENT" = "dev" ]; then
        CONNECTION_STRING=$(echo "$CONNECTION_STRING" |
            sed 's/Server=lundbot-mariadb-dev/Server=127.0.0.1/')
    fi
}


start_mariadb() {
    if [ "$ENVIRONMENT" != "dev" ]; then
        return
    fi

    local running
    running=$(docker ps --filter "name=$DB_CONTAINER" --format "{{.Names}}")

    if [ "$running" = "$DB_CONTAINER" ]; then
        echo "MariaDB dev container already running."
        return
    fi

    if ! docker info >/dev/null 2>&1; then
        echo "ERROR: Cannot start MariaDB dev container because Docker is not running."
        exit 1
    fi

    echo "Starting MariaDB dev container..."
    docker compose --profile "$PROFILE" up -d mariadb-dev

    STARTED_BY_SCRIPT=true
}


wait_for_mariadb() {
    if [ "$ENVIRONMENT" != "dev" ]; then
        return
    fi

    echo "Waiting for MariaDB to become ready..."

    local status=""

    for i in {1..30}; do
        status=$(docker inspect \
            --format='{{.State.Health.Status}}' \
            "$DB_CONTAINER" 2>/dev/null)

        if [ "$status" = "healthy" ]; then
            echo "MariaDB is ready."
            return
        fi

        if [ "$status" = "unhealthy" ]; then
            echo "ERROR: MariaDB container became unhealthy."

            docker inspect \
                --format='{{range .State.Health.Log}}{{println .Output}}{{end}}' \
                "$DB_CONTAINER"

            exit 1
        fi

        echo "MariaDB not ready yet... ($i)"
        sleep 1
    done

    echo "ERROR: MariaDB did not become ready in time."
    exit 1
}


run_migrations() {
    export EF_CONNECTION_STRING="$CONNECTION_STRING"

    echo "Adding migration..."
    dotnet ef migrations add "$MIGRATION_NAME" \
        --project "$SCRIPT_DIR/src" \
        --startup-project "$SCRIPT_DIR/src"

    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to add migration."
        exit 1
    fi

    echo "Updating database..."
    dotnet ef database update \
        --project "$SCRIPT_DIR/src" \
        --startup-project "$SCRIPT_DIR/src" \
        --connection "$CONNECTION_STRING"

    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to update database."
        exit 1
    fi
}


cleanup() {
    if [ "$ENVIRONMENT" = "dev" ] && [ "$STARTED_BY_SCRIPT" = true ]; then
        echo "Stopping MariaDB dev container..."
        docker compose --profile "$PROFILE" down
    fi
}


main() {
    check_dependencies
    read_input
    configure_environment
    load_connection_string
    start_mariadb
    wait_for_mariadb
    run_migrations
    cleanup

    echo "Done."
}


main