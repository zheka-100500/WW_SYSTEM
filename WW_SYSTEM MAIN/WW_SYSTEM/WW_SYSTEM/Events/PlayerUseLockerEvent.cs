using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerUseLockerEvent : Event
	{

		public Player Player { get; }




		public bool Allow { get; set; }


		public PlayerUseLockerEvent(Player Player, bool allow)
		{
			this.Player = Player;
		
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerUseLocker)handler).OnUseLocker(this);
		}
	}
}
