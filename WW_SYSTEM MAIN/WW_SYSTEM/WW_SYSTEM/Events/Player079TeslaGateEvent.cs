using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class Player079TeslaGateEvent : PlayerEvent
	{
		public TeslaGate TeslaGate { get; }

		public bool Allow { get; set; }


		public Player079TeslaGateEvent(Player player, TeslaGate teslaGate, bool allow) : base(player)
		{
			this.TeslaGate = teslaGate;
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079TeslaGate)handler).On079TeslaGate(this);
		}
	}
}
