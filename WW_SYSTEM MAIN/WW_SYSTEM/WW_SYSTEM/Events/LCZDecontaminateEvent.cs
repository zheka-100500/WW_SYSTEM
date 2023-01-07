﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class LCZDecontaminateEvent : Event
	{
		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerLCZDecontaminate)handler).OnDecontaminate();
		}
	}
}
