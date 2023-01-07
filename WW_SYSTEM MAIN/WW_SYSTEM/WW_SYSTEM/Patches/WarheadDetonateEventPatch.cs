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
	[HarmonyPatch(typeof(AlphaWarheadController), "Detonate")]
	public class WarheadDetonateEventPatch
	{

		public static bool Prefix(AlphaWarheadController __instance)
		{

			try
			{
				WarheadDetonateEvent ev = new WarheadDetonateEvent();
				EventManager.Manager.HandleEvent<IEventHandlerWarheadDetonate>(ev);
				Round.WarheadLocked = false;
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("WarheadDetonateEvent error: {0}", arg));
			}
			return true;
		}
	}

}
