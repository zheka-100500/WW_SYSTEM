using HarmonyLib;
using PlayerRoles;
using RemoteAdmin;
using Respawning.NamingRules;
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
	[HarmonyPatch(typeof(NineTailedFoxNamingRule), "PlayEntranceAnnouncement")]
	public class CassieTeamAnnouncementEventPatch
	{

		public static bool Prefix(string regular)
		{
			
			try
			{
				string[] array = regular.Split(new char[]
			{
					'-'
			});
		



				int _scpsLeft = (from x in Server.Players
						   where x.CheckForTeam(TEAMTYPE.SCP) && x.Role != RoleTypeId.Scp0492
						   select x).Count();
				int _mtfNumber = Convert.ToInt32(array[1]);
				char _mtfLetter = array[0][0];

				CassieTeamAnnouncementEvent cassieTeamAnnouncementEvent = new CassieTeamAnnouncementEvent(_mtfLetter, _mtfNumber, _scpsLeft, true);
				EventManager.Manager.HandleEvent<IEventHandlerCassieTeamAnnouncement>(cassieTeamAnnouncementEvent);
				if (cassieTeamAnnouncementEvent.Allow)
				{
					return true;
				}
				else
				{
					return false;
				}
			
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("CassieTeamAnnouncementEvent error: {0}", arg));
			}
			return true;
		}
	}

}
