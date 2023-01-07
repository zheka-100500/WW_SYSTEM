using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
	public struct HitInfo
	{
		
		public HitInfo(float amnt, Player attacker, StandardDamageHandler damageHandler, string AttackerNick, bool CustomName)
		{
			this.Amount = amnt;
			this.DamageHandler = damageHandler;
			this.Attacker = attacker;
			this.Time = ServerTime.time;
			this.AttackerNick = AttackerNick;
			this.CustomName = CustomName;
		}

	
		public float Amount;

	
		public readonly StandardDamageHandler DamageHandler;

	
		public readonly int Time;

		
		public readonly Player Attacker;

		public readonly string AttackerNick;

		public bool CustomName { get; }

	}
}
