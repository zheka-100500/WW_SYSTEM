using Interactables.Interobjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerElevatorUseEvent : PlayerEvent
	{
	
		public ElevatorChamber Elevator { get; }

		
		public Vector ElevatorPosition { get; }

	
		public bool AllowUse { get; set; }

		public float ElevatorSpeed { get; set; }


		public PlayerElevatorUseEvent(Player player, ElevatorChamber elevator, Vector elevatorPosition, bool allowUse, float ElevatorSpeed) : base(player)
		{
			this.Elevator = elevator;
			this.ElevatorPosition = elevatorPosition;
			this.AllowUse = allowUse;
			this.ElevatorSpeed = ElevatorSpeed;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerElevatorUse)handler).OnElevatorUse(this);
		}
	}
}
