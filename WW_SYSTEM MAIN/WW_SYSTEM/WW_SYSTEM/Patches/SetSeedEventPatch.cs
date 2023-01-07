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
using MapGeneration;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(SeedSynchronizer), "Start")]
	public class SetSeedEventPatch
	{

		public static bool Prefix(SeedSynchronizer __instance)
		{

			try
			{
				if (!NetworkServer.active)
				{
					return false;
				}
				int num = ConfigFile.ServerConfig.GetInt("map_seed", -1);
				if (num < 1)
				{
					num = UnityEngine.Random.Range(1, int.MaxValue);
					SeedSynchronizer.DebugInfo("Server has successfully generated a random seed: " + num, MessageImportance.Normal, false);
				}
				else
				{
					SeedSynchronizer.DebugInfo("Server has successfully loaded a seed from config: " + num, MessageImportance.Normal, false);
				}
				SetSeedEvent setSeedEvent = new SetSeedEvent(num);
				EventManager.Manager.HandleEvent<IEventHandlerSetSeed>(setSeedEvent);
				num = setSeedEvent.Seed;
				__instance.Network_syncSeed = Mathf.Clamp(num, 1, int.MaxValue);
				Logger.Info("MAP", "SET MAP SEED TO: " + __instance.Network_syncSeed);
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("SetSeedEvent error: {0}", arg));
			}
			return true;
		}
	}

}
