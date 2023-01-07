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
	[HarmonyPatch(typeof(CharacterClassManager), "Update")]
	public class UpdateEventPatch
	{

		public static bool Prefix(CharacterClassManager __instance)
		{

			try
			{
				if (__instance.isServer && __instance.isLocalPlayer)
				{
					EventManager.Manager.HandleEvent<IEventHandlerUpdate>(new UpdateEvent());
				}
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("UpdateEvent error: {0}", arg));
			}
			return true;
		}
	}

}
