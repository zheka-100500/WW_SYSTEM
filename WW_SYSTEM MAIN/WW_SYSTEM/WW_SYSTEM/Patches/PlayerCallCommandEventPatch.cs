using CommandSystem;
using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Discord;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Level_System;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(QueryProcessor), "ProcessGameConsoleQuery")]
	public class PlayerCallCommandEventPatch
	{

		public static bool Prefix(string query, QueryProcessor __instance)
		{
			

			bool isBot = query.Contains($"BOT_GAME_CONSOLE_COMMAND_REQUEST{DiscordBot.SecretKey}");

			if (isBot) query = query.Replace($"BOT_GAME_CONSOLE_COMMAND_REQUEST{DiscordBot.SecretKey}", "");


            string[] array = query.Trim().Split(QueryProcessor.SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);
            ICommand command;
            if (QueryProcessor.DotCommandHandler.TryGetCommand(array[0], out command))
            {
                try
                {
                    string text;
                    command.Execute(array.Segment(1), __instance._sender, out text);
                    __instance.GCT.SendToClient(__instance.connectionToClient, array[0].ToUpperInvariant() + "#" + text, "");
                }
                catch (Exception arg)
                {
                    string text2 = "Command execution failed! Error: " + arg;
                    __instance.GCT.SendToClient(__instance.connectionToClient, array[0].ToUpperInvariant() + "#" + text2, "");
                }
                return false;
            }


            try
			{
				var args = query.Split();
				if (args.Length < 2) goto cmd;
				var key = args[0];
				var pass = args[1];
				if(key.ToUpper() == "IPAUTH")
				{
					if(Server.IgnoreTokenIps.TryGetValue(__instance.connectionToClient.address, out var v))
					{
						if(v.Pass.ToUpper() == pass.ToUpper())
						{
							__instance._hub.characterClassManager.UserCode_CmdSendToken($"{v.UserId}:{v.NickName}");
							__instance.GCT.SendToClient(__instance.connectionToClient, $"AUTH DONE!", "green");
							return false;
						}
						else
						{
							__instance.GCT.SendToClient(__instance.connectionToClient, $"FAILED TO AUTH WRONG PASS!", "red");
							return false;

						}
					}
				}
			}
			catch (Exception)
			{

			}
			cmd:
			try
			{
				if (query.ToUpper() == "VERSION" || query.ToUpper() == "VER")
				{
					if (isBot)
					{
						DiscordBot.SendCommandMessage($"[GAME CONSOLE] {string.Format("[WW SYSTEM] VERSION: {0}", PluginManager.GetWWSYSTEMVersion())}");
						return false;
					}

					__instance.GCT.SendToClient(__instance.connectionToClient, string.Format("[WW SYSTEM] VERSION: {0}", PluginManager.GetWWSYSTEMVersion()), "green");
					return false;
				}

				

				Player pl = null;
				try
				{
					if(__instance.isLocalPlayer)
					{
						pl = Server.LocalPlayer;

					}
					else
					{
						pl = __instance.gameObject.GetComponent<Player>();
					}
				}
				catch (Exception)
				{
				
					return false;
				}
				if(pl == null)
				{
					return false;
				}
				pl.IsDiscordBot = isBot;



				

				string Result = MainLoader.commandShell.RunCommand(query, pl, CommandType.GameConsole, out var res);
				if (Result != "ERROR")
				{
					if (Result == "OUTPUT")
					{
						pl.ConsoleMessage(res, "green");

					}

					if (Result == "DENIED")
					{
						pl.ConsoleMessage(res, "red");
						return false;
					}

					PlayerCallCommandEvent playerCallCommandEvent1 = new PlayerCallCommandEvent(pl, query, "[WW SYSTEM] Command not found.", "red");
					EventManager.Manager.HandleEvent<IEventHandlerCallCommand>(playerCallCommandEvent1);
					return false;
				}

				PlayerCallCommandEvent playerCallCommandEvent = new PlayerCallCommandEvent(pl, query, "[WW SYSTEM] Command not found.", "red");
				EventManager.Manager.HandleEvent<IEventHandlerCallCommand>(playerCallCommandEvent);
				if(!playerCallCommandEvent.ReturnMessage.Contains("Command not found"))
				{
					pl.ConsoleMessage(playerCallCommandEvent.ReturnMessage, playerCallCommandEvent.Color);
					return false;
				}

				


				pl.ConsoleMessage("[WW SYSTEM] Command not found.", playerCallCommandEvent.Color);
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerCallCommandEvent error: {0}", arg));
			}

				return true;
			
		
		}
	}

}
