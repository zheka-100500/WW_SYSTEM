using Achievements;
using CustomPlayerEffects;
using GameCore;
using HarmonyLib;
using LightContainmentZoneDecontamination;
using MapGeneration;
using Mirror;
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
	[HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
	public class PlayerPocketDimensionDieEventPatch
	{

		public static bool Prefix(Collider other, PocketDimensionTeleport __instance)
		{

			try
			{
				if (!NetworkServer.active)
				{
					return false;
				}
				NetworkIdentity component = other.GetComponent<NetworkIdentity>();
				if (component == null)
				{
					return false;
				}
				Player player;
				if (!component.TryGetComponent<Player>(out player))
				{
					return false;
				}
                var referenceHub = player.Hub;


                if (referenceHub.roleManager.CurrentRole.ActiveTime < 1f)
                {
                    return false;
                }

				PlayerPocketDimensionDieEvent ev = new PlayerPocketDimensionDieEvent(player);
				EventManager.Manager.HandleEvent<IEventHandlerPocketDimensionDie>(ev);
				
                if (ev.ForceExit || (__instance._type != PocketDimensionTeleport.PDTeleportType.Killer && !AlphaWarheadController.Detonated) || PocketDimensionTeleport.DebugBool)
                {
                    if (__instance._type == PocketDimensionTeleport.PDTeleportType.Exit || PocketDimensionTeleport.DebugBool)
                    {
                        IFpcRole fpcRole;
                        if ((fpcRole = (referenceHub.roleManager.CurrentRole as IFpcRole)) == null)
                        {
                            return false;
                        }

						if(ev.CustomTeleportPosition != Vector3.zero)
						{
                            fpcRole.FpcModule.ServerOverridePosition(ev.CustomTeleportPosition, Vector3.zero);
                        }
                        else
						{
                            fpcRole.FpcModule.ServerOverridePosition(Scp106PocketExitFinder.GetBestExitPosition(fpcRole), Vector3.zero);
                        }
                        
                        referenceHub.playerEffectsController.EnableEffect<Disabled>(10f, true);
                        referenceHub.playerEffectsController.DisableEffect<Corroding>();
                        AchievementHandlerBase.ServerAchieve(component.connectionToClient, AchievementName.LarryFriend);
                        ImageGenerator.pocketDimensionGenerator.GenerateRandom();
                    }
                    return false;
                }
			
                referenceHub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.PocketDecay, null));

              
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerPocketDimensionExitEvent or PlayerPocketDimensionDieEvent error: {0}", arg));
			}
			return true;
		}
	}

}
