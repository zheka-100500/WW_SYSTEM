 using HarmonyLib;
using MEC;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	//[HarmonyPatch(typeof(Intercom), "RequestTransmission")]
	public class PlayerIntercomCooldownCheckEventPatch
	{

		//public static bool Prefix(GameObject spk, Intercom __instance)
		//{

		//	try
		//	{
		//		if (spk == null)
		//		{
		//			__instance.Networkspeaker = null;
		//			return false;
		//		}
		//		if (spk.GetComponent<Player>() != null)
		//		{
		//			PlayerIntercomCooldownCheckEvent playerIntercomCooldownCheckEvent = new PlayerIntercomCooldownCheckEvent(spk.GetComponent<Player>(), __instance.remainingCooldown);
		//			EventManager.Manager.HandleEvent<IEventHandlerIntercomCooldownCheck>(playerIntercomCooldownCheckEvent);
		//			__instance.remainingCooldown = playerIntercomCooldownCheckEvent.CurrentCooldown;
		//		}
		//		if ((__instance.remainingCooldown <= 0f && !__instance._inUse) || (spk.GetComponent<ServerRoles>().BypassMode && !__instance.speaking))
		//		{
		//			__instance.speaking = true;
		//			__instance.remainingCooldown = -1f;
		//			__instance._inUse = true;
		//			if (spk.GetComponent<Player>() != null)
		//			{
		//				PlayerIntercomEvent ev = new PlayerIntercomEvent(spk.GetComponent<Player>());
		//				EventManager.Manager.HandleEvent<IEventHandlerIntercom>(ev);
		//			}
		//			Timing.RunCoroutine(__instance._StartTransmitting(spk), Segment.FixedUpdate);
		//		}
		//		return false;
		//	}
		//	catch (Exception arg)
		//	{


		//		Logger.Error("[EVENT MANAGER]", string.Format("PlayerIntercomCooldownCheckEvent or PlayerIntercomEvent error: {0}", arg));
		//	}
		//	return true;
		//}
	}

}
