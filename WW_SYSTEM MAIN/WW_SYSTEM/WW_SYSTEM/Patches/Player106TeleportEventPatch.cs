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
	//[HarmonyPatch(typeof(Scp106PlayerScript), "UserCode_CmdUsePortal")]
	//public class Player106TeleportEventPatch
	//{

	//	public static bool Prefix(Scp106PlayerScript __instance)
	//	{

	//		try
	//		{
	//			if (!__instance._interactRateLimit.CanExecute(true) || !__instance._hub.playerMovementSync.Grounded)
	//			{
	//				return false;
	//			}
	//			if (__instance.iAm106 && __instance.portalPosition != Vector3.zero && !__instance.goingViaThePortal)
	//			{
	//				Vector3 vector = __instance.portalPrefab.transform.position;
	//				Player106TeleportEvent player106TeleportEvent = new Player106TeleportEvent(__instance.gameObject.GetComponent<Player>(), new Vector(vector.x, vector.y, vector.z));
	//				EventManager.Manager.HandleEvent<IEventHandler106Teleport>(player106TeleportEvent);
	//				vector = player106TeleportEvent.Position.ToVector3();
	//				__instance.portalPrefab.transform.position = vector;
	//				Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);
	//			}

	//			return false;
	//		}
	//		catch (Exception arg)
	//		{


	//			Logger.Error("[EVENT MANAGER]", string.Format("Player106TeleportEvent error: {0}", arg));
	//		}
	//		return true;
	//	}
	//}

}
