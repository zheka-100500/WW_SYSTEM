﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class WarheadStopEvent : WarheadEvent
	{
	
		public WarheadStopEvent(Player player, float timeLeft) : base(player, timeLeft)
		{
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerWarheadStopCountdown)handler).OnStopCountdown(this);
		}
	}
}
