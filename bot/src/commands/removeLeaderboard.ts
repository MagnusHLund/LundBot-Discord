import {
  ChannelType,
  ChatInputCommandInteraction,
  MessageFlags,
  PermissionFlagsBits,
  SlashCommandBuilder,
} from 'discord.js';
import { getPrismaClient } from '../services/database.js';
import { Command } from '../types/index.js';

const command: Command = {
  data: new SlashCommandBuilder()
    .setName('remove-leaderboard')
    .setDescription('Remove an upvote leaderboard from a selected channel')
    .setDefaultMemberPermissions(PermissionFlagsBits.Administrator)
    .addChannelOption((option) =>
      option
        .setName('channel')
        .setDescription('Channel that contains the leaderboard')
        .addChannelTypes(ChannelType.GuildText)
        .setRequired(true)
    )
    .addBooleanOption((option) =>
      option
        .setName('confirm')
        .setDescription('Set to true to confirm deleting the leaderboard')
        .setRequired(true)
    ),

  async execute(interaction: ChatInputCommandInteraction) {
    if (!interaction.inGuild() || !interaction.guildId) {
      await interaction.reply({
        content: 'This command can only be used inside a server.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    if (!interaction.memberPermissions?.has(PermissionFlagsBits.Administrator)) {
      await interaction.reply({
        content: 'You must be an administrator to use this command.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    const channel = interaction.options.getChannel('channel', true, [ChannelType.GuildText]);
    const confirm = interaction.options.getBoolean('confirm', true);
    const prisma = getPrismaClient();

    if (!confirm) {
      await interaction.reply({
        content: 'Set confirm to true to remove the leaderboard.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

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
        content: `No leaderboard found in <#${channel.id}>.`,
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    let deletedDiscordMessage = false;
    const leaderboardMessages = await prisma.leaderboardMessages.findMany({
      where: {
        leaderboardsId: leaderboard.leaderboardsId,
      },
      select: {
        discordMessageId: true,
      },
    });

    try {
      for (const leaderboardMessage of leaderboardMessages) {
        const message = await channel.messages
          .fetch(leaderboardMessage.discordMessageId)
          .catch(() => null);

        if (message) {
          await message.delete();
          deletedDiscordMessage = true;
        }
      }
    } catch (error) {
      console.error('Failed to delete leaderboard message:', error);
    }

    try {
      await prisma.leaderboards.delete({
        where: {
          leaderboardsId: leaderboard.leaderboardsId,
        },
      });

      await interaction.reply({
        content: deletedDiscordMessage
          ? `✓ Removed the leaderboard from <#${channel.id}>.`
          : `✓ Removed the leaderboard from <#${channel.id}>. The Discord message could not be deleted, but the database entry was removed.`,
        flags: MessageFlags.Ephemeral,
      });
    } catch (error) {
      console.error('Failed to remove leaderboard from database:', error);
      await interaction.reply({
        content: 'Failed to remove the leaderboard. Please try again.',
        flags: MessageFlags.Ephemeral,
      });
    }
  },
};

export default command;
