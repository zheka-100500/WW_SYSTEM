using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using PlayerRoles;

namespace WW_SYSTEM.Events
{
	public class TeamRespawnEvent : Event
	{

		public List<Player> PlayersToRespawn { get; set; }

		public bool IsChaos { get; set; }


		public bool Allow { get; set; }

		public bool UseCustomQueue { get; set; } = false;


		public Queue<RoleTypeId> CustomQueue { get; set; }





		public TeamRespawnEvent(List<Player> PlayersToRespawn, bool IsChaos, bool allow)
		{
		
			this.PlayersToRespawn = PlayersToRespawn;
			this.IsChaos = IsChaos;
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerTeamRespawn)handler).OnTeamRespawn(this);
		}
	}
}
