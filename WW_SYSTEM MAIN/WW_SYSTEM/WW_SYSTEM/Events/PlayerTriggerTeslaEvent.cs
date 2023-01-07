using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerTriggerTeslaEvent : PlayerEvent
	{

		


		public bool Triggerable { get; set; }


		public PlayerTriggerTeslaEvent(Player player, bool triggerable) : base(player)
		{
		
			this.Triggerable = triggerable;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerTriggerTesla)handler).OnPlayerTriggerTesla(this);

		}
	}
}
