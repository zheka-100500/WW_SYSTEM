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

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(Broadcast), "RpcAddElement")]
	public class ServerBroadcastEventPatch
	{

		public static bool Prefix(ref string data, ushort time, Broadcast.BroadcastFlags flags, Broadcast __instance)
		{

			try
			{
				ServerBroadcastEvent serverBroadcastEvent = new ServerBroadcastEvent(true, data);
				EventManager.Manager.HandleEvent<IEventHandlerServerBroadcast>(serverBroadcastEvent);
				if (!serverBroadcastEvent.Allow)
				{
					return false;
				}
				data = serverBroadcastEvent.BroadcastMessage;
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("ServerBroadcastEvent error: {0}", arg));
			}
			return true;
		}
	}

}
