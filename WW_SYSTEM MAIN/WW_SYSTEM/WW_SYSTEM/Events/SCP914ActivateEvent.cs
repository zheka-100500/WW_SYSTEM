using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class SCP914ActivateEvent : Event
	{
		
		public Player User { get; }

		public KnobSetting KnobSetting { get; set; }

		public bool Allow { get; set; }

		public SCP914ActivateEvent(Player user, KnobSetting knobSetting, bool Allow)
		{
			this.User = user;
			this.KnobSetting = knobSetting;
			this.Allow = Allow;

		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSCP914Activate)handler).OnSCP914Activate(this);
		}
	}
}
