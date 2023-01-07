using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class BanEvent : Event
	{

		public Player Player { get;}

		public Player Admin { get;}

	
		public long Duration { get; set; }

	
		public string Reason { get; set; }


		public bool AllowBan { get; set; }

		
		public BanEvent(Player Player, Player Admin, long Duration, string Reason, bool AllowBan)
		{
			this.Player = Player;
			this.Admin = Admin;
			this.Duration = Duration;
			this.Reason = Reason;
			this.AllowBan = AllowBan;

		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerBan)handler).OnBan(this);
		}
	}
}
