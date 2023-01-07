using CustomPlayerEffects;
using Footprinting;
using HarmonyLib;
using Hints;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.AntiCheat;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items;
using WW_SYSTEM.Custom_Items.Items;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerPerformShot))]
	public class PlayerShootEventPatch
	{
	

		

		public static bool Prefix(Ray ray, SingleBulletHitreg __instance)
			{

			try
			{
                ray = __instance.ServerRandomizeRay(ray);
                FirearmBaseStats baseStats = __instance.Firearm.BaseStats;

                if (Round.UseItemPhysics)
				{
					RaycastHit hit1;
					if (Physics.Raycast(ray, out hit1, baseStats.MaxDistance(), Round.DetectionMask))
					{
						ItemPickupBase pickup;
						if ((pickup = (hit1.collider.gameObject.GetComponentInParent<ItemPickupBase>())) != null)
						{
							var type = pickup.NetworkInfo.ItemId;

							if (type != ItemType.GrenadeFlash && type != ItemType.GrenadeHE)
							{
								var pos = __instance.Hub.transform.position;
								if(Round.TryGetPlayer(__instance.Hub, out var pl))
								{
									pos = pl.GetPosition();
								}
								pickup.Rb.AddExplosionForce(Vector3.Distance(pickup.transform.position, pos), pos, 500f, 3f, ForceMode.Impulse);
							}
							else
							{
								if ((pickup as ThrownProjectile) == null)
								{

									ItemBase ItemB = InventoryItemLoader.AvailableItems[type];
									ThrownProjectile grenade = null;




									ThrowableItem throwableItem;
									if ((throwableItem = (ItemB as ThrowableItem)) != null && !(throwableItem.Projectile == null))
									{
										grenade = throwableItem.Projectile;

									}
									ThrownProjectile thrownProjectile = GameObject.Instantiate<ThrownProjectile>(grenade, pickup.transform.position, pickup.transform.rotation);
									pickup.DestroySelf();
									PickupSyncInfo pickupSyncInfo = new PickupSyncInfo
									{
										ItemId = ItemType.GrenadeHE,
										Locked = true,
										Serial = ItemSerialGenerator.GenerateNext(),
										Weight = 10,
                                        RelativePosition = new RelativePositioning.RelativePosition(thrownProjectile.transform.position),
										RelativeRotation = new LowPrecisionQuaternion(thrownProjectile.transform.rotation)
									};
									thrownProjectile.NetworkInfo = pickupSyncInfo;
									thrownProjectile.PreviousOwner = new Footprint(__instance.Hub);
									NetworkServer.Spawn(thrownProjectile.gameObject);
									thrownProjectile.NetworkInfo = pickupSyncInfo;
									thrownProjectile.ServerActivate();

									return false;
								}
							}

						}
					

					}
				}

				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, baseStats.MaxDistance(), StandardHitregBase.HitregMask))
				{
				
					IDestructible destructible;
					float damage = baseStats.DamageAtDistance(__instance.Firearm, hit.distance);
					if (hit.collider.TryGetComponent<IDestructible>(out destructible))
					{
						PlayerShootEvent ev = new PlayerShootEvent(Round.GetPlayer(__instance.Hub), hit.distance, destructible, damage, __instance.Firearm, hit, ray, hit.distance);
						EventManager.Manager.HandleEvent<IEventHandlerShoot>(ev);

						if (CustomItemManager.TryGetItem(__instance.Firearm.ItemSerial, CustomItemType.WEAPON, out var item))
						{
							CustomWeapon Weapon;
							if ((Weapon = item as CustomWeapon) != null)
							{
								if (!Weapon.Shoot(ev.Player, ev.Distance, ev.Component, ev.Damage, ev.Firearm, ev.Hit)) return false;
							}
						}

						if (ev.Damage > 0)
						if (destructible.Damage(ev.Damage, new FirearmDamageHandler(__instance.Firearm, damage, true), hit.point))
						{
                                ReferenceHub referenceHub;
                                if (!ReferenceHub.TryGetHubNetID(destructible.NetworkId, out referenceHub) || !referenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
                                {
                                    Hitmarker.SendHitmarker(__instance.Conn, 1f);
                                }
                                __instance.ShowHitIndicator(destructible.NetworkId, damage, ray.origin);
                                __instance.PlaceBloodDecal(ray, hit, destructible);
                                return false;
						}

					}
					else
					{
						PlayerShootEvent ev = new PlayerShootEvent(Round.GetPlayer(__instance.Hub), hit.distance, destructible, damage, __instance.Firearm, hit, ray, hit.distance);
						EventManager.Manager.HandleEvent<IEventHandlerShoot>(ev);



						if (CustomItemManager.TryGetItem(__instance.Firearm.ItemSerial, CustomItemType.WEAPON, out var item))
						{
							CustomWeapon Weapon;
							if ((Weapon = item as CustomWeapon) != null)
							{
								if (!Weapon.Shoot(ev.Player, ev.Distance, ev.Component, ev.Damage, ev.Firearm, ev.Hit)) return false;
							}
						}

						__instance.PlaceBulletholeDecal(ray, hit);
					}
				}
	
				
				return false;

			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerShootEventPatch error: {0}", arg));
			}
				return true;
			}
	
	}

	[HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ShootPellet))]
	public class PlayerShootEventPatch1
	{


		public static bool Prefix(Vector2 pelletSettings, Ray originalRay, Vector2 offsetVector, BuckshotHitreg __instance)
		{

			try
			{

                Vector2 vector = Vector2.Lerp(pelletSettings, __instance.GenerateRandomPelletDirection, __instance.BuckshotRandomness) * __instance.BuckshotScale;
                Vector3 vector2 = originalRay.direction;
                vector2 = Quaternion.AngleAxis(vector.x + offsetVector.x, __instance.Hub.PlayerCameraReference.up) * vector2;
                vector2 = Quaternion.AngleAxis(vector.y + offsetVector.y, __instance.Hub.PlayerCameraReference.right) * vector2;
                Ray ray = new Ray(originalRay.origin, vector2);
                RaycastHit hit;
                var distance = __instance.Firearm.BaseStats.MaxDistance();


				if (Round.UseItemPhysics)
				{
					RaycastHit hit1;
					if (Physics.Raycast(ray, out hit1, distance, Round.DetectionMask))
					{
						ItemPickupBase pickup;
						if ((pickup = (hit1.collider.gameObject.GetComponentInParent<ItemPickupBase>())) != null)
						{

							var type = pickup.NetworkInfo.ItemId;

							if (type != ItemType.GrenadeFlash && type != ItemType.GrenadeHE)
							{
								var pos = Round.GetPlayer(__instance.Hub).GetPosition();
								pickup.Rb.AddExplosionForce(Vector3.Distance(pickup.transform.position, pos), pos, 500f, 3f, ForceMode.Impulse);
							}
							else
							{
								
								
								
								if ((pickup as ThrownProjectile) == null && !pickup.Info.Locked)
								{
									var info = pickup.NetworkInfo;
									info.Locked = true;
									pickup.NetworkInfo = info;

									ItemBase ItemB = InventoryItemLoader.AvailableItems[type];
									ThrownProjectile grenade = null;




									ThrowableItem throwableItem;
									if ((throwableItem = (ItemB as ThrowableItem)) != null && !(throwableItem.Projectile == null))
									{
										grenade = throwableItem.Projectile;

									}
									ThrownProjectile thrownProjectile = GameObject.Instantiate<ThrownProjectile>(grenade, pickup.transform.position, pickup.transform.rotation);
									pickup.DestroySelf();
									PickupSyncInfo pickupSyncInfo = new PickupSyncInfo
									{
										ItemId = ItemType.GrenadeHE,
										Locked = true,
										Serial = ItemSerialGenerator.GenerateNext(),
										Weight = 10,
										RelativePosition = new RelativePositioning.RelativePosition(thrownProjectile.transform.position),
										RelativeRotation = new LowPrecisionQuaternion(thrownProjectile.transform.rotation)
									};
									thrownProjectile.NetworkInfo = pickupSyncInfo;
									thrownProjectile.PreviousOwner = new Footprint(__instance.Hub);
									NetworkServer.Spawn(thrownProjectile.gameObject);
									thrownProjectile.NetworkInfo = pickupSyncInfo;
									
									thrownProjectile.ServerActivate();
									
									return false;
								}
							}


						}


					}
				}

				if (Physics.Raycast(ray, out hit, __instance.Firearm.BaseStats.MaxDistance(), StandardHitregBase.HitregMask))
				{
				

					IDestructible destructible;
                    float damage = __instance.Firearm.BaseStats.DamageAtDistance(__instance.Firearm, hit.distance) / (float)__instance._buckshotSettings.MaxHits;
                    if (hit.collider.TryGetComponent<IDestructible>(out destructible))
					{
						
						PlayerShootEvent ev = new PlayerShootEvent(Round.GetPlayer(__instance.Hub), hit.distance, destructible, damage, __instance.Firearm, hit, ray, distance);
						EventManager.Manager.HandleEvent<IEventHandlerShoot>(ev);

						if (CustomItemManager.TryGetItem(__instance.Firearm.ItemSerial, CustomItemType.WEAPON, out var item))
						{
							CustomWeapon Weapon;
							if ((Weapon = item as CustomWeapon) != null)
							{
								if (!Weapon.Shoot(ev.Player, ev.Distance, ev.Component, ev.Damage, ev.Firearm, ev.Hit)) return false;
							}
						}

						if (ev.Damage > 0)
							if (__instance.CanShoot(destructible))
						{
                                BuckshotHitreg.Hits[destructible].Add(new BuckshotHitreg.ShotgunHit(damage, ray, hit));

                                return false;
						}
					}
					else
					{
						PlayerShootEvent ev = new PlayerShootEvent(Round.GetPlayer(__instance.Hub), hit.distance, destructible, damage, __instance.Firearm, hit, ray, distance);
						EventManager.Manager.HandleEvent<IEventHandlerShoot>(ev);
						if (CustomItemManager.TryGetItem(__instance.Firearm.ItemSerial, CustomItemType.WEAPON, out var item))
						{
							CustomWeapon Weapon;
							if ((Weapon = item as CustomWeapon) != null)
							{
								if (!Weapon.Shoot(ev.Player, ev.Distance, ev.Component, ev.Damage, ev.Firearm, ev.Hit)) return false;
							}
						}

						__instance.PlaceBulletholeDecal(ray, hit);
					}
				}
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerShootEventPatch error: {0}", arg));
			}
			return true;
		}

	}
}
