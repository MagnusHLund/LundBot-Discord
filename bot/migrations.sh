#!/bin/bash

echo "Choose environment (dev/prod): "
read ENVIRONMENT

echo "Migration name: "
read MIGRATION_NAME

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Determine settings file
if [ "$ENVIRONMENT" = "dev" ]; then
    SETTINGS_FILE="$SCRIPT_DIR/src/appsettings.Development.json"
    DB_CONTAINER="lundbot-mariadb-dev"
    PROFILE="Development"
else
    SETTINGS_FILE="$SCRIPT_DIR/src/appsettings.Production.json"
    PROFILE="Production"
fi

# Extract connection string from JSON
CONNECTION_STRING=$(jq -r '.Database.ConnectionString' "$SETTINGS_FILE")

if [ -z "$CONNECTION_STRING" ] || [ "$CONNECTION_STRING" = "null" ]; then
    echo "ERROR: Connection string is missing or empty in $SETTINGS_FILE"
    exit 1
fi

STARTED_BY_SCRIPT=false
if [ "$ENVIRONMENT" = "dev" ]; then
    # Replace 'Server=lundbot-mariadb-dev' with 'Server=127.0.0.1
    CONNECTION_STRING=$(echo "$CONNECTION_STRING" | sed 's/Server=lundbot-mariadb-dev/Server=127.0.0.1/')

    # Check if mariadb-dev is running (only for dev)
    RUNNING=$(docker ps --filter "name=$DB_CONTAINER" --format "{{.Names}}")

    if [ "$RUNNING" = "$DB_CONTAINER" ]; then
        echo "MariaDB dev container already running."
    else
        if ! docker info >/dev/null 2>&1; then
            echo "ERROR: Cannot start MariaDB dev container because Docker is not running."
            exit 1
        fi

        echo "Starting MariaDB dev container..."
        docker compose --profile Development up -d mariadb-dev
        STARTED_BY_SCRIPT=true
    fi

    # Health check to ensure mariadb is running
    for i in {1..20}; do
        if docker exec "$DB_CONTAINER" mysqladmin ping -h127.0.0.1 -uroot -ppassword --silent; then
            echo "MariaDB is ready."
            break
        fi

        echo "MariaDB not ready yet... ($i)"
        sleep 1
    done
fi

# Set EF_CONNECTION_STRING environment variable for dotnet ef commands
export EF_CONNECTION_STRING="$CONNECTION_STRING"

# Run migration
echo "Adding migration..."
dotnet ef migrations add "$MIGRATION_NAME" --project "$SCRIPT_DIR/src" --startup-project "$SCRIPT_DIR/src"

echo "Updating database..."
dotnet ef database update --project "$SCRIPT_DIR/src" --startup-project "$SCRIPT_DIR/src" --connection "$CONNECTION_STRING"

# Stop container if script started it
if [ "$ENVIRONMENT" = "dev" ] && [ "$STARTED_BY_SCRIPT" = true ]; then
    echo "Stopping MariaDB dev container..."
    docker compose --profile Development down
fi

echo "Done."
