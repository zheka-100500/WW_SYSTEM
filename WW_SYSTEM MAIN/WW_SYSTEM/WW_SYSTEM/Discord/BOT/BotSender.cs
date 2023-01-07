using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    class BotSender : CommandSender
	{
		public override string SenderId
		{
			get
			{
				return "SERVER CONSOLE";
			}
		}

		public override string Nickname
		{
			get
			{
				return "SERVER CONSOLE";
			}
		}

		public override ulong Permissions
		{
			get
			{
				return ServerStatic.PermissionsHandler.FullPerm;
			}
		}



		public override byte KickPower
		{
			get
			{
				return byte.MaxValue;
			}
		}


		public override bool FullPermissions
		{
			get
			{
				return true;
			}
		}


		public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
		{
            try
            {
				if (text.Contains("#"))
				{

					int index = text.IndexOf("#");
					string Result = text.Substring(index + 1);
					string Prefix = text.Replace(text.Substring(index), "");

					Logger.Info("BOT COMMAND", Result);
					DiscordBot.SendCommandMessage($"[{Prefix}] {Result}");
				}
				else
				{
					Logger.Info("BOT COMMAND", text);
					DiscordBot.SendCommandMessage(text);
				}
			}
            catch (Exception ex)
			{
				Logger.Error("BOT", $"DETECTED ERROR {ex}");
				Logger.Info("BOT COMMAND", text);
				DiscordBot.SendCommandMessage(text);
			}
		}


		public override void Print(string text)
		{
			try
			{
				if (text.Contains("#"))
				{

					int index = text.IndexOf("#");
					string Result = text.Substring(index + 1);
					string Prefix = text.Replace(text.Substring(index), "");

					Logger.Info("BOT COMMAND", Result);
					DiscordBot.SendCommandMessage($"[{Prefix}] {Result}");
				}
				else
				{
					Logger.Info("BOT COMMAND", text);
					DiscordBot.SendCommandMessage(text);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("BOT", $"DETECTED ERROR {ex}");
				Logger.Info("BOT COMMAND", text);
				DiscordBot.SendCommandMessage(text);
			}
		}

		public BotSender()
		{
	
		
	    }
    }
}
