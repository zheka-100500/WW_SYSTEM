using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PersonalBroadcastEvent : Event
	{

		public bool Allow { get; set; }

		public string BroadcastMessage { get; set; }

		public Player Player { get; }


		public PersonalBroadcastEvent(bool Allow, string BroadcastMessage, Player Player)
		{
			this.Allow = Allow;
			this.BroadcastMessage = BroadcastMessage;
			this.Player = Player;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPersonalBroadcast)handler).OnPersonalBroadcast(this);
		}
	}
}
