using HarmonyLib;
using LightContainmentZoneDecontamination;
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
	[HarmonyPatch(typeof(DecontaminationController), "FinishDecontamination")]
	public class LCZDecontaminateEventPatch
	{

		public static bool Prefix(DecontaminationController __instance)
		{

			try
			{
				if (NetworkServer.active)
				{
					EventManager.Manager.HandleEvent<IEventHandlerLCZDecontaminate>(new LCZDecontaminateEvent());

				}

					return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("LCZDecontaminateEvent error: {0}", arg));
			}
			return true;
		}
	}

}
