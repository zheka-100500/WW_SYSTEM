using Footprinting;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using RemoteAdmin;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using Time = UnityEngine.Time;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow), new Type[]
	{
		typeof(float),
		typeof(float),
		typeof(Vector3),
		typeof(Vector3)
	})]
	public class PlayerThrowGrenadeEventPatch
	{

		public static bool Prefix(ref float forceAmount, ref float upwardFactor, ref Vector3 torque, ref Vector3 startVel, ThrowableItem __instance)
		{

			try
			{
				if(Round.TryGetPlayer(__instance.Owner, out var pl))
				{
					PlayerThrowGrenadeEvent ev = new PlayerThrowGrenadeEvent(pl, upwardFactor, forceAmount, torque, startVel, __instance.ItemTypeId, true);
					EventManager.Manager.HandleEvent<IEventHandlerThrowGrenade>(ev);
					if (!ev.Allow) return false;
					forceAmount = ev.ForceAmount;
					upwardFactor = ev.UpwardFactor;
					torque = ev.Torque;
					startVel = ev.StartVel;
				}



				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerThrowGrenadeEventPatch error: {0}", arg));
			}
			return true;
		}
	}
}
