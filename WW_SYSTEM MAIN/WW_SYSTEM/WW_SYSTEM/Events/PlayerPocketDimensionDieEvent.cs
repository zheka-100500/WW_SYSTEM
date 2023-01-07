using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using UnityEngine;

namespace WW_SYSTEM.Events
{
	public class PlayerPocketDimensionDieEvent : PlayerEvent
	{

		public bool ForceExit { get; set; } = false;
		public Vector3 CustomTeleportPosition { get; set; } = Vector3.zero;
		public PlayerPocketDimensionDieEvent(Player player) : base(player)
		{
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPocketDimensionDie)handler).OnPocketDimensionDie(this);
		}
	}
}
