using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Custom_Events;

namespace WW_SYSTEM.Events
{
	public class RoundEndEvent : Event
	{
		public int RoundCount { get; }


		public RoundEndEvent(int RoundCount)
		{
			this.RoundCount = RoundCount;
			if (CustomEventManager.EventInProgress)
			{
				CustomEventManager.CurrentEvent.OnRoundEnd(this.RoundCount);
			}
		}




		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerRoundEnd)handler).OnRoundEnd(this);
		}


	}
}
