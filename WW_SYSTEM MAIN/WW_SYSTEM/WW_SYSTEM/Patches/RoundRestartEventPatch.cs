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
using GameObjectPools;
using RoundRestarting;
using Mirror;
namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
	public class RoundRestartEventPatch
	{

		public static bool Prefix()
		{

			try
			{
		
				if (!NetworkServer.active)
				{
					throw new InvalidOperationException("Round restart can only be triggerred by the server!");
				}
               
                PoolManager.Singleton.ReturnAllPoolObjects();
				if (RoundRestart.IsRoundRestarting)
				{
					return false;
				}

				EventManager.Manager.HandleEvent<IEventHandlerRoundRestart>(new RoundRestartEvent());

				

				Round.WarheadLocked = false;
				RoundRestart.IsRoundRestarting = true;
				CustomLiteNetLib4MirrorTransport.DelayConnections = true;
				CustomLiteNetLib4MirrorTransport.UserIdFastReload.Clear();
				IdleMode.PauseIdleMode = true;
				if (CustomNetworkManager.EnableFastRestart)
				{
					foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
					{
						if (referenceHub.Mode != ClientInstanceMode.DedicatedServer)
						{
							try
							{
								CustomLiteNetLib4MirrorTransport.UserIdFastReload.Add(referenceHub.characterClassManager.UserId);
							}
							catch (Exception ex)
							{
								ServerConsole.AddLog("Exception occured during processing online player list for Fast Restart: " + ex.Message, ConsoleColor.Yellow);
							}
						}
					}
					NetworkServer.SendToAll<RoundRestartMessage>(new RoundRestartMessage(RoundRestartType.FastRestart, 0f, 0, true, true), 0, false);
					RoundRestart.ChangeLevel(false);

					return false;
				}
				if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.DoNothing)
				{
					float offset = (float)RoundRestart.LastRestartTime / 1000f;
					NetworkServer.SendToAll<RoundRestartMessage>(new RoundRestartMessage(RoundRestartType.FullRestart, offset, 0, true, true), 0, false);
				}

				RoundRestart.ChangeLevel(false);

				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("RoundRestartEvent error: {0}", arg));
			}
			return true;
		}
	}

}
