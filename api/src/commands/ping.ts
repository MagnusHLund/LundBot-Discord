import {
  SlashCommandBuilder,
  ChatInputCommandInteraction,
  MessageFlags,
  PermissionFlagsBits,
} from 'discord.js';
import { Command } from '@/types/index.js';

const command: Command = {
  data: new SlashCommandBuilder()
    .setName('ping')
    .setDescription('Replies with Pong!')
    .setDefaultMemberPermissions(PermissionFlagsBits.Administrator),
  async execute(interaction: ChatInputCommandInteraction) {
    if (!interaction.memberPermissions?.has(PermissionFlagsBits.Administrator)) {
      await interaction.reply({
        content: 'You must be an administrator to use this command.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    await interaction.reply('Pong!');
  },
};

export default command;
