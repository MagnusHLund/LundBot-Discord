import 'dotenv/config';
import { ActivityType, Client, Events, GatewayIntentBits, MessageFlags } from 'discord.js';
import { loadCommands, registerCommands } from '@/utils/loader.js';
import { getPrismaClient, disconnectPrisma } from '@/services/database.js';
import { logWithTimestamp } from '@/utils/helpers.js';

const TOKEN = process.env.DISCORD_TOKEN;

if (!TOKEN) {
  throw new Error('DISCORD_TOKEN environment variable is not set');
}

const client = new Client({
  intents: [GatewayIntentBits.Guilds],
});

// Initialize Prisma client
getPrismaClient();

// Handle uncaught errors
process.on('unhandledRejection', (reason, promise) => {
  console.error('Unhandled Rejection at:', promise, 'reason:', reason);
});

process.on('uncaughtException', (error) => {
  console.error('Uncaught Exception:', error);
});

// Bot startup
client.once('ready', async () => {
  if (!client.user) {
    throw new Error('Client user is not available');
  }

  logWithTimestamp('info', `Bot logged in as ${client.user.tag}`);

  // Load commands and register with Discord
  const commands = await loadCommands();

  if (commands.size > 0) {
    await registerCommands(client, commands);
  }

  // Store commands in client for later use
  (client as any).commands = commands;

  // Set bot status
  client.user.setActivity('the server', { type: ActivityType.Watching });
});

client.on(Events.InteractionCreate, async (interaction) => {
  if (!interaction.isChatInputCommand()) {
    return;
  }

  const command = (interaction.client as any).commands?.get(interaction.commandName);

  if (!command) {
    console.warn(`No command matching ${interaction.commandName} was found.`);
    return;
  }

  try {
    await command.execute(interaction);
  } catch (error) {
    console.error(`Error executing command ${interaction.commandName}:`, error);

    if (interaction.replied || interaction.deferred) {
      await interaction.followUp({
        content: 'There was an error while executing this command!',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    await interaction.reply({
      content: 'There was an error while executing this command!',
      flags: MessageFlags.Ephemeral,
    });
  }
});

// Graceful shutdown
async function shutdown(signal: string): Promise<void> {
  logWithTimestamp('info', `Received ${signal}, shutting down gracefully...`);
  await disconnectPrisma();
  await client.destroy();
  process.exit(0);
}

process.on('SIGINT', () => shutdown('SIGINT'));
process.on('SIGTERM', () => shutdown('SIGTERM'));

// Login to Discord
await client.login(TOKEN);
