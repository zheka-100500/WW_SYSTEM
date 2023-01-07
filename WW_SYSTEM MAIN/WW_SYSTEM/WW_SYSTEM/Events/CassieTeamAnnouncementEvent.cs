using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class CassieTeamAnnouncementEvent : Event
	{

		public char NatoLetter { get; }

		public int NatoNumber { get; }


		public int SCPsLeft { get; }

	
		public bool Allow { get; set; }


		public CassieTeamAnnouncementEvent(char natoLetter, int natoNumber, int scpsLeft, bool allow = true)
		{
			this.NatoLetter = natoLetter;
			this.NatoNumber = natoNumber;
			this.SCPsLeft = scpsLeft;
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerCassieTeamAnnouncement)handler).OnCassieTeamAnnouncement(this);
		}
	}
}
