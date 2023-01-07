using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerSCP914ChangeKnobEvent : PlayerEvent
	{
		
		public KnobSetting KnobSetting { get; }
		public bool Allow { get; set; }

		public PlayerSCP914ChangeKnobEvent(Player player, KnobSetting knobSetting, bool Allow) : base(player)
		{
			this.KnobSetting = knobSetting;
			this.Allow = Allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSCP914ChangeKnob)handler).OnSCP914ChangeKnob(this);
		}
	}
}
