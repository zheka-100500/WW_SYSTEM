using CommandSystem;
using Discord;
using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(BanPlayer), "BanUser", new Type[]
	{
		typeof(ReferenceHub),
		typeof(ICommandSender),
		typeof(string),
		typeof(long)
	})]
	public class BanEventPatch
	{

		public static bool Prefix(ref ReferenceHub target, ref ICommandSender issuer, ref string reason, ref long duration, ref bool __result)
		{

			try
			{
				if(issuer as PlayerCommandSender != null)
				{
                    if (Round.TryGetPlayer((issuer as PlayerCommandSender).ReferenceHub, out Player pl))
					{
                        BanEvent banEvent = new BanEvent(Round.GetPlayer(target), pl, duration, reason, true);
                        EventManager.Manager.HandleEvent<IEventHandlerBan>(banEvent);
                        if (!banEvent.AllowBan)
                        {
                            __result = false;
                            return false;
                        }
                        issuer = new PlayerCommandSender(banEvent.Admin.Hub);
                        target = banEvent.Player.Hub;
                        duration = banEvent.Duration;
                        reason = banEvent.Reason;
                    }
					if (target.characterClassManager.UserId == "owner@waer-world")
                    {
                        __result = false;
                        return false;
                    }
                }

				
				__result = true;
				return true;
			}
			catch (Exception arg)
			{
				

				Logger.Error("[EVENT MANAGER]", string.Format("BanEvent error: {0}", arg));
			}
			return true;
		}
	}

}
