import { ChatInputCommandInteraction, MessageFlags, SlashCommandBuilder } from 'discord.js';
import { Command } from '@/types/index.js';

const command: Command = {
  data: new SlashCommandBuilder()
    .setName('upvote')
    .setDescription('Upvote a user and return both Discord user IDs')
    .addUserOption((option) =>
      option.setName('user').setDescription('The user to upvote').setRequired(true)
    ),

  async execute(interaction: ChatInputCommandInteraction) {
    const voterUserId = interaction.user.id;
    const targetUserId = interaction.options.getUser('user', true).id;

    if (targetUserId === voterUserId) {
      await interaction.reply({
        content: 'You cannot upvote yourself.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    await interaction.reply({
      content: `Upvote registered!`,
      flags: MessageFlags.Ephemeral,
    });
  },
};

export default command;
