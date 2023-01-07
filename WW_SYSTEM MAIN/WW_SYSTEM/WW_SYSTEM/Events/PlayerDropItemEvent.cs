using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerDropItemEvent : Event
	{
		public Player Player { get; }
		public ItemType Item { get; }
		public bool allow { get; set; }
		

		public PlayerDropItemEvent(Player player, ItemType item, bool allow)
		{
			this.Player = player;
			this.Item = item;
			this.allow = allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerDropItem)handler).OnPlayerDropItem(this);
		}
	}
}
