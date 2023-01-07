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
using Mirror;
using GameCore;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), "ForceRoundStart")]
	public class RoundStartEventPatch
	{

		public static bool Prefix(ref bool __result, CharacterClassManager __instance)
		{

			try
			{
				if (!NetworkServer.active)
				{
					__result = false;
					return false;
				}
				if (!RoundSummary.RoundInProgress())
				{
					Round.WarheadLocked = false;
					ServerLogs.AddLog(ServerLogs.Modules.Logger, "Round has been started.", ServerLogs.ServerLogType.GameEvent);
					ServerConsole.AddLog("New round has been started.", ConsoleColor.Gray);
					Round.RoundCount++;
					EventManager.Manager.HandleEvent<IEventHandlerRoundStart>(new RoundStartEvent(Round.RoundCount));
				
					var Stage = "NONE";
					try
					{
						Stage = "Set Timer";
                        RoundStart.singleton.NetworkTimer = -1;
						Stage = "Restart Timer";
                        RoundStart.RoundStartTimer.Restart();
                        Stage = "Set RoundStarted!";
                        __instance.NetworkRoundStarted = true;
						Stage = "Send Rpc";
                        __instance.RpcRoundStarted();
                    }
					catch (Exception ex)
					{
						Logger.Error("ROUND START", $"ERROR STAGE: {Stage} ex: {ex}");
					}

                    __result = true;
					return false;
				}
				__result = false;
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("RoundStartEvent error: {0}", arg));
			}
			return true;
		}
	}

}
