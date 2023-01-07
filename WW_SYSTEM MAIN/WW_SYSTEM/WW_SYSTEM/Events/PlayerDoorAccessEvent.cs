using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using Interactables.Interobjects.DoorUtils;

namespace WW_SYSTEM.Events
{
	public class PlayerDoorAccessEvent : PlayerEvent
	{
	
		public ROOM_DOOR Door { get; }


		public bool Allow { get; set; }


		public bool ForceDeny { get; set; }

	
		public bool Destroy { get; set; }


		public PlayerDoorAccessEvent(Player player, ROOM_DOOR door) : base(player)
		{
			this.Door = door;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerDoorAccess)handler).OnDoorAccess(this);
		}
	}
}
