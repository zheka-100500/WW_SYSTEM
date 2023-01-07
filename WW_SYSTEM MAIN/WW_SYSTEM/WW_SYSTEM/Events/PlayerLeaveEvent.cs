using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Events;
namespace WW_SYSTEM.Events
{
	public class PlayerLeaveEvent : Event
	{
		
		public Player Player { get; }

	
		public PlayerLeaveEvent(Player player)
		{
			if (player != null && player.Overwatch)
			{
				if(!Round.OverWatchEnabledUserIds.Contains(player.UserId))
				Round.OverWatchEnabledUserIds.Add(player.UserId);

			}
			this.Player = player;
			if (CustomEventManager.EventInProgress)
			{
				CustomEventManager.CurrentEvent.OnPlayerLeave(player);
			}
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerLeave)handler).OnPlayerLeave(this);
		}
	}
}
