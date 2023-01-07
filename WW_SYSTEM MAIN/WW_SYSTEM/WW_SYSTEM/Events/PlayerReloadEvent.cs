using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using InventorySystem.Items;

namespace WW_SYSTEM.Events
{
	public class PlayerReloadEvent : PlayerEvent
	{

		public ItemIdentifier Item { get; }
		public bool Allow { get; set; }

		public PlayerReloadEvent(Player player, ItemIdentifier item) : base(player)
		{
			this.Item = item;
			Allow = true;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerReload)handler).OnReload(this);
		}
	}
}
