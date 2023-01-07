using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerThrowGrenadeEvent : Event
	{
		public Player Player { get; }
		public float UpwardFactor { get; set; }
		public float ForceAmount { get; set; }
		public ItemType Type { get; }
		public Vector3 Torque { get; }
		public Vector3 StartVel { get; }
		public bool Allow { get; set; }


		public PlayerThrowGrenadeEvent(Player Player, float upwardFactor, float ForceAmount, Vector3 torque, Vector3 startVel, ItemType Type, bool Allow)
		{
			this.Player = Player;
			this.UpwardFactor = upwardFactor;
			this.ForceAmount = ForceAmount;
			this.Type = Type;
			this.Allow = Allow;
			Torque = torque;
			StartVel = startVel;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerThrowGrenade)handler).OnThrowGrenade(this);
		}
	}
}
