using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class Scp096PanicEvent : PlayerEvent
	{
		
		public bool Allow { get; set; }

	
		public float PanicTime { get; set; }


		public Scp096PanicEvent(Player player, bool allow, float panicTime) : base(player)
		{
			this.Allow = allow;
			this.PanicTime = panicTime;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerScp096Panic)handler).OnScp096Panic(this);
		}
	}
}
