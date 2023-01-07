using PlayerRoles;
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
    public class PlayerSetRoleEvent : PlayerEvent
    {
		public PlayerSetRoleEvent(Player player, Dictionary<AmmoType, int> ammos, RoleTypeId role, RoleChangeReason spawnReason, TEAMTYPE team, List<ItemType> items, float HP, float MAXHP, bool CustomHP, bool UseCustomSpawnPos, Vector3 SpawnPos) : base(player)
		{
			this.Role = role;
			this.Ammos = ammos;
			this.Items = items;
			Team = team;
			SpawnReason = spawnReason;
			this.HP = HP;
			this.MaxHp = MAXHP;
			this.CustomHP = CustomHP;
			UseCustomSpawn = UseCustomSpawnPos;
			this.SpawnPos = SpawnPos;
			WeaponsAmmos = new Dictionary<ItemType, byte>();
		}


		public Dictionary<AmmoType, int> Ammos { get; set; }

		public List<ItemType> Items { get; set; }

		public Dictionary<ItemType, byte> WeaponsAmmos { get; set; }

		public float HP { get; set; }

		public float MaxHp { get; set; }

		public bool CustomHP { get; set; }

		public RoleTypeId Role { get; set; }

		public bool UseCustomSpawn { get; set; }
		public Vector3 SpawnPos { get; set; }

		public TEAMTYPE Team { get; }
		public RoleChangeReason SpawnReason { get; }


		public override void ExecuteHandler(IEventHandler handler)
		{

			((IEventHandlerSetRole)handler).OnSetRole(this);

		}
	}
}
