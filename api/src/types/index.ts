import { Client, SlashCommandBuilder } from 'discord.js';

export interface Command {
  data: SlashCommandBuilder | Omit<SlashCommandBuilder, 'addSubcommand' | 'addSubcommandGroup'>;
  execute: (interaction: any) => Promise<void>;
}

export interface BotEvent {
  name: string;
  once?: boolean;
  execute: (...args: any[]) => Promise<void>;
}
