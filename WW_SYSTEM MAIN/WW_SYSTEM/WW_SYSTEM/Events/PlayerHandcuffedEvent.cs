using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerHandcuffedEvent : PlayerEvent
	{
		public bool Allow { get; set; }


		public Player Owner { get; set; }

		public DisarmType DisarmType { get; }

		public PlayerHandcuffedEvent(Player player, Player owner, DisarmType disarmType, bool allow = true) : base(player)
		{
			this.Allow = allow;
			this.Owner = owner;
			DisarmType = disarmType;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerHandcuffed)handler).OnHandcuffed(this);
		}
	}
}
