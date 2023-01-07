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
using GameCore;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ServerConsole), "ReloadServerName")]
	public class SetServerNameEventPatch
	{

		public static bool Prefix(ServerConsole __instance)
		{

			try
			{
				SetServerNameEvent setServerNameEvent = new SetServerNameEvent(ConfigFile.ServerConfig.GetString("server_name", "My Server Name"));
				EventManager.Manager.HandleEvent<IEventHandlerSetServerName>(setServerNameEvent);
				ServerConsole._serverName = setServerNameEvent.ServerName + $" <color=#ffffff00><size=1>WW-SYSTEM-{PluginManager.GetWWSYSTEMVersion()}</size></color>";

				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("SetServerNameEvent error: {0}", arg));
			}
			return true;
		}
	}

}
