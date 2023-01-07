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
using PlayerStatsSystem;
using PlayerRoles;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
	public class ScpDeathAnnouncementEventPatch
	{

		public static bool Prefix(ReferenceHub scp, DamageHandlerBase hit, NineTailedFoxAnnouncer __instance)
		{

			try
			{


                NineTailedFoxAnnouncer.singleton.scpListTimer = 0f;
                if (!scp.IsSCP(false))
                {
                    return false;
                }

                ScpDeathAnnouncementEvent scpDeathAnnouncementEvent = new ScpDeathAnnouncementEvent(scp.GetRoleId(), hit, true);
				EventManager.Manager.HandleEvent<IEventHandlerScpDeathAnnouncement>(scpDeathAnnouncementEvent);
				if (!scpDeathAnnouncementEvent.Allow)
				{
					return false;
				}

				string cassieDeathAnnouncement = hit.CassieDeathAnnouncement.Announcement;
				if (string.IsNullOrEmpty(cassieDeathAnnouncement))
				{
					return false;
				}

                foreach (NineTailedFoxAnnouncer.ScpDeath scpDeath in NineTailedFoxAnnouncer.scpDeaths)
                {
                    if (!(scpDeath.announcement != cassieDeathAnnouncement))
                    {
                        scpDeath.scpSubjects.Add(scp.GetRoleId());
                        return false;
                    }
                }
                NineTailedFoxAnnouncer.scpDeaths.Add(new NineTailedFoxAnnouncer.ScpDeath
                {
                    scpSubjects = new List<RoleTypeId>(new RoleTypeId[]
                    {
                scp.GetRoleId()
                    }),
                    announcement = cassieDeathAnnouncement,
                    subtitleParts = hit.CassieDeathAnnouncement.SubtitleParts
                });



                return false;




			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("ScpDeathAnnouncementEvent error: {0}", arg));
			}
			return true;
		}
	}

}
