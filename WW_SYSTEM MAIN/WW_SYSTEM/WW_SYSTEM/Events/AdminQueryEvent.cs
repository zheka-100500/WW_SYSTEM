using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class AdminQueryEvent : Event
	{
		
		public Player Admin { get;}

		public ulong Permissions { get; }

		public string Query { get; }

		public string Output { get; set; }

	
		public bool Handled { get; set; }

		
		public bool Successful { get; set; }

		public AdminQueryEvent(Player Admin, string Query, string Output, bool Handled, bool Successful)
		{
			this.Permissions = Admin.Permissions;
			this.Admin = Admin;
			this.Query = Query;
			this.Output = Output;
			this.Handled = Handled;
			this.Successful = Successful;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerAdminQuery)handler).OnAdminQuery(this);
		}
	}
}
