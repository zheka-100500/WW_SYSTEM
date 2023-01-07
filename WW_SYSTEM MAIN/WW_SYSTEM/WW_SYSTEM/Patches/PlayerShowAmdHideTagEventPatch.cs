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
    public class PlayerShowAmdHideTagEventPatch
    {


		[HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRequestShowTag")]
		public class PlayerShowTagEventPatch
		{

			public static bool Prefix(bool global, CharacterClassManager __instance)
			{

				try
				{
					if (!__instance._commandRateLimit.CanExecute(true))
					{
						return false;
					}
					PlayerShowAmdHideTagEvent playerShowAmdHideTagEvent = new PlayerShowAmdHideTagEvent(__instance.GetComponent<Player>(), TagEventType.SHOW, global, true);
					EventManager.Manager.HandleEvent<IEventHandlerPlayerShowAndHideTag>(playerShowAmdHideTagEvent);
					if (!playerShowAmdHideTagEvent.Allow)
					{
						__instance.ConsolePrint("[WW SYSTEM] ACCESS DENIED.", "red");
						return false;
					}
					return true;
				}
				catch (Exception arg)
				{


					Logger.Error("[EVENT MANAGER]", string.Format("PlayerShowAmdHideTagEvent error: {0}", arg));
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRequestHideTag")]
		public class PlayerHideTagEventPatch
		{

			public static bool Prefix(CharacterClassManager __instance)
			{

				try
				{
					if (!__instance._commandRateLimit.CanExecute(true))
					{
						return false;
					}
					PlayerShowAmdHideTagEvent playerShowAmdHideTagEvent = new PlayerShowAmdHideTagEvent(__instance.GetComponent<Player>(), TagEventType.HIDE, false, true);
					EventManager.Manager.HandleEvent<IEventHandlerPlayerShowAndHideTag>(playerShowAmdHideTagEvent);
					if (!playerShowAmdHideTagEvent.Allow)
					{
						__instance.ConsolePrint("[WW SYSTEM] ACCESS DENIED.", "red");
						return false;
					}
					return true;
				}
				catch (Exception arg)
				{


					Logger.Error("[EVENT MANAGER]", string.Format("PlayerShowAmdHideTagEvent error: {0}", arg));
				}
				return true;
			}
		}

	}
}
