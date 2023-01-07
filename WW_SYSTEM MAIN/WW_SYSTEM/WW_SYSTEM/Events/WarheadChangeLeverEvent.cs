using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class WarheadChangeLeverEvent : Event
	{
		public bool AllowChange { get; set; }

		public Player Player { get; }

		public WarheadChangeLeverEvent(Player player, bool AllowChange)
		{
			this.Player = player;
			this.AllowChange = AllowChange;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerWarheadChangeLever)handler).OnWarheadChange(this);
		}
	}
}
