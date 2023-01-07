using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerDropAllItemsEvent : PlayerEvent
	{
	
		public bool Allow { get; set; }

	
		public PlayerDropAllItemsEvent(Player player, bool allow = true) : base(player)
		{
			this.Allow = allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerDropAllItems)handler).OnPlayerDropAllItems(this);
		}
	}
}
