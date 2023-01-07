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
	[HarmonyPatch(typeof(AlphaWarheadController), "CancelDetonation", new Type[]
	{
		typeof(ReferenceHub)
	})]
	public class WarheadStopEventPatch
	{

		public static bool Prefix(ReferenceHub disabler, AlphaWarheadController __instance)
		{

			try
			{
				if (!AlphaWarheadController.InProgress || AlphaWarheadController.TimeUntilDetonation <= 10f || __instance.IsLocked)
				{
					return false;
				}
				if (Round.WarheadLocked)
				{
					return false;
				}
				WarheadStopEvent warheadStopEvent = new WarheadStopEvent((disabler != null) ? disabler.GetComponent<Player>() : null, AlphaWarheadController.TimeUntilDetonation);
				EventManager.Manager.HandleEvent<IEventHandlerWarheadStopCountdown>(warheadStopEvent);
				if (warheadStopEvent.Cancel)
				{
					return false;
				}
				return true;
			}
			catch (Exception arg)
			{

				Logger.Error("[EVENT MANAGER]", string.Format("WarheadStopEvent error: {0}", arg));
			}
			return true;
		}
	}

}
