using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class PlayerUsedMedicalItemEvent : Event
    {
		public Player Player { get; }

		public ItemType Type { get; }


		public PlayerUsedMedicalItemEvent(Player Player, ItemType Type)
		{
			this.Player = Player;
			this.Type = Type;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerUsedMedicalItem)handler).OnUsedMedicalItem(this);
		}
	}
}
