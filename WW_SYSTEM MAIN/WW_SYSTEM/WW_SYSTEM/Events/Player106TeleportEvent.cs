using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class Player106TeleportEvent : PlayerEvent
	{
	
		public Vector Position { get; set; }

	
		public Player106TeleportEvent(Player player, Vector position) : base(player)
		{
			this.Position = position;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler106Teleport)handler).On106Teleport(this);
		}
	}
}
