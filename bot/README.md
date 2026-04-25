# Lundbot Discord API

A professional TypeScript Discord bot with Prisma ORM for MariaDB database integration.

## Project Structure

```
api/
├── src/
│   ├── commands/          # Slash commands
│   ├── services/          # Business logic services
│   ├── types/             # TypeScript type definitions
│   ├── utils/             # Utility functions
│   └── index.ts           # Bot entry point
├── prisma/
│   └── schema.prisma      # Database schema
├── dist/                  # Compiled JavaScript
├── .env.example           # Environment variables template
├── .eslintrc.json         # ESLint configuration
├── .prettierrc.json       # Prettier configuration
├── tsconfig.json          # TypeScript configuration
└── package.json           # Dependencies and scripts
```

## Setup Instructions

### 1. Install Dependencies

```bash
cd api
npm install
```

### 2. Configure Environment Variables

```bash
cp .env.example .env
```

Edit `.env` with your Discord bot token and database connection string:

```
DISCORD_TOKEN=your_bot_token_here
DATABASE_URL=mysql://user:password@localhost:3306/lundbot
NODE_ENV=development
BOT_API_KEY=some-long-random-secret
BOT_API_PORT=3000
BOT_ALLOWED_ORIGINS=https://infinitewarefarecommunity.com,https://www.infinitewarefarecommunity.com
```

### 2b. HTTP API

The bot starts a small HTTP API alongside Discord. Protect it with `BOT_API_KEY`.

**Health check**

```bash
GET /health
```

Returns a lightweight liveness response with the bot runtime state.

**Readiness check**

```bash
GET /ready
```

Returns `200` when the Discord client is ready and the database is reachable, otherwise `503`.

**Send a message**

```bash
POST /message
Content-Type: application/json
x-api-key: your-secret

{
  "channelId": "123456789012345678",
  "content": "Hello from my API"
}
```

**Edit a message**

```bash
PATCH /message
Content-Type: application/json
x-api-key: your-secret

{
  "channelId": "123456789012345678",
  "messageId": "987654321098765432",
  "content": "Updated message text"
}
```

**Responses**

- `201` for send
- `200` for edit
- `200` for healthy `GET /health`
- `200` for ready `GET /ready`
- `401` if the API key is wrong
- `503` if the bot is not ready yet

### 3. Set Up Database

```bash
# Generate Prisma client
npm run prisma:generate

# Run migrations
npm run prisma:migrate
```

### 4. Run the Bot

**Development mode (with hot reload):**

```bash
npm run dev
```

**Production mode:**

```bash
npm run build
npm run start
```

## Docker Compose

The recommended deployment path is to build the bot image in GitHub Actions and deploy it to your server over SSH.

GitHub Actions will:

1. build the Docker image
2. push it to GitHub Container Registry
3. SSH into your server
4. write the Compose file and `.env` on the server
5. log in to GHCR on the server
6. run `docker compose pull` and `docker compose up -d`

Your server needs Docker and Docker Compose installed, and it must allow SSH access from the key you add to GitHub Secrets.

You can still run the bot locally with Docker Compose from the repo root.

### 1. Create your env file

```bash
cp .env.example .env
```

Run that from the repo root so Compose picks up the same `.env` file. Fill in the values in `.env`, especially:

- `DISCORD_TOKEN`
- `DATABASE_URL`

If you want to run Compose locally, also set `BOT_IMAGE_TAG` if you want a specific image tag. By default, Compose pulls `latest`.

### 2. Start everything

```bash
docker compose up -d
```

For local runs, if the image is private, authenticate to GitHub Container Registry first with a token that has `read:packages` access:

```bash
docker login ghcr.io
```

For the automated server deployment, this login happens on the server inside GitHub Actions.

The bot container will connect to your existing database, run Prisma migrations on startup, then start the Discord bot and HTTP API.

### 3. GitHub Actions secrets for deployment

Set these repository secrets in GitHub:

- `DEPLOY_HOST` - your server hostname or IP
- `DEPLOY_USER` - SSH username on the server
- `DEPLOY_SSH_KEY` - private SSH key used by GitHub Actions
- `DEPLOY_PORT` - optional SSH port, defaults to `22`
- `DEPLOY_PATH` - optional directory on the server, defaults to `/opt/lundbot-discord`
- `GHCR_USERNAME` - your GitHub username
- `GHCR_TOKEN` - classic PAT with `read:packages`
- `DISCORD_TOKEN` - Discord bot token
- `DATABASE_URL` - database connection string
- `BOT_API_KEY` - API key used by your HTTP API
- `BOT_API_PORT` - optional, defaults to `3000`
- `BOT_ALLOWED_ORIGINS` - optional comma-separated CORS allowlist, defaults to `https://infinitewarefarecommunity.com,https://www.infinitewarefarecommunity.com`

### 4. What runs

- MariaDB on `localhost:3306`
- The bot + HTTP API on `localhost:3000`

The bot container will run Prisma migrations on startup, then start the Discord bot and HTTP API.

## Available Commands

- `npm run dev` - Start bot in development mode with auto-reload
- `npm run build` - Compile TypeScript to JavaScript
- `npm run start` - Run compiled bot
- `npm run lint` - Check code quality with ESLint
- `npm run lint:fix` - Auto-fix ESLint issues
- `npm run format` - Format code with Prettier
- `npm run prisma:generate` - Generate Prisma client
- `npm run prisma:migrate` - Create and run database migrations
- `npm run prisma:studio` - Open Prisma Studio for database GUI
- `npm run type-check` - Run TypeScript type checking

## Creating a New Command

Create a new file in `src/commands/`:

```typescript
import { SlashCommandBuilder, ChatInputCommandInteraction } from 'discord.js';
import { Command } from '@/types/index.js';

const command: Command = {
  data: new SlashCommandBuilder().setName('mycommand').setDescription('Command description'),
  async execute(interaction: ChatInputCommandInteraction) {
    await interaction.reply('Command executed!');
  },
};

export default command;
```

## Creating a New Event

Create a new file in `src/events/`:

```typescript
import { BotEvent } from '@/types/index.js';

const event: BotEvent = {
  name: 'guildMemberAdd',
  async execute(member: any) {
    console.log(`${member.user.tag} joined the guild!`);
  },
};

export default event;
```

## Accessing the Database

In any service or command:

```typescript
import { getPrismaClient } from '@/services/database.js';

const prisma = getPrismaClient();

// Create a user
const user = await prisma.user.create({
  data: {
    userId: '123456789',
    username: 'user',
  },
});

// Query users
const users = await prisma.user.findMany();
```

## Best Practices Implemented

- ✅ **TypeScript** - Full type safety
- ✅ **ESLint + Prettier** - Code quality and formatting
- ✅ **Prisma ORM** - Type-safe database access with migrations
- ✅ **Environment Variables** - Secure configuration with `.env`
- ✅ **Modular Structure** - Separation of concerns (commands, events, services)
- ✅ **Error Handling** - Graceful shutdown and error logging
- ✅ **Module Paths** - `@/` alias for clean imports
- ✅ **Singleton Pattern** - Prisma client singleton for memory efficiency
- ✅ **Hot Reload** - Development with `tsx watch`
- ✅ **Type Safety** - Strict TypeScript configuration

## Troubleshooting

### Bot not responding to commands

1. Make sure you've invited the bot with the `/applications.commands` scope
2. Verify `DISCORD_TOKEN` is correct
3. Check that the bot has permissions in the guild
4. Run `npm run prisma:generate` if you see Prisma client errors

### Database connection issues

1. Verify `DATABASE_URL` format: `mysql://user:password@host:port/database`
2. Ensure MariaDB is running
3. Check database user permissions
4. Run `npm run prisma:migrate` to create tables

### Hot reload not working

Try deleting `node_modules` and reinstalling:

```bash
rm -rf node_modules
npm install
npm run dev
```

## License

MIT
