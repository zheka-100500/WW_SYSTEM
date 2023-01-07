using HarmonyLib;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(FirstPersonMovementModule), "FixedUpdate")]
	public class FixedUpdateEventPatch
	{

		public static bool Prefix(FirstPersonMovementModule __instance)
		{

			try
			{
				if (__instance.Hub.isServer && __instance.Hub.isLocalPlayer)
				{
					EventManager.Manager.HandleEvent<IEventHandlerFixedUpdate>(new FixedUpdateEvent());
				}
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("FixedUpdateEvent error: {0}", arg));
			}
			return true;
		}
	}

}
