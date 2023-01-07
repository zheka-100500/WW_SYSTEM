using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class DisconnectEvent : Event
	{

		public string IpAddress { get; }

		public DisconnectEvent(string IpAddress) : base()
		{
			this.IpAddress = IpAddress;
		}



		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerDisconnect)handler).OnDisconnect(this);
		}
	}
}
