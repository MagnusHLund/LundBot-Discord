# Deploy LundBot Discord on Unraid

## Appdata directory

1. Within `/mnt/user/appdata` create a directory called `LundBot-Discord`.
2. Copy the whole project into the newly created directory.

## Add app icon

1. Run the command below in the terminal within Unraid.

```bash
mkdir -p /boot/config/plugins/dockerMan/images
cp /mnt/user/appdata/LundBot-Discord/unraid/LundBot.png /boot/config/plugins/dockerMan/images/
```

## App template

1. Navigate into `/boot/config/plugins/dockerMan/templates-user/`
2. create an xml file named `lundbotdiscord.xml` with the same contents as [lundbotdiscord.xml](unraid/lundbotdiscord.xml) has within this directory.

## Create the app in Unraid

1. In the web GUI, navigate to docker
2. From there click `add container`
3. Choose the template called `LundBot Discord`
4. Scroll down and click `Apply`

## Build container script

1. Ensure you have the `User Scripts` plugin installed
2. Create a script within `User Scripts` with the following contents

```bash
#!/bin/bash
cd /mnt/user/appdata/LundBot-Discord
docker compose down --remove-orphans
docker compose up -d --build
```

Use this script whenever you change:
* Dockerfile
* Docker-compose
* Entrypoint