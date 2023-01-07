using GameCore;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Pickups;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using MEC;
using UnityEngine;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using PlayerRoles;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(InventoryItemProvider), "RoleChanged")]
	public class PlayerSetRoleEventPatch
	{


		static IEnumerator<float> SetHPAndMaxHP(float HP, float MaxHP, Player player)
		{
			yield return Timing.WaitForOneFrame;
			if(player != null)
			{
				player.HP = HP;
				player.MaxHP = (int)MaxHP;
			}
		}
		static IEnumerator<float> DisableIgnorePos(Player player)
		{
			yield return Timing.WaitForOneFrame;
			if (player != null)
			{
				player.IgnoreSpawnPos = false;
			}
		}
		static IEnumerator<float> DisableCustomSpawnPos(Player player)
		{
			yield return Timing.WaitForOneFrame;
			if (player != null)
			{
				player.UseCustomSpawnPos = false;
			}
		}

		public static bool Prefix(ReferenceHub ply, PlayerRoleBase prevRole, PlayerRoleBase newRole)
			{

				try
				{
				if (!NetworkServer.active)
				{
					return false;
				}
				Inventory inventory = ply.inventory;
				new HashSet<ushort>(inventory.UserInventory.Items.Keys);
			
				if (newRole.ServerSpawnReason == RoleChangeReason.Escaped)
				{
					List<ItemPickupBase> list = new List<ItemPickupBase>();
					BodyArmor bodyArmor;
					if (inventory.TryGetBodyArmor(out bodyArmor))
					{
						bodyArmor.DontRemoveExcessOnDrop = true;
					}
					while (inventory.UserInventory.Items.Count > 0)
					{
						list.Add(inventory.ServerDropItem(inventory.UserInventory.Items.ElementAt(0).Key));
					}
					InventoryItemProvider.PreviousInventoryPickups[ply] = list;
				}
				else
				{
					while (inventory.UserInventory.Items.Count > 0)
					{
						inventory.ServerRemoveItem(inventory.UserInventory.Items.ElementAt(0).Key, null);
					}
					inventory.UserInventory.ReserveAmmo.Clear();
					inventory.SendAmmoNextFrame = true;
				}
		
				InventoryRoleInfo inventoryRoleInfo;
				var Ammos = new Dictionary<AmmoType, int>();
				var Items = new List<ItemType>();
				var WeaponAmmos = new Dictionary<ItemType, byte>();
				if (StartingInventories.DefinedInventories.TryGetValue(newRole.RoleTypeId, out inventoryRoleInfo))
				{
					foreach (KeyValuePair<ItemType, ushort> keyValuePair in inventoryRoleInfo.Ammo)
					{
						Ammos.Add(keyValuePair.Key.GetAmmo(), keyValuePair.Value);
					}
					for (int i = 0; i < inventoryRoleInfo.Items.Length; i++)
					{
						Items.Add(inventoryRoleInfo.Items[i]);


					}
		

				}
			
				if (Round.TryGetPlayer(ply, out var pl))
				{
					float HP = Round.GetMaxHp(newRole.RoleTypeId);
					if (HP <= 1)
					{
						IHealthbarRole healthbarRole;
						if ((healthbarRole = (newRole as IHealthbarRole)) != null)
						{
							HP = healthbarRole.MaxHealth;
						}
					}
					
					PlayerSetRoleEvent ev = new PlayerSetRoleEvent(pl, Ammos, newRole.RoleTypeId, newRole.ServerSpawnReason, newRole.RoleTypeId.GetTeam(true), Items, HP, HP, false, false, Vector3.zero);
					EventManager.Manager.HandleEvent<IEventHandlerSetRole>(ev);
					Items = ev.Items;
					Ammos = ev.Ammos;
					WeaponAmmos = ev.WeaponsAmmos;
					if (ev.Role != newRole.RoleTypeId)
					{



						if (ev.UseCustomSpawn)
						{
							pl.UseCustomSpawnPos = true;
							pl.CustomSpawnPosition = ev.SpawnPos;
						}

						pl.Role = ev.Role;
						if (ev.UseCustomSpawn) Timing.RunCoroutine(DisableCustomSpawnPos(pl));
						
					}
					if (ev.CustomHP)
					{
						Timing.RunCoroutine(SetHPAndMaxHP(ev.HP, ev.MaxHp, pl));
					}
					else
					{
						HP = Round.GetMaxHp(newRole.RoleTypeId);
						Timing.RunCoroutine(SetHPAndMaxHP(HP, HP, pl));
					}

			
				}
			
			

				foreach (var item in Ammos)
				{

					switch (item.Key)
					{
						case AmmoType.Ammo12gauge:
							inventory.ServerSetAmmo(ItemType.Ammo12gauge, item.Value);
							break;
						case AmmoType.Ammo556x45:
							inventory.ServerSetAmmo(ItemType.Ammo556x45, item.Value);
							break;
						case AmmoType.Ammo44cal:
							inventory.ServerSetAmmo(ItemType.Ammo44cal, item.Value);
							break;
						case AmmoType.Ammo9x19:
							inventory.ServerSetAmmo(ItemType.Ammo9x19, item.Value);
							break;
						case AmmoType.Ammo762x39:
							inventory.ServerSetAmmo(ItemType.Ammo762x39, item.Value);
							break;
						default:
							break;
					}



				}
			
				foreach (var item in Items)
				{
					ItemBase arg = inventory.ServerAddItem(item, 0, null);
					Firearm firearm;
					if ((firearm = arg as Firearm) != null)
					{


						Dictionary<ItemType, uint> dictionary;
						uint code;
						if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(ply, out dictionary) && dictionary.TryGetValue(item, out code))
						{
							firearm.ApplyAttachmentsCode(code, true);
						}
						FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;
						//if (firearm.CombinedAttachments.AdditionalPros.HasFlagFast(AttachmentDescriptiveAdvantages.Flashlight))
						//{
						//	firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;
						//}

						var ammo = WeaponAmmos.Count > 0 && WeaponAmmos.ContainsKey(item) ? WeaponAmmos[item] : firearm.AmmoManagerModule.MaxAmmo;
						
						firearm.Status = new FirearmStatus(ammo, firearmStatusFlags, firearm.GetCurrentAttachmentsCode());
					}
					Action<ReferenceHub, ItemBase> onItemProvided = InventoryItemProvider.OnItemProvided;
					if (onItemProvided != null)
					{
						onItemProvided(ply, arg);
					}
				}



				return false;
				}
				catch (Exception arg)
				{


					Logger.Error("[EVENT MANAGER]", string.Format("PlayerSetRoleEvent error: {0}", arg));
				}
				return true;
			}
		
	}
}
