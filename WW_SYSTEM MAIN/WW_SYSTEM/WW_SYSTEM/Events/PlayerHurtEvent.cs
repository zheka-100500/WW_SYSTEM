using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using PlayerStatsSystem;

namespace WW_SYSTEM.Events
{
	public class PlayerHurtEvent : PlayerEvent
	{
	
		public PlayerHurtEvent(Player player, Player attacker, StandardDamageHandler damagehandler, bool instakill) : base(player)
		{
			Attacker = attacker;
			DamageHandler = damagehandler;
			InstaKill = instakill;
		}

		public bool InstaKill = false;

		public Player Attacker { get; }

		public float Damage
		{
			get
			{
				return DamageHandler.Damage;
			}
			set
			{

				DamageHandler.Damage = value;

				if(value == 0)
				{
					InstaKill = false;
				}
			}
		}

		public DamageType DamageType => DamageHandler.GetDamageType();

		public StandardDamageHandler DamageHandler { get; }

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerHurt)handler).OnPlayerHurt(this);
		}


	}
}
