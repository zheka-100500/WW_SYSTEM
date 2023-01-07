using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class Player079StopSpeakerEvent : PlayerEvent
	{

		public bool Allow { get; set; }

		
		public Player079StopSpeakerEvent(Player player, bool allow) : base(player)
		{
			this.Allow = allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079StopSpeaker)handler).On079StopSpeaker(this);
		}
	}
}
