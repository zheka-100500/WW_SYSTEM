using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerCheckLimitEvent : Event
	{
		public byte Limit { get; set; }
		public ItemType Type { get; }
		public Player Player { get; }
		public bool Allow { get; set; }
		public PlayerCheckLimitEvent(Player Player, ItemType Type, byte Limit, bool Allow)
		{
			this.Limit = Limit;
			this.Type = Type;
			this.Player = Player;
			this.Allow = Allow;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerCheckLimit)handler).OnCheckLimit(this);
		}
	}
}
