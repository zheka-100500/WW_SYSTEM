using InventorySystem.Items.Firearms;
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
	public class PlayerShootEvent : PlayerEvent
	{

        public float Distance { get; }
        public float Damage { get; set; }
		public IDestructible Component { get; set; }
		public Firearm Firearm { get; }
		public RaycastHit Hit { get; }

	    public float RayDistance { get; }
		public Ray Ray { get; }
	
	
		public PlayerShootEvent(Player player, float distance, IDestructible component, float damage, Firearm firearm, RaycastHit hit, Ray ray, float rayDistance) : base(player)
		{

			Distance = distance;
			Component = component;
			Damage = damage;
			Firearm = firearm;
			Hit = hit;
			RayDistance = rayDistance;
			Ray = ray;
		}

        public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerShoot)handler).OnShoot(this);
		}
	}
}
