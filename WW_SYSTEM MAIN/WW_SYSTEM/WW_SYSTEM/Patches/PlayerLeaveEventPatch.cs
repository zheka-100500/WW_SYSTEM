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

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ReferenceHub), "OnDestroy")]
	public class PlayerLeaveEventPatch
	{

		public static bool Prefix(ReferenceHub __instance)
		{

			try
			{
				if (!string.IsNullOrEmpty(__instance.characterClassManager.UserId))
				{
					var pl = __instance.characterClassManager.GetComponent<Player>();
					PlayerLeaveEvent ev = new PlayerLeaveEvent(pl);
					EventManager.Manager.HandleEvent<IEventHandlerPlayerLeave>(ev);
					Server.Players.Remove(pl);
				}

				return true;
			}
			catch (Exception arg)
			{

				Logger.Error("[EVENT MANAGER]", string.Format("PlayerLeaveEvent error: {0}", arg));
			}
			return true;
		}
	}

}
