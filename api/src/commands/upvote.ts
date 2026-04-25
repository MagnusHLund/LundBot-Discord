import {
  ChannelType,
  ChatInputCommandInteraction,
  MessageFlags,
  SlashCommandBuilder,
} from 'discord.js';
import { getPrismaClient } from '@/services/database.js';
import { Command } from '@/types/index.js';

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
            leaderboards_id: leaderboard.leaderboardsId,
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
            leaderboards_id: leaderboard.leaderboardsId,
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
        take: 10,
      });

      const leaderboardLines =
        topScores.length > 0
          ? topScores
              .map((entry, index) => `${index + 1}. <@${entry.discordUserId}> — ${entry.score}`)
              .join('\n')
          : '_No upvotes yet._';

      const leaderboardMessage = await channel.messages
        .fetch(leaderboard.discordMessageId)
        .catch(() => null);

      if (leaderboardMessage) {
        await leaderboardMessage.edit(
          `**${leaderboard.message}**\n\n# ${leaderboard.title}\n\n${leaderboardLines}`
        );
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
