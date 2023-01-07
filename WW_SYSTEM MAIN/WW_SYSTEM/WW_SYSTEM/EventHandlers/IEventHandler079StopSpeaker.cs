using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.EventHandlers
{
	public interface IEventHandler079StopSpeaker : IEventHandler
	{
	
		void On079StopSpeaker(Player079StopSpeakerEvent ev);
	}
}
