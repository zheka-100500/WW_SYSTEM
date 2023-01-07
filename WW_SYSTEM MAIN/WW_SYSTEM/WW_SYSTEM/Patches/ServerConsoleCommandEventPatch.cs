using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using UnityEngine;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ServerConsole), "EnterCommand")]
	public class ServerConsoleCommandEventPatch
	{

		public static bool Prefix(string cmds, ref string __result, CommandSender sender, ServerConsole __instance)
		{

			try
			{
				 
			
				string[] array = cmds.Split(new char[]
				{
					' '
				});
				string text2 = array[0].ToUpper();

				if(array.Length == 1 && text2 == "PLUGIN")
				{
					__result = "PLUGIN {LOAD, UNLOAD, LIST}";
			
					return false;
				}
				if (text2 == "PLUGIN" && array.Length >= 2)
				{
					if (string.IsNullOrEmpty(array[1].ToUpper()))
					{
						__result = "PLUGIN {LOAD, UNLOAD, LIST}";
						
						return false;
					}
					if (array[1].ToUpper() == "UNLOAD")
					{
						MainLoader.manager.DisablePlugin(array[2]);
						__result = "DONE!";
				
						return false;
					}
					if (array[1].ToUpper() == "LOAD")
					{
						MainLoader.manager.EnablePlugin(PluginManager.Manager.GetDisabledPlugin(array[2]));
						__result = "DONE!";
				
						return false;
					}
					if (array[1].ToUpper() == "LIST")
					{
						string text3 = "PLUGINS LIST";
						foreach (Plugin plugin in MainLoader.manager.EnabledPlugins)
						{
							text3 = string.Concat(new string[]
							{
								text3,
								Environment.NewLine,
								"NAME: ",
								plugin.Details.name,
								", AUTHOR ",
								plugin.Details.author,
								", DESCRIPTION: ",
								plugin.Details.description,
								", ID: ",
								plugin.Details.id
							});
						}
						__result = text3;
						return false;
					}
					__result = "PLUGIN {LOAD, UNLOAD, LIST}";
			
					return false;
				}
				else
				{
					
					string Result = MainLoader.commandShell.RunCommand(cmds, Server.LocalPlayer, CommandType.ServerConsole, out var res);
					if (Result != "ERROR")
					{
						if (Result == "DONE!")
						{
							__result = "";
							return false;
						}
						__result = res;
						return false;
					}


					ServerConsoleCommandEvent serverConsoleCommandEvent = new ServerConsoleCommandEvent();
					serverConsoleCommandEvent.Command = text2;
					serverConsoleCommandEvent.Args = array;
					serverConsoleCommandEvent.Handled = false;
					EventManager.Manager.HandleEvent<IEventHandlerCallConsoleCommand>(serverConsoleCommandEvent);
					if (serverConsoleCommandEvent.Handled)
					{
						if (string.IsNullOrEmpty(serverConsoleCommandEvent.Output))
						{
							__result = "ERROR OUTPUT MESSAGE NOT SET!";
					
						}
						else
						{
							__result = serverConsoleCommandEvent.Output;
						}
				
						return false;
					}
					return true;
				}
		
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("ServerConsoleCommandEvent error: {0}", arg));
			}
		
			return true;
		}
	}

}
