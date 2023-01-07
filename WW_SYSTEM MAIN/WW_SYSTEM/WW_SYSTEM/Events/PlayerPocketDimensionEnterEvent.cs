using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerPocketDimensionEnterEvent : PlayerEvent
	{
		
		public float Damage { get; set; }

	
		public Vector LastPosition { get; }

		public Player Attacker { get; }

		public bool Allow { get; set; }


		public PlayerPocketDimensionEnterEvent(Player player, float damage, Vector lastPosition, Player attacker, bool Allow) : base(player)
		{
			this.Damage = damage;
			this.LastPosition = lastPosition;
			this.Attacker = attacker;
			this.Allow = Allow;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPocketDimensionEnter)handler).OnPocketDimensionEnter(this);
		}
	}
}
