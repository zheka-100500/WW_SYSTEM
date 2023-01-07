using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class CassieCustomAnnouncementEvent : Event
	{

		public string Words { get; set; }

		
		public bool MonoSpaced { get; set; }

		
		public bool Allow { get; set; }


		public CassieCustomAnnouncementEvent(string words, bool monospaced, bool allow = true)
		{
			this.Words = words;
			this.MonoSpaced = monospaced;
			this.Allow = allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerCassieCustomAnnouncement)handler).OnCassieCustomAnnouncement(this);
		}
	}
}
