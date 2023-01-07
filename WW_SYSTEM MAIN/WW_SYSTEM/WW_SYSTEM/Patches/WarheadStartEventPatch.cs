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
	[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
	public class WarheadStartEventPatch
	{

		public static bool Prefix(AlphaWarheadController __instance, bool isAutomatic = false, bool suppressSubtitles = false, ReferenceHub trigger = null)
		{

			try
			{
				Round.StartWarheadAdv(false, !Round.WarheadLocked, trigger);
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("WarheadStartEvent error: {0}", arg));
			}
			return true;
		}
	}

}
