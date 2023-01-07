using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Custom_Events;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Events
{
	public class RoundStartEvent :Event
	{

		public int RoundCount { get; }


		public RoundStartEvent(int RoundCount)
		{
			Round.OverWatchEnabledUserIds.Clear();
			this.RoundCount = RoundCount;
			if (CustomEventManager.EventInProgress)
			{
				CustomEventManager.CurrentEvent.OnRoundStart(this.RoundCount);
			}
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerRoundStart)handler).OnRoundStart(this);
		}
	}
}
