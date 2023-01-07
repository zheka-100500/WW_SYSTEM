using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class WarheadStartEvent : WarheadEvent
	{
		
		public WarheadStartEvent(Player activator, float timeLeft, bool isResumed, bool openDoorsAfter) : base(activator, timeLeft)
		{
			this.IsResumed = isResumed;
			this.OpenDoorsAfter = openDoorsAfter;
		}

		public bool IsResumed { get; set; }

		public bool OpenDoorsAfter { get; set; }


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerWarheadStartCountdown)handler).OnStartCountdown(this);
		}
	}
}
