import { Client, REST, Routes, Collection } from 'discord.js';
import { readdir } from 'fs/promises';
import { join } from 'path';
import { fileURLToPath } from 'url';
import { dirname } from 'path';
import { Command } from '@/types/index.js';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

/**
 * Load all commands from the commands directory
 */
export async function loadCommands(): Promise<Collection<string, Command>> {
  const commands = new Collection<string, Command>();
  const commandsPath = join(__dirname, '..', 'commands');

  try {
    const commandFiles = await readdir(commandsPath);
    const typeScriptFiles = commandFiles.filter(
      (file) => file.endsWith('.ts') && !file.endsWith('.test.ts')
    );

    for (const file of typeScriptFiles) {
      const filePath = join(commandsPath, file);
      const command: Command = (await import(`file://${filePath}`)).default;

      if ('data' in command && 'execute' in command) {
        commands.set(command.data.name, command);
        console.info(`✓ Loaded command: ${command.data.name}`);
      } else {
        console.warn(`⚠ Command at ${filePath} is missing required data or execute property.`);
      }
    }
  } catch (error) {
    if ((error as NodeJS.ErrnoException).code !== 'ENOENT') {
      console.error('Error loading commands:', error);
    }
  }

  return commands;
}

/**
 * Register all slash commands with Discord
 */
export async function registerCommands(
  client: Client,
  commands: Collection<string, Command>
): Promise<void> {
  const token = process.env.DISCORD_TOKEN;
  const clientId = client.user?.id;
  const guildIds = process.env.DISCORD_GUILD_ID?.split(',')
    .map((id) => id.trim())
    .filter(Boolean);

  if (!token || !clientId) {
    throw new Error('Missing DISCORD_TOKEN or unable to get client ID');
  }

  const rest = new REST({ version: '10' }).setToken(token);
  const commandData = commands.map((cmd) => cmd.data.toJSON());

  try {
    console.info(`Started registering ${commandData.length} slash commands...`);

    if (guildIds && guildIds.length > 0) {
      await Promise.all(
        guildIds.map((id) =>
          rest.put(Routes.applicationGuildCommands(clientId, id), { body: commandData })
        )
      );
      console.info(`✓ Successfully registered slash commands for guilds: ${guildIds.join(', ')}`);
      return;
    }

    await rest.put(Routes.applicationCommands(clientId), { body: commandData });

    console.info('✓ Successfully registered slash commands');
  } catch (error) {
    console.error('Error registering commands:', error);
  }
}

/**
 * Clear all slash commands from registered scopes (global + configured guilds).
 */
export async function clearCommands(client: Client): Promise<void> {
  const token = process.env.DISCORD_TOKEN;
  const clientId = client.user?.id;
  const guildIds = process.env.DISCORD_GUILD_ID?.split(',')
    .map((id) => id.trim())
    .filter(Boolean);

  if (!token || !clientId) {
    throw new Error('Missing DISCORD_TOKEN or unable to get client ID');
  }

  const rest = new REST({ version: '10' }).setToken(token);

  console.info('Started clearing slash commands from Discord...');

  await rest.put(Routes.applicationCommands(clientId), { body: [] });

  if (guildIds && guildIds.length > 0) {
    await Promise.all(
      guildIds.map((id) => rest.put(Routes.applicationGuildCommands(clientId, id), { body: [] }))
    );
  }

  console.info('✓ Cleared stale slash commands from Discord');
}
