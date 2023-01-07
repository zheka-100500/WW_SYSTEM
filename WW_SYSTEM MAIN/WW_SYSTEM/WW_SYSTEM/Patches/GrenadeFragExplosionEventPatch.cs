using Footprinting;
using HarmonyLib;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items;
using WW_SYSTEM.Custom_Items.Items;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.Explode))]
	public class GrenadeFragExplosionEventPatch
	{
		public static bool Prefix(Footprint attacker, Vector3 position, ExplosionGrenade settingsReference)
		{
			try
			{

	 
			HashSet<uint> hashSet = HashSetPool<uint>.Shared.Rent();
			HashSet<uint> hashSet2 = HashSetPool<uint>.Shared.Rent();
				float maxRadius = settingsReference._maxRadius;
				var Colliders = Physics.OverlapSphere(position, maxRadius, settingsReference._detectionMask);
			GrenadeFragExplosionEvent ev = new GrenadeFragExplosionEvent(Round.GetPlayer(attacker.Hub), Colliders.ToList(), settingsReference, true);
			EventManager.Manager.HandleEvent<IEventHandlerFragExplosion>(ev);
			if (!ev.Allow)
			{
				return false;
			}
			Colliders = ev.Colliders.ToArray();

			if (CustomItemManager.TryGetItem(settingsReference.NetworkInfo.Serial, CustomItemType.GRENADE, out var item))
			{
				CustomGrenade Grenade;
				if ((Grenade = item as CustomGrenade) != null)
				{
					if (!Grenade.ExplodeFrag(ev.Player, ev.Colliders, settingsReference, settingsReference._detectionMask)) return false;
				}
			}


				foreach (Collider collider in Colliders)
			{
				if (NetworkServer.active)
				{
					IExplosionTrigger explosionTrigger;
					if (collider.TryGetComponent<IExplosionTrigger>(out explosionTrigger))
					{
						explosionTrigger.OnExplosionDetected(attacker, position, maxRadius);
					}
					IDestructible destructible;
					InteractableCollider interactableCollider;
					DoorVariant doorVariant;
					if (collider.TryGetComponent<IDestructible>(out destructible))
					{
						if (!hashSet.Contains(destructible.NetworkId) && ExplosionGrenade.ExplodeDestructible(destructible, attacker, position, settingsReference))
						{
							hashSet.Add(destructible.NetworkId);
						}
					}
					else if (collider.TryGetComponent<InteractableCollider>(out interactableCollider) && (doorVariant = (interactableCollider.Target as DoorVariant)) != null && hashSet2.Add(doorVariant.netId))
					{
							ExplosionGrenade.ExplodeDoor(doorVariant, position, settingsReference);
					}
				}
				if (collider.attachedRigidbody != null)
				{
						ExplosionGrenade.ExplodeRigidbody(collider.attachedRigidbody, position, maxRadius, settingsReference);
				}
			}
			HashSetPool<uint>.Shared.Return(hashSet);
			HashSetPool<uint>.Shared.Return(hashSet2);

                Action<Footprint, Vector3, ExplosionGrenade> onExploded = ExplosionGrenade.OnExploded;
                if (onExploded == null)
                {
                    return false;
                }
                onExploded(attacker, position, settingsReference);
                return false;
			}
			catch (Exception)
			{

				return true;
			}
		}
	}

	[HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
	public class GrenadeFlashExplosionEventPatch
	{
		public static bool Prefix(FlashbangGrenade __instance)
		{
			try
			{
				if (!NetworkServer.active)
				{
					return false;
				}
				float time = __instance._blindingOverDistance.keys[__instance._blindingOverDistance.length - 1].time;
				float num = time * time;
				var Players = new List<Player>();
                foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
                {
                    if ((__instance.transform.position - referenceHub.transform.position).sqrMagnitude <= num && !(referenceHub == __instance.PreviousOwner.Hub) && HitboxIdentity.CheckFriendlyFire(__instance.PreviousOwner.Role, referenceHub.GetRoleId(), false))
                    {
                        if (Round.TryGetPlayer(referenceHub, out var pl))
                        {
                            Players.Add(pl);
                        }
                    }
                }


				if (CustomItemManager.TryGetItem(__instance.NetworkInfo.Serial, CustomItemType.GRENADE, out var item))
				{
					CustomGrenade Grenade;
					if ((Grenade = item as CustomGrenade) != null)
					{
						if (!Grenade.ExplodeFlash(Round.GetPlayer(__instance.PreviousOwner.Hub), Players, __instance, __instance._blindingMask)) return false;
					}
				}

				foreach (var Player in Players)
				{
					__instance.ProcessPlayer(Player.Hub);
				}
				return false;
			}
			catch (Exception)
			{

				return true;
			}
			



		}
	}
}
