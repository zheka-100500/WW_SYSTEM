using InventorySystem.Items.Pickups;
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
	public class SCP914UpgradeEvent : Event
	{

		public List<Player> Users { get; set; }

		public KnobSetting KnobSetting { get; set; }


		public List<ItemPickupBase> Items { get; set; }


		public bool UpgradeItems { get; set; }

		public bool UpgradePlayers { get; set; }

		public Vector3 moveVector { get; }



		public Vector3 GetMovePos(ItemPickupBase itemPickup) => itemPickup.transform.position + moveVector;
		public Vector3 GetMovePos(Player player) => player.transform.position + moveVector;

		public SCP914UpgradeEvent(Vector3 moveVector, bool UpgradeItems, bool UpgradePlayers, List<Player> users, KnobSetting knobSetting, List<ItemPickupBase> ItemsToUpgrade)
		{
		
			this.Users = users;
			this.KnobSetting = knobSetting;
			this.Items = ItemsToUpgrade;
			this.moveVector = moveVector;
			this.UpgradeItems = UpgradeItems;
			this.UpgradePlayers = UpgradePlayers;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSCP914Upgrade)handler).OnSCP914Upgrade(this);
		}
	}
}
