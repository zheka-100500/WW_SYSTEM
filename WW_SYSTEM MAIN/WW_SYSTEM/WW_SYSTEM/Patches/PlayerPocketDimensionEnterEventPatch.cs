using CustomPlayerEffects;
using HarmonyLib;
using InventorySystem.Items.MicroHID;
using MapGeneration;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp106;
using PlayerStatsSystem;
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
	[HarmonyPatch(typeof(Scp106Attack), nameof(Scp106Attack.ServerShoot))]
	public class PlayerPocketDimensionEnterEventPatch
	{

		public static bool Prefix(Scp106Attack __instance)
		{

			try
			{










                using (new FpcBacktracker(__instance._targetHub, __instance._targetPosition, 0.35f))
                {
                    Vector3 vector = __instance._targetPosition - __instance._ownerPosition;
                    float sqrMagnitude = vector.sqrMagnitude;
                    if (sqrMagnitude > __instance._maxRangeSqr)
                    {
                        return false;
                    }
                    Vector3 forward = __instance.OwnerCam.forward;
                    forward.y = 0f;
                    vector.y = 0f;
                    if (Physics.Linecast(__instance._ownerPosition, __instance._targetPosition, MicroHIDItem.WallMask))
                    {
                        return false;
                    }
                    if (__instance._dotOverDistance.Evaluate(sqrMagnitude) > Vector3.Dot(vector.normalized, forward.normalized))
                    {
                        __instance.SendCooldown(__instance._missCooldown);
                        return false;
                    }

					Player target = Round.GetPlayer(__instance._targetHub);

					Vector3 lastPos = target.GetPosition();
                    PlayerPocketDimensionEnterEvent ev = new PlayerPocketDimensionEnterEvent(target, (float)__instance._damage, lastPos.ToVector(), Round.GetPlayer(__instance.Owner), true);
                    EventManager.Manager.HandleEvent<IEventHandlerPocketDimensionEnter>(ev);

                    if (!ev.Allow)
                    {
                        return false;
                    }

                    DamageHandlerBase handler = new ScpDamageHandler(__instance.Owner, ev.Damage, DeathTranslations.PocketDecay);
					
                    if (!__instance._targetHub.playerStats.DealDamage(handler))
                    {
                        return false;
                    }
                }

                __instance.SendCooldown(__instance._hitCooldown);
                __instance.Vigor.VigorAmount += 0.3f;
                __instance.ReduceSinkholeCooldown();
                Hitmarker.SendHitmarker(__instance.Owner, 1f);
        
                Scp106Attack.InvokeData(__instance._targetHub);
                PlayerEffectsController playerEffectsController = __instance._targetHub.playerEffectsController;
                playerEffectsController.EnableEffect<Traumatized>(180f, false);
                playerEffectsController.EnableEffect<Corroding>(0f, false);


                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerPocketDimensionEnterEvent error: {0}", arg));
			}
			return true;
		}
	}

}
