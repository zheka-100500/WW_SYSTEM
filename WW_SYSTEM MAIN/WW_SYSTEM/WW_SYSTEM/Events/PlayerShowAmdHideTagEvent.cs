using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class PlayerShowAmdHideTagEvent : PlayerEvent
    {
		public bool Allow { get; set; }

		public TagEventType type { get; }

		public bool Global { get; }


		public PlayerShowAmdHideTagEvent(Player Player, TagEventType type, bool Global, bool Allow) : base(Player)
		{


			this.type = type;
			this.Allow = Allow;
			this.Global = Global;


		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerShowAndHideTag)handler).OnRequestTag(this);
		}
	}
}
