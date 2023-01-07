using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class Player079StartSpeakerEvent : PlayerEvent
	{



		public bool Allow { get; set; }

	


		public Player079StartSpeakerEvent(Player player, bool allow) : base(player)
		{
			this.Allow = allow;
		}

		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079StartSpeaker)handler).On079StartSpeaker(this);
		}
	}
}
