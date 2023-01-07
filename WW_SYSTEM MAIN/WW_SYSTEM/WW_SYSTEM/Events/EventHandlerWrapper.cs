using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class EventHandlerWrapper
	{
		
		public Priority Priority { get; }


		public IEventHandler Handler { get; }

	
	
		public Plugin Plugin { get; }

		public EventHandlerWrapper(Plugin plugin, Priority priority, IEventHandler handler)
		{
			this.Plugin = plugin;
			this.Priority = priority;
			this.Handler = handler;
		}
	}
}
