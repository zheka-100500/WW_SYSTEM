using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class Player079ElevatorEvent : PlayerEvent
	{
	
		


		public bool Allow { get; set; }

	

	
		public Player079ElevatorEvent(Player player, bool allow) : base(player)
		{
		

			this.Allow = allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079Elevator)handler).On079Elevator(this);
		}
	}
}
