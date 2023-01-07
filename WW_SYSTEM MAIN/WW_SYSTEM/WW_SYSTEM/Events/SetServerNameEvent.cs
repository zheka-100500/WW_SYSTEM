using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class SetServerNameEvent : Event
	{
		
		public SetServerNameEvent(string ServerName)
		{
			this.ServerName = ServerName;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSetServerName)handler).OnSetServerName(this);
		}


		public string ServerName;
	}
}
