using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(Inventory), "UserCode_CmdDropItem")]
	public class PlayerDropItemEventPatch
	{

		public static bool Prefix(ushort itemSerial, bool tryThrow, Inventory __instance)
		{

			try
			{
				


				ItemBase itemBase;
				if (!__instance.UserInventory.Items.TryGetValue(itemSerial, out itemBase) || !itemBase.CanHolster())
				{
					return false;
				}

				PlayerDropItemEvent ev = new PlayerDropItemEvent(__instance.GetComponent<Player>(), itemBase.ItemTypeId, true);
				EventManager.Manager.HandleEvent<IEventHandlerPlayerDropItem>(ev);
				if (!ev.allow)
				{
					return false;
				}

                ItemPickupBase itemPickupBase = __instance.ServerDropItem(itemSerial);
                __instance.SendItemsNextFrame = true;
                Rigidbody rigidbody;
                if (!tryThrow || itemPickupBase == null || !itemPickupBase.TryGetComponent<Rigidbody>(out rigidbody))
                {
                    return false;
                }
        
                Vector3 velocity = __instance._hub.GetVelocity();
                Vector3 vector = velocity / 3f + __instance._hub.PlayerCameraReference.forward * 6f * (Mathf.Clamp01(Mathf.InverseLerp(7f, 0.1f, rigidbody.mass)) + 0.3f);
                vector.x = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(vector.x)) * (float)((vector.x < 0f) ? -1 : 1);
                vector.y = Mathf.Max(Mathf.Abs(velocity.y), Mathf.Abs(vector.y)) * (float)((vector.y < 0f) ? -1 : 1);
                vector.z = Mathf.Max(Mathf.Abs(velocity.z), Mathf.Abs(vector.z)) * (float)((vector.z < 0f) ? -1 : 1);
                rigidbody.position = __instance._hub.PlayerCameraReference.position;
                rigidbody.velocity = vector;
                rigidbody.angularVelocity = Vector3.Lerp(itemBase.ThrowSettings.RandomTorqueA, itemBase.ThrowSettings.RandomTorqueB, UnityEngine.Random.value);
                float magnitude = rigidbody.angularVelocity.magnitude;
                if (magnitude > rigidbody.maxAngularVelocity)
                {
                    rigidbody.maxAngularVelocity = magnitude;
                }

                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerDropItemEvent error: {0}", arg));
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropAmmo))]
	public class PlayerDropItemEventPatch1
	{

		public static bool Prefix(byte ammoType, ushort amount, Inventory __instance)
		{

			try
			{



				

				PlayerDropItemEvent ev = new PlayerDropItemEvent(__instance.GetComponent<Player>(), (ItemType)ammoType, true);
				EventManager.Manager.HandleEvent<IEventHandlerPlayerDropItem>(ev);
				if (!ev.allow)
				{
					return false;
				}

			
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerDropItemEvent error: {0}", arg));
			}
			return true;
		}
	}

}
