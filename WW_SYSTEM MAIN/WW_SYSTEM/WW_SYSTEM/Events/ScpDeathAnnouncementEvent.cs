using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using PlayerStatsSystem;
using PlayerRoles;

namespace WW_SYSTEM.Events
{
	public class ScpDeathAnnouncementEvent : Event
	{
		


	
		

		public bool Allow { get; set; }
		
		public RoleTypeId PlayerRole { get; }

		public DamageHandlerBase info { get; }

			public ScpDeathAnnouncementEvent(RoleTypeId playerRole, DamageHandlerBase info, bool Allow)
		{

			this.PlayerRole = playerRole;
			this.Allow = Allow;
			this.info = info;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerScpDeathAnnouncement)handler).OnScpDeathAnnouncement(this);
		}
	}
}
