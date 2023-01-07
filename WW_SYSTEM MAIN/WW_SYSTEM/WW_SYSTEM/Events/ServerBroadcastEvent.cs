using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class ServerBroadcastEvent : Event
	{
		
		public bool Allow { get; set; }

		public string BroadcastMessage { get; set; }


	public ServerBroadcastEvent(bool Allow, string BroadcastMessage)
	{
			this.Allow = Allow;
			this.BroadcastMessage = BroadcastMessage;
	}


	public override void ExecuteHandler(IEventHandler handler)
	{
	((IEventHandlerServerBroadcast)handler).OnServerBroadcast(this);
	}
	}
}
