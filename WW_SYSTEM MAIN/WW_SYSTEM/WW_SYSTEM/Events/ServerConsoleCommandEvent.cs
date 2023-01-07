using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class ServerConsoleCommandEvent : Event
	{


		public string Command { get; set; }

		public string[] Args { get; set; }

		public string Output { get; set; }


		public bool Handled { get; set; }


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerCallConsoleCommand)handler).OnCallConsoleCommand(this);
		}
	}
}
