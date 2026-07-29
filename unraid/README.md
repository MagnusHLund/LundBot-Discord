# Deploy LundBot Discord on Unraid

## Appdata directory

1. Within `/mnt/user/appdata` create a directory called `LundBot-Discord`.
2. Copy the whole project into the newly created directory.

```bash
cd /mnt/user/appdata/LundBot-Discord
git clone https://github.com/MagnusHLund/LundBot-Discord.git
mv LundBot-Discord/* .
mv LundBot-Discord/.* . 2>/dev/null
rm -rf LundBot-Discord
```

3. After the files are present, follow the [setup guide](/README.md)

## Build container script

1. Ensure you have the `User Scripts` plugin installed, by `Andrew Zawadzki`
2. Navigate to `plugins` in the navbar
3. Click on the icon for `User Scripts`
4. Create a script, with a memorable name, with the following contents:

```bash
#!/bin/bash
cd /mnt/user/appdata/LundBot-Discord

docker compose --profile Production -f docker-compose.yml down --remove-orphans
docker compose --profile Development -f docker-compose.yml down --remove-orphans

git checkout main
git fetch --tags

LATEST_TAG=$(git describe --tags `git rev-list --tags --max-count=1`)
echo "Latest tag: $LATEST_TAG"

git checkout $LATEST_TAG

docker compose -f docker-compose.yml up -d --build
```

5. Run `git config --global --add safe.directory /mnt/user/appdata/LundBot-Discord` in the terminal.
6. Run the newly created script. Wait until the pop up window closes itself.

Use this script whenever you change:

- Dockerfile
- Docker-compose
- Entrypoint
