using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Level_System;
namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(NicknameSync), "SetNick")]
	public class PlayerJoinEventPatch
	{

		public static bool Prefix(ref string nick, NicknameSync __instance)
		{
			
			try
			{

				if (!NetworkServer.active)
				{
					Debug.LogWarning("[Server] function 'System.Void NicknameSync::SetNick(System.String)' called on client");
					return false;
				}

				if (string.IsNullOrEmpty(__instance.MyNick))
				{
					
					__instance.MyNick = nick;
					
					if (__instance.isLocalPlayer && ServerStatic.IsDedicated && __instance._hub.characterClassManager.UserId != "owner@waer-world") return false;
					Player pl = __instance.gameObject.AddComponent<Player>();
				
					pl.LoadComponents();
					if (Round.UseLevelSYSTEM)
					{
			
						LevelManager.CheckForPlayer(pl.UserId);
					}
					

					Server.Players.Add(pl);
						PlayerJoinEvent ev = new PlayerJoinEvent(pl);
						EventManager.Manager.HandleEvent<IEventHandlerPlayerJoin>(ev);
					
					
				}
				else
				{
                    __instance.MyNick = nick;
                    string text;
                    try
                    {
                        Regex nickFilter = __instance._nickFilter;
                        text = (((nickFilter != null) ? nickFilter.Replace(nick, __instance._replacement) : null) ?? nick);
                    }
                    catch (Exception arg)
                    {
                        ServerConsole.AddLog(string.Format("Error when filtering nick {0}: {1}", nick, arg), ConsoleColor.Gray);
                        text = "(filter failed)";
                    }
                    if (nick != text)
                    {
                        __instance.DisplayName = text;
                    }
                    if (__instance.isLocalPlayer && ServerStatic.IsDedicated)
                    {
                        return false;
                    }
                    ServerConsole.AddLog(string.Concat(new string[]
{
            "Nickname of ",
            __instance._hub.characterClassManager.UserId,
            " is now ",
            nick,
            "."
}), ConsoleColor.Gray);
                    ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Concat(new string[]
                    {
            "Nickname of ",
            __instance._hub.characterClassManager.UserId,
            " is now ",
            nick,
            "."
                    }), ServerLogs.ServerLogType.ConnectionUpdate, false);
                }

                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]",string.Format("PlayerJoinEvent error: {0}", arg));
			}
			return true;
		}
	}

}
