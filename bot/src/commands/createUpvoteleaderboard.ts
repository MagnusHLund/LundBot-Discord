import {
  ChannelType,
  ChatInputCommandInteraction,
  MessageFlags,
  PermissionFlagsBits,
  SlashCommandBuilder,
} from 'discord.js';
import { getPrismaClient } from '../services/database.js';
import { Command } from '../types/index.js';

const LEADERBOARD_EMPTY_STATE = '_No upvotes yet._';

const command: Command = {
  data: new SlashCommandBuilder()
    .setName('create-upvote-leaderboard')
    .setDescription('Create and post an upvote leaderboard in a selected channel')
    .setDefaultMemberPermissions(PermissionFlagsBits.Administrator)
    .addChannelOption((option) =>
      option
        .setName('channel')
        .setDescription('Channel where the leaderboard message will be posted')
        .addChannelTypes(ChannelType.GuildText)
        .setRequired(true)
    )
    .addStringOption((option) =>
      option
        .setName('title')
        .setDescription('Leaderboard title (max 64 chars)')
        .setMaxLength(64)
        .setRequired(true)
    )
    .addStringOption((option) =>
      option
        .setName('message')
        .setDescription('Message to prepend above the leaderboard (max 256 chars)')
        .setMaxLength(256)
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
    const title = interaction.options.getString('title', true).trim();
    const prependMessage = interaction.options.getString('message', true).trim();

    if (title.length > 64) {
      await interaction.reply({
        content: 'Title is too long. Maximum length is 64 characters.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    if (prependMessage.length > 256) {
      await interaction.reply({
        content: 'Message is too long. Maximum length is 256 characters.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    const prisma = getPrismaClient();
    let postedMessage: Awaited<ReturnType<typeof channel.send>> | null = null;

    const existingLeaderboard = await prisma.leaderboards.findUnique({
      where: {
        leaderboards_index_2: {
          discordServerId: interaction.guildId,
          discordChannelId: channel.id,
        },
      },
    });

    if (existingLeaderboard) {
      await interaction.reply({
        content: `There can only be one leaderboard per channel. <#${channel.id}> already has a leaderboard.`,
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    try {
      postedMessage = await channel.send({
        content: `**${prependMessage}**\n# ${title}\n\n${LEADERBOARD_EMPTY_STATE}`,
      });

      const leaderboard = await prisma.leaderboards.create({
        data: {
          discordServerId: interaction.guildId,
          discordChannelId: channel.id,
          discordMessageId: postedMessage.id,
          title,
          message: prependMessage,
        },
      });

      await interaction.reply({
        content: `Leaderboard created in <#${channel.id}> (id: ${leaderboard.leaderboardsId}).`,
        flags: MessageFlags.Ephemeral,
      });
    } catch (error) {
      if (postedMessage) {
        await postedMessage.delete().catch(() => undefined);
      }

      console.error('Failed to create leaderboard:', error);
      if (interaction.replied || interaction.deferred) {
        await interaction.followUp({
          content: 'Failed to create leaderboard. Please try again.',
          flags: MessageFlags.Ephemeral,
        });
      } else {
        await interaction.reply({
          content: 'Failed to create leaderboard. Please try again.',
          flags: MessageFlags.Ephemeral,
        });
      }
    }
  },
};

export default command;
