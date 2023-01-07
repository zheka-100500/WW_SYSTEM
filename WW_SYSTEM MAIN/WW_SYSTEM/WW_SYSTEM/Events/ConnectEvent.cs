using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using WW_SYSTEM.EventHandlers;
using System.Net;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Events
{
	public class ConnectEvent : Event
	{
		
		public string IpAddress { get; }

		public bool IsBanned
		{
			get
			{
				return BanHandler.QueryBan(null, this.IpAddress).Value != null;
			}
		}

		public ConnectEvent(string IpAddress) : base()
		{
			this.IpAddress = IpAddress;
		}

		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerConnect)handler).OnConnect(this);
		}
	}
}
