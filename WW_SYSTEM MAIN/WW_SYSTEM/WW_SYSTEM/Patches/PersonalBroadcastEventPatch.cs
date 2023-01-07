using HarmonyLib;
using Mirror;
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
	[HarmonyPatch(typeof(Broadcast), "TargetAddElement")]
	public class PersonalBroadcastEventPatch
	{

		public static bool Prefix(NetworkConnection conn, ref string data, ushort time, Broadcast.BroadcastFlags flags, Broadcast __instance)
		{

			try
			{
				PersonalBroadcastEvent personalBroadcastEvent = new PersonalBroadcastEvent(true, data, conn.identity.gameObject.GetComponent<Player>());
				EventManager.Manager.HandleEvent<IEventHandlerPersonalBroadcast>(personalBroadcastEvent);
				if (!personalBroadcastEvent.Allow)
				{
					return false;
				}
				data = personalBroadcastEvent.BroadcastMessage;
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PersonalBroadcastEvent error: {0}", arg));
			}
			return true;
		}
	}

}
