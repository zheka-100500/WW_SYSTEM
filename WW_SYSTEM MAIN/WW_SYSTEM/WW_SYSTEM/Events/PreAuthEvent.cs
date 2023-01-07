using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using Mirror;
using LiteNetLib;

namespace WW_SYSTEM.Events
{
	public class PreAuthEvent : Event
	{
		public bool ForceAllow { get; set; }

		public string UserID { get; }

		public string IP { get; }
		public bool ForceDeny { get; set; }

		public PreAuthEvent(bool ForceAllow, string UserID, string IP)
		{
			ForceDeny = false;
			this.ForceAllow = ForceAllow;
			this.UserID = UserID;
			this.IP = IP;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPreAuth)handler).OnPreAuth(this);
		}
	}
}
