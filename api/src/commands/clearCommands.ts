import {
  ChatInputCommandInteraction,
  MessageFlags,
  PermissionFlagsBits,
  REST,
  Routes,
  SlashCommandBuilder,
} from 'discord.js';
import { Command } from '@/types/index.js';

const command: Command = {
  data: new SlashCommandBuilder()
    .setName('clear-commands')
    .setDescription('Clear all registered slash commands (owner only)')
    .setDefaultMemberPermissions(PermissionFlagsBits.Administrator),

  async execute(interaction: ChatInputCommandInteraction) {
    if (!interaction.inGuild() || !interaction.guildId) {
      await interaction.reply({
        content: 'This command can only be used inside a server.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    const token = process.env.DISCORD_TOKEN;
    const clientId = interaction.client.user?.id;

    if (!token || !clientId) {
      await interaction.reply({
        content: 'Bot configuration error.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    try {
      await interaction.deferReply({ ephemeral: true });

      const rest = new REST({ version: '10' }).setToken(token);

      const commands = await rest.get(
        Routes.applicationGuildCommands(clientId, interaction.guildId)
      );

      if (!Array.isArray(commands) || commands.length === 0) {
        await interaction.editReply({
          content: 'No commands to clear.',
        });
        return;
      }

      let deletedCount = 0;
      for (const cmd of commands) {
        await rest.delete(
          Routes.applicationGuildCommand(clientId, interaction.guildId, (cmd as any).id)
        );
        deletedCount++;
      }

      await interaction.editReply({
        content: `✓ Cleared ${deletedCount} slash command(s) from this guild.`,
      });
    } catch (error) {
      console.error('Failed to clear commands:', error);
      await interaction.editReply({
        content: 'Failed to clear commands. Please try again.',
      });
    }
  },
};

export default command;
