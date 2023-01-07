using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Events;
using PlayerStatsSystem;

namespace WW_SYSTEM.Events
{
	public class PlayerDeathEvent : PlayerEvent
	{
	
		public PlayerDeathEvent(Player player, Player killer, bool spawnRagdoll, StandardDamageHandler damagehandler) : base(player)
		{
			this.Killer = killer;
			this.SpawnRagdoll = spawnRagdoll;
			this.Damagehandler = damagehandler;
			if (CustomEventManager.EventInProgress)
			{
				CustomEventManager.CurrentEvent.OnPlayerDie(player, killer, damagehandler);
			}
		}


		public Player Killer { get; }


		public bool SpawnRagdoll { get; set; }

	
		public StandardDamageHandler Damagehandler { get;}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerDie)handler).OnPlayerDie(this);
		}
	}
}
