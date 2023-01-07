using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerPocketDimensionExitEvent : PlayerEvent
	{
		public Vector ExitPosition { get; set; }


		public PlayerPocketDimensionExitEvent(Player player, Vector exitPosition) : base(player)
		{
			this.ExitPosition = exitPosition;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPocketDimensionExit)handler).OnPocketDimensionExit(this);
		}
	}
}
