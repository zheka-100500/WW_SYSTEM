using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerLureEvent : PlayerEvent
	{
	
		public bool AllowContain { get; set; }

		public PlayerLureEvent(Player player, bool allowContain) : base(player)
		{
			this.AllowContain = allowContain;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerLure)handler).OnLure(this);
		}
	}
}
