using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class FixedUpdateEvent : Event
	{
	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerFixedUpdate)handler).OnFixedUpdate(this);
		}
	}
}
