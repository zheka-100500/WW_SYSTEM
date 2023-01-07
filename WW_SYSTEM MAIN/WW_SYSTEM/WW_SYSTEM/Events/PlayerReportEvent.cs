using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerReportEvent : Event
	{

		public Player ReportOwner { get; }

		public Player ReportedPlayer { get; }

		public string Reason { get; }

		public bool Allow { get; set; }
		
		public REPORT_TYPE Type { get; }


		public PlayerReportEvent(Player ReportOwner, Player ReportedPlayer, string Reason, bool Allow, REPORT_TYPE Type)
		{

			this.ReportOwner = ReportOwner;
			this.ReportedPlayer = ReportedPlayer;
			this.Reason = Reason;
			this.Allow = Allow;
			this.Type = Type;


		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerReport)handler).OnReport(this);
		}
	}
}
