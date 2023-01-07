using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using InventorySystem.Items.Pickups;

namespace WW_SYSTEM.Events
{
	public class PlayerPickupItemEvent : PlayerEvent
	{

		public ItemType Item { get; set; }
		public ItemPickupBase ItemPickup { get; }
		public ItemCategory Category { get; }
		public bool Allow { get; set; }
		public bool ShowDenyPickupMsg { get; set; }
		
		public PlayerPickupItemEvent(Player player, ItemType item, ItemPickupBase itemPickup, ItemCategory category, bool allow, bool showDenyMsg) : base(player)
		{

			Item = item;
			ItemPickup = itemPickup;
			Category = category;
			Allow = allow;
			ShowDenyPickupMsg = showDenyMsg;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerPickupItem)handler).OnPlayerPickupItem(this);
		}
	}
}
