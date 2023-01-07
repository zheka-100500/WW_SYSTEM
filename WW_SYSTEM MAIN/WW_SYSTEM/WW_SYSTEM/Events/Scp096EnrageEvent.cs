using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class Scp096EnrageEvent : PlayerEvent
	{
		
		public bool Allow { get; set; }


		public Scp096EnrageEvent(Player player, bool allow) : base(player)
		{
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerScp096Enrage)handler).OnScp096Enrage(this);
		}
	}
}
