import 'dotenv/config';
import { Client, ChannelType, GatewayIntentBits } from 'discord.js';
import { loadCommands, loadEvents, registerCommands } from '@/utils/loader.js';
import { getPrismaClient, disconnectPrisma } from '@/services/database.js';
import { logWithTimestamp } from '@/utils/helpers.js';

const TOKEN = process.env.DISCORD_TOKEN;

if (!TOKEN) {
  throw new Error('DISCORD_TOKEN environment variable is not set');
}

const client = new Client({
  intents: [
    GatewayIntentBits.Guilds,
    GatewayIntentBits.GuildMembers,
    GatewayIntentBits.GuildMessages,
    GatewayIntentBits.MessageContent,
    GatewayIntentBits.DirectMessages,
  ],
});

// Initialize Prisma client
const prisma = getPrismaClient();

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
  const commands = await loadCommands(client);

  if (commands.size > 0) {
    await registerCommands(client, commands);
  }

  // Store commands in client for later use
  (client as any).commands = commands;

  // Set bot status
  client.user.setActivity('the server', { type: 'WATCHING' });
});

// Load events
await loadEvents(client);

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
