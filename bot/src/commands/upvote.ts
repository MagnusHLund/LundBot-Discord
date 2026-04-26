import {
  ChannelType,
  ChatInputCommandInteraction,
  MessageFlags,
  SlashCommandBuilder,
} from 'discord.js';
import { getPrismaClient } from '../services/database.js';
import { Command } from '../types/index.js';

const MAX_LEADERBOARD_MESSAGE_LENGTH = 1900;

function splitLeaderboardContent(
  prependMessage: string,
  title: string,
  leaderboardLines: string
): string[] {
  const header = `**${prependMessage}**\n# ${title}\n\n`;

  if (!leaderboardLines.trim()) {
    return [header];
  }

  const lines = leaderboardLines.split('\n');
  const chunks: string[] = [];
  let currentChunk = header;

  for (const line of lines) {
    const lineWithNewline = `${line}\n`;

    if (
      currentChunk.length + lineWithNewline.length > MAX_LEADERBOARD_MESSAGE_LENGTH &&
      currentChunk !== header
    ) {
      chunks.push(currentChunk.trimEnd());
      currentChunk = lineWithNewline;
      continue;
    }

    if (currentChunk.length + lineWithNewline.length > MAX_LEADERBOARD_MESSAGE_LENGTH) {
      chunks.push(currentChunk.trimEnd());
      currentChunk = lineWithNewline;
      continue;
    }

    currentChunk += lineWithNewline;
  }

  if (currentChunk.trim().length > 0) {
    chunks.push(currentChunk.trimEnd());
  }

  return chunks.length > 0 ? chunks : [header.trimEnd()];
}

const command: Command = {
  data: new SlashCommandBuilder()
    .setName('upvote')
    .setDescription('Upvote a user in a specific leaderboard channel')
    .addChannelOption((option) =>
      option
        .setName('channel')
        .setDescription('Channel that has the leaderboard')
        .addChannelTypes(ChannelType.GuildText)
        .setRequired(true)
    )
    .addUserOption((option) =>
      option.setName('user').setDescription('The user to upvote').setRequired(true)
    ),

  async execute(interaction: ChatInputCommandInteraction) {
    if (!interaction.inGuild() || !interaction.guildId) {
      await interaction.reply({
        content: 'This command can only be used inside a server.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    const channel = interaction.options.getChannel('channel', true, [ChannelType.GuildText]);
    const voterUserId = interaction.user.id;
    const targetUserId = interaction.options.getUser('user', true).id;
    const prisma = getPrismaClient();

    if (targetUserId === voterUserId) {
      await interaction.reply({
        content: 'You cannot upvote yourself.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    try {
      const leaderboard = await prisma.leaderboards.findUnique({
        where: {
          leaderboards_index_2: {
            discordServerId: interaction.guildId,
            discordChannelId: channel.id,
          },
        },
      });

      if (!leaderboard) {
        await interaction.reply({
          content: 'No leaderboard found for that channel.',
          flags: MessageFlags.Ephemeral,
        });
        return;
      }

      const existingVote = await prisma.upvotingLeaderBoard.findUnique({
        where: {
          UpvotingLeaderBoard_index_2: {
            leaderboardsId: leaderboard.leaderboardsId,
            discordUserIdVoter: voterUserId,
            discordUserIdTarget: targetUserId,
          },
        },
      });

      if (existingVote) {
        await interaction.reply({
          content: `You can only upvote <@${targetUserId}> once in this leaderboard.`,
          flags: MessageFlags.Ephemeral,
        });
        return;
      }

      await prisma.$transaction([
        prisma.upvotingLeaderBoard.create({
          data: {
            leaderboardsId: leaderboard.leaderboardsId,
            discordUserIdVoter: voterUserId,
            discordUserIdTarget: targetUserId,
          },
        }),
        prisma.leaderboardScores.upsert({
          where: {
            LeaderboardScores_index_2: {
              discordUserId: targetUserId,
              leaderboardsId: leaderboard.leaderboardsId,
            },
          },
          create: {
            leaderboardsId: leaderboard.leaderboardsId,
            discordUserId: targetUserId,
            score: 1,
          },
          update: {
            score: {
              increment: 1,
            },
          },
        }),
      ]);

      const topScores = await prisma.leaderboardScores.findMany({
        where: {
          leaderboardsId: leaderboard.leaderboardsId,
        },
        orderBy: [{ score: 'desc' }, { updatedAt: 'asc' }],
      });

      const leaderboardLines =
        topScores.length > 0
          ? topScores
              .map((entry, index) => `${index + 1}. <@${entry.discordUserId}> — ${entry.score}`)
              .join('\n')
          : '_No upvotes yet._';

      const messageChunks = splitLeaderboardContent(
        leaderboard.message,
        leaderboard.title,
        leaderboardLines
      );

      const leaderboardMessages = await prisma.leaderboardMessages.findMany({
        where: {
          leaderboardsId: leaderboard.leaderboardsId,
        },
        orderBy: {
          leaderboardMessagesId: 'asc',
        },
        select: {
          leaderboardMessagesId: true,
          discordMessageId: true,
        },
      });

      const sharedLength = Math.min(leaderboardMessages.length, messageChunks.length);

      for (let index = 0; index < sharedLength; index += 1) {
        const leaderboardMessage = leaderboardMessages[index];
        const chunk = messageChunks[index];
        const discordMessage = await channel.messages
          .fetch(leaderboardMessage.discordMessageId)
          .catch(() => null);

        if (discordMessage) {
          await discordMessage.edit(chunk);
          continue;
        }

        const replacementMessage = await channel.send({ content: chunk });
        await prisma.leaderboardMessages.update({
          where: {
            leaderboardMessagesId: leaderboardMessage.leaderboardMessagesId,
          },
          data: {
            discordMessageId: replacementMessage.id,
          },
        });
      }

      if (messageChunks.length > leaderboardMessages.length) {
        for (let index = leaderboardMessages.length; index < messageChunks.length; index += 1) {
          const newMessage = await channel.send({ content: messageChunks[index] });
          await prisma.leaderboardMessages.create({
            data: {
              leaderboardsId: leaderboard.leaderboardsId,
              discordMessageId: newMessage.id,
            },
          });
        }
      }

      if (leaderboardMessages.length > messageChunks.length) {
        const extraMessages = leaderboardMessages.slice(messageChunks.length);

        for (const extraMessage of extraMessages) {
          const discordMessage = await channel.messages
            .fetch(extraMessage.discordMessageId)
            .catch(() => null);

          if (discordMessage) {
            await discordMessage.delete().catch(() => undefined);
          }
        }

        await prisma.leaderboardMessages.deleteMany({
          where: {
            leaderboardMessagesId: {
              in: extraMessages.map((message) => message.leaderboardMessagesId),
            },
          },
        });
      }

      await interaction.reply({
        content: `Upvoted <@${targetUserId}> in <#${channel.id}>.`,
        flags: MessageFlags.Ephemeral,
      });
    } catch (error) {
      console.error('Failed to register upvote:', error);
      await interaction.reply({
        content: 'Failed to register upvote. Please try again.',
        flags: MessageFlags.Ephemeral,
      });
    }
  },
};

export default command;
