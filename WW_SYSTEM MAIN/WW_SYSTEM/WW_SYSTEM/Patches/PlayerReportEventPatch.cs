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
	[HarmonyPatch(typeof(CheaterReport), "UserCode_CmdReport")]
	public class PlayerReportEventPatch
	{

		public static bool Prefix(uint playerNetId, string reason, byte[] signature, bool notifyGm, CheaterReport __instance)
		{

			try
			{
				if (!__instance._commandRateLimit.CanExecute(true))
				{
					return false;
				}
				if (reason == null || signature == null)
				{
					return false;
				}
				Player component = __instance.gameObject.GetComponent<Player>();
				Player player = Round.GetPlayer(playerNetId);
				if (component != null && player != null)
				{
					REPORT_TYPE type = notifyGm ? REPORT_TYPE.CHEATER : REPORT_TYPE.SERVER_RULES;
					PlayerReportEvent playerReportEvent = new PlayerReportEvent(component, player, reason, true, type);
					EventManager.Manager.HandleEvent<IEventHandlerPlayerReport>(playerReportEvent);
					if (!playerReportEvent.Allow)
					{
						return false;
					}
				}

				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerReportEvent error: {0}", arg));
			}
			return true;
		}
	}

}
