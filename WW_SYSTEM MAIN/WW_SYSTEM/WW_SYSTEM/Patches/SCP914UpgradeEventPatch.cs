using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using UnityEngine;
using Scp914;
using Mirror;
using NorthwoodLib.Pools;
using InventorySystem.Items.Pickups;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(Scp914Upgrader), "Upgrade")]
	public class SCP914UpgradeEventPatch
	{

		public static bool Prefix(Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
		{

			try
			{
				if (!NetworkServer.active)
				{
					throw new InvalidOperationException("Scp914Upgrader.Upgrade is a serverside-only script.");
				}
				HashSet<GameObject> hashSet = HashSetPool<GameObject>.Shared.Rent();
				bool upgradeDropped = (mode & Scp914Mode.Dropped) == Scp914Mode.Dropped;
				bool flag = (mode & Scp914Mode.Inventory) == Scp914Mode.Inventory;
				bool heldOnly = flag && (mode & Scp914Mode.Held) == Scp914Mode.Held;
				List<Player> Players = new List<Player>();
				List<ItemPickupBase> ItemPickups = new List<ItemPickupBase>();
				for (int i = 0; i < intake.Length; i++)
				{
					GameObject gameObject = intake[i].transform.root.gameObject;
					if (hashSet.Add(gameObject))
					{
						ReferenceHub ply;
						ItemPickupBase pickup;
						if (ReferenceHub.TryGetHub(gameObject, out ply))
						{
							Players.Add(Round.GetPlayer(ply.characterClassManager.UserId));
						
						}
						else if (gameObject.TryGetComponent<ItemPickupBase>(out pickup))
						{
						
							ItemPickups.Add(pickup);
						
						}

					}
				}

				SCP914UpgradeEvent ev = new SCP914UpgradeEvent(moveVector, true, true, Players, (KnobSetting)setting, ItemPickups);
				EventManager.Manager.HandleEvent<IEventHandlerSCP914Upgrade>(ev);
				Players = ev.Users;
				ItemPickups = ev.Items;

				hashSet.Clear();

                foreach (var item in Players)
                {
				
                    if (ev.UpgradePlayers)
					{
						if (item != null)
						{
							hashSet.Add(item.PlayerObj);
							Scp914Upgrader.ProcessPlayer(item.Hub, flag, heldOnly, moveVector, setting);
						}
					}
                }
				foreach (var item in ItemPickups)
				{
				
					if (ev.UpgradeItems)
					{
						if (item != null)
						{
							hashSet.Add(item.gameObject);
							Scp914Upgrader.ProcessPickup(item, upgradeDropped, moveVector, setting);
						}
					}

				}
				HashSetPool<GameObject>.Shared.Return(hashSet);
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("SCP914UpgradeEvent error: {0}", arg));
			}
			return true;
		}
	}

}
