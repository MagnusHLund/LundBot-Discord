import { Client, REST, Routes, Collection } from 'discord.js';
import { readdir } from 'fs/promises';
import { join } from 'path';
import { fileURLToPath } from 'url';
import { dirname } from 'path';
import { Command, BotEvent } from '@/types/index.js';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

/**
 * Load all commands from the commands directory
 */
export async function loadCommands(client: Client): Promise<Collection<string, Command>> {
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
 * Load all events from the events directory
 */
export async function loadEvents(client: Client): Promise<void> {
  const eventsPath = join(__dirname, '..', 'events');

  try {
    const eventFiles = await readdir(eventsPath);
    const typeScriptFiles = eventFiles.filter(
      (file) => file.endsWith('.ts') && !file.endsWith('.test.ts')
    );

    for (const file of typeScriptFiles) {
      const filePath = join(eventsPath, file);
      const event: BotEvent = (await import(`file://${filePath}`)).default;

      if ('name' in event && 'execute' in event) {
        if (event.once) {
          client.once(event.name, (...args: any[]) => event.execute(...args));
        } else {
          client.on(event.name, (...args: any[]) => event.execute(...args));
        }
        console.info(`✓ Loaded event: ${event.name}`);
      } else {
        console.warn(`⚠ Event at ${filePath} is missing required name or execute property.`);
      }
    }
  } catch (error) {
    if ((error as NodeJS.ErrnoException).code !== 'ENOENT') {
      console.error('Error loading events:', error);
    }
  }
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

  if (!token || !clientId) {
    throw new Error('Missing DISCORD_TOKEN or unable to get client ID');
  }

  const rest = new REST({ version: '10' }).setToken(token);
  const commandData = commands.map((cmd) => cmd.data.toJSON());

  try {
    console.info(`Started registering ${commandData.length} slash commands...`);

    await rest.put(Routes.applicationCommands(clientId), { body: commandData });

    console.info('✓ Successfully registered slash commands');
  } catch (error) {
    console.error('Error registering commands:', error);
  }
}
