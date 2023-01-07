using Interactables.Interobjects.DoorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
 
namespace WW_SYSTEM.Events
{
	public class Player079DoorEvent : PlayerEvent
	{
		
		public ROOM_DOOR Door { get; }

		public bool Allow { get; set; }
	
		public Player079DoorEvent(Player player, ROOM_DOOR door, bool allow) : base(player)
		{
			this.Door = door;
			this.Allow = allow;
		}

		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079Door)handler).On079Door(this);
		}
	}
}
