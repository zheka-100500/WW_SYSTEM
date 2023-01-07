using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerSpawnRagdollEvent : PlayerEvent
	{
		
		public RoleTypeId Role { get; set; }


		public Vector Position { get; set; }


		public Vector Rotation { get; set; }

	
		public Player Attacker { get; }


		public DamageHandlerBase DamageHandler { get; }

	
		public bool Allow { get; set; }


		public PlayerSpawnRagdollEvent(Player player, RoleTypeId role, Vector position, Vector rotation, Player attacker, DamageHandlerBase damagehandler, bool allow) : base(player)
		{
			this.Role = role;
			this.Position = position;
			this.Rotation = rotation;
			this.Attacker = attacker;
			this.DamageHandler = damagehandler;
			this.Allow = allow;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSpawnRagdoll)handler).OnSpawnRagdoll(this);
		}
	}
}
