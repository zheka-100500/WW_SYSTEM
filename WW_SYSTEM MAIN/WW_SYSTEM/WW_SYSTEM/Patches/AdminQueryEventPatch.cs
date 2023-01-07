using CommandSystem;
using HarmonyLib;
using Mirror;
using NorthwoodLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using RemoteAdmin;
using RemoteAdmin.Communication;
using RemoteAdmin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Discord;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Translation;
using WW_SYSTEM.VanillaFixes;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(CommandProcessor), "ProcessQuery")]
	public class AdminQueryEventPatch
	{

		public static bool Prefix(string q, CommandSender sender, ref string __result)
		{

			try
			{
				__result = null;

                bool isBot = q.Contains($"BOT_RA_COMMAND_REQUEST{DiscordBot.SecretKey}");

				if (isBot) q = q.Replace($"BOT_RA_COMMAND_REQUEST{DiscordBot.SecretKey}", "");


				

				if (!q.StartsWith("$", StringComparison.Ordinal) && sender != null)
				{




					Player component = null;
		

					try
					{
						if (sender.Nickname == "SERVER CONSOLE")
						{

							component = Server.LocalPlayer;

						}
						else
						{
							PlayerCommandSender playerCommandSender1 = sender as PlayerCommandSender;

							component = Round.GetPlayer(playerCommandSender1.ReferenceHub);
						}
					}
					catch (Exception)
					{

						return false;
					}
					if (component == null)
					{
						return false;
					}
						component.IsDiscordBot = isBot;

					var cmds = q.Split(new char[] { ' ' }).ToList();


					string Result = MainLoader.commandShell.RunCommand(q, component, CommandType.RemoteAdmin, out var res);
					if (Result != "ERROR")
					{
						if(Result == "OUTPUT")
						{
							sender.RaReply($"{cmds[0].ToUpper()}#{res}", true, true, string.Empty);
					
						}

						if (Result == "DENIED")
						{
							sender.RaReply($"{cmds[0].ToUpper()}#{res}", false, true, string.Empty);
							return false;
						}

						AdminQueryEvent adminQueryEvent1 = new AdminQueryEvent(component, q, null, false, false);
						EventManager.Manager.HandleEvent<IEventHandlerAdminQuery>(adminQueryEvent1);
						return false;
					}

					
					if(cmds.Count > 0)
					{
						if (!component.IsVanillaCmdPermitted(cmds.ToArray(), out var perms))
						{
							

							var RequiredPerms = "";

							foreach (var item in perms)
							{
								RequiredPerms += $" {Translator.MainTranslation.GetTranslation(item)}.";
							}
							var NoPermMsg = Translator.MainTranslation.GetTranslation("CMD_NO_PERM").Replace("%perm%", RequiredPerms);
							sender.RaReply($"{cmds[0].ToUpper()}#{NoPermMsg}", false, true, string.Empty);
							return false;
						}
					}

			
					AdminQueryEvent adminQueryEvent = new AdminQueryEvent(component, q, null, false, false);
						EventManager.Manager.HandleEvent<IEventHandlerAdminQuery>(adminQueryEvent);
						if (adminQueryEvent.Handled)
						{
							if (string.IsNullOrEmpty(adminQueryEvent.Output))
							{
								Logger.Error("EVENT MANAGER", string.Format("AdminQueryEvent error: {0}", "Output is null or empty!"));
								return true;
							}
							sender.RaReply(adminQueryEvent.Output, adminQueryEvent.Successful, true, string.Empty);
							return false;
						}


					






				}

				if (isBot)
				{
					CommandProcessor.ProcessQuery(q, sender);
					return false;
				}


                if (q.StartsWith("$", StringComparison.Ordinal))
                {
                    string[] array = q.Remove(0, 1).Split(new char[]
                    {
                    ' '
                    });
                    if (array.Length <= 1)
                    {
                        __result = null;
                        return false;
                    }
                    int key;
                    if (!int.TryParse(array[0], out key))
                    {
                        __result = null;
                        return false;
                    }
                    IServerCommunication serverCommunication;
                    if (CommunicationProcessor.ServerCommunication.TryGetValue(key, out serverCommunication))
                    {
                        serverCommunication.ReceiveData(sender, string.Join(" ", array.Skip(1)));
                    }

                    __result = null;
                    return false;
                }
                else
                {
                    PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
                    if (q.StartsWith("@", StringComparison.Ordinal))
                    {
                        if (!CommandProcessor.CheckPermissions(sender, "Admin Chat", PlayerPermissions.AdminChat, string.Empty, true))
                        {
                            if (playerCommandSender != null)
                            {
                                playerCommandSender.ReferenceHub.queryProcessor.TargetAdminChatAccessDenied(playerCommandSender.ReferenceHub.queryProcessor.connectionToClient);
                            }
                            __result = "You don't have permissions to access Admin Chat!";
							return false;
                        }
                        q = q + " ~" + sender.Nickname;
                        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
                        {
                            if ((referenceHub.serverRoles.AdminChatPerms || referenceHub.serverRoles.RaEverywhere) && referenceHub.Mode != ClientInstanceMode.Unverified)
                            {
                                referenceHub.queryProcessor.TargetReply(referenceHub.queryProcessor.connectionToClient, q, true, false, string.Empty);
                            }
                        }
                        __result = null;
                        return false;
                    }
                    else
                    {
                        string[] array2 = q.Trim().Split(QueryProcessor.SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);
                        ICommand command;
                        if (CommandProcessor.RemoteAdminCommandHandler.TryGetCommand(array2[0], out command))
                        {
                            try
                            {
                                string text;
                                bool flag = command.Execute(array2.Segment(1), sender, out text);
                                if (!string.IsNullOrEmpty(text))
                                {
                                    sender.RaReply(array2[0].ToUpperInvariant() + "#" + text, flag, true, "");
                                }
                                __result = text;
								return false;
                            }
                            catch (Exception ex)
                            {
                                string text2 = "Command execution failed! Error: " + Misc.RemoveStacktraceZeroes(ex.ToString());
                                sender.RaReply(text2, false, true, array2[0].ToUpperInvariant() + "#" + text2);
                                __result = text2;
								return false;
                            }
                        }
                        sender.RaReply("SYSTEM#Unknown command!", false, true, string.Empty);
                        __result = "Unknown command!";
						return false;
                    }
                }




            }
			catch (Exception arg)
			{
				

				Logger.Error("[EVENT MANAGER]", string.Format("AdminQueryEvent error: {0}", arg));
			}
			return true;
		}
	}

}
