using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM_BOT.Commands
{
    public class RaCommands
    {
        [Command("list")]
        public async Task Ping(CommandContext ctx)
        {
            if (!Bot.CommandsChannels.Contains(ctx.Message.Channel)) return;

            if (Bot.Config.AllowedRaUsers.Contains(ctx.Message.Author.Id))
            {

                
             await ctx.Channel.SendMessageAsync("Sending command to server..").ConfigureAwait(false);
                //  string request = JsonConvert.SerializeObject(new DiscordRequest(ctx.Message.Author.Username, ctx.Message.Author.Id, "LIST"));
                Packet p = new Packet();
                p.Write(Bot.Config.SecretKey);
                p.Write((int)DiscordBotMsgType.REQUEST);
                p.Write("LIST");
                
                await Bot.SendMsg(p);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("NOT PERM").ConfigureAwait(false);
            }
           
        }

        [Command("sudo")]
        public async Task SUDO(CommandContext ctx)
        {
            if (!Bot.CommandsChannels.Contains(ctx.Message.Channel)) return;

            if (Bot.Config.AllowedConsoleUsers.Contains(ctx.Message.Author.Id))
            {
                if (string.IsNullOrEmpty(ctx.RawArgumentString))
                {
                    await ctx.Channel.SendMessageAsync("NO ARGUMENTS").ConfigureAwait(false);
                    return;
                }

              //  await ctx.Channel.SendMessageAsync("Sending command to server console..").ConfigureAwait(false);
                Logger.Info("ADMIN", $"ADMIN: {ctx.Message.Author.Username} USED COMMAND: {ctx.RawArgumentString.Remove(0, 1)}");
                Packet p = new Packet();
                p.Write(Bot.Config.SecretKey);
                p.Write((int)DiscordBotMsgType.CONSOLE_COMMAND);
                p.Write(ctx.RawArgumentString.Remove(0, 1));

                await Bot.SendMsg(p);
                ///  await Bot.SendMsg(new DiscordBotMsg(Bot.Config.SecretKey, ctx.RawArgumentString.Remove(0, 1), DiscordBotMsgType.CONSOLE_COMMAND));
            }
            else
            {
                await ctx.Channel.SendMessageAsync("NOT PERM").ConfigureAwait(false);
            }

        }

        [Command("ra")]
        public async Task ra(CommandContext ctx)
        {
            if (!Bot.CommandsChannels.Contains(ctx.Message.Channel)) return;

            if (Bot.Config.AllowedRaUsers.Contains(ctx.Message.Author.Id))
            {
                if (string.IsNullOrEmpty(ctx.RawArgumentString))
                {
                    await ctx.Channel.SendMessageAsync("NO ARGUMENTS").ConfigureAwait(false);
                    return;
                }

                await ctx.Channel.SendMessageAsync("Sending command to remote admin..").ConfigureAwait(false);
                Logger.Info("ADMIN", $"ADMIN: {ctx.Message.Author.Username} USED COMMAND: {ctx.RawArgumentString.Remove(0, 1)}");
                Packet p = new Packet();
                p.Write(Bot.Config.SecretKey);
                p.Write((int)DiscordBotMsgType.RA_COMMAND);
                p.Write(ctx.RawArgumentString.Remove(0, 1));

                await Bot.SendMsg(p);

                //  Logger.Info("ADMIN", $"ADMIN: {ctx.Message.Author.Username} USED COMMAND: {ctx.RawArgumentString.Remove(0, 1)}");
                //  await Bot.SendMsg(new DiscordBotMsg(Bot.Config.SecretKey, ctx.RawArgumentString.Remove(0, 1), DiscordBotMsgType.RA_COMMAND));
            }
            else
            {
                await ctx.Channel.SendMessageAsync("NOT PERM").ConfigureAwait(false);
            }

        }

        [Command("game")]
        public async Task game(CommandContext ctx)
        {
            if (!Bot.CommandsChannels.Contains(ctx.Message.Channel)) return;

            if (Bot.Config.AllowedRaUsers.Contains(ctx.Message.Author.Id))
            {
                if (string.IsNullOrEmpty(ctx.RawArgumentString))
                {
                    await ctx.Channel.SendMessageAsync("NO ARGUMENTS").ConfigureAwait(false);
                    return;
                }

                await ctx.Channel.SendMessageAsync("Sending command to game console..").ConfigureAwait(false);
                Logger.Info("ADMIN", $"ADMIN: {ctx.Message.Author.Username} USED COMMAND: {ctx.RawArgumentString.Remove(0, 1)}");
                Packet p = new Packet();
                p.Write(Bot.Config.SecretKey);
                p.Write((int)DiscordBotMsgType.GAME_CONSOLE);
                p.Write(ctx.RawArgumentString.Remove(0, 1));

                await Bot.SendMsg(p);
                //   await Bot.SendMsg(new DiscordBotMsg(Bot.Config.SecretKey, ctx.RawArgumentString.Remove(0, 1), DiscordBotMsgType.GAME_CONSOLE));
            }
            else
            {
                await ctx.Channel.SendMessageAsync("NOT PERM").ConfigureAwait(false);
            }

        }


    }
}
