using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class PlayerChangeXPEvent : Event
    {
		public bool Allow { get; set; }

		public bool IsAdd { get; }

		public int XpCount { get; set; }

		public Player Player { get; }

		public PlayerChangeXPEvent(bool IsAdd, bool Allow, int XpCount, Player Player)
		{
			this.IsAdd = IsAdd;
			this.Allow = Allow;
			this.XpCount = XpCount;
			this.Player = Player;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerChangeXP)handler).OnPlayerChangeXP(this);
		}
	}
}
