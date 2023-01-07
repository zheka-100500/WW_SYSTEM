using CustomPlayerEffects;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.MicroHID;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using RemoteAdmin;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Events;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using Time = UnityEngine.Time;

namespace WW_SYSTEM.Patches
{


	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
	public class PlayerDeathEventPatch
	{

		public static bool Prefix(DamageHandlerBase handler, PlayerStats __instance)
		{

			try
			{
				Player Attacker;

				AttackerDamageHandler attackerhandler;
				if ((attackerhandler = (handler as AttackerDamageHandler)) != null && Round.TryGetPlayer(attackerhandler.Attacker.Hub, out var attackerpl))
				{
					Attacker = attackerpl;
				}
				else
				{
					Attacker = Server.LocalPlayer;

				}


				var ev = new PlayerDeathEvent(Round.GetPlayer(__instance._hub), Attacker, true, null);
				StandardDamageHandler standarthandler;
				if ((standarthandler = (handler as StandardDamageHandler)) != null)
				{
					if (standarthandler.GetDamageType() == DamageType.Tesla) standarthandler.Damage = Round.TeslaDamage;

					if (Attacker.InstantKill) standarthandler.Damage = float.MaxValue;

					ev = new PlayerDeathEvent(Round.GetPlayer(__instance._hub), Attacker, true, standarthandler);
					EventManager.Manager.HandleEvent<IEventHandlerPlayerDie>(ev);
				}
		

				if(ev.SpawnRagdoll)
                    RagdollManager.ServerSpawnRagdoll(__instance._hub, handler);

                __instance._hub.inventory.ServerDropEverything();
                __instance._hub.roleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
                __instance._hub.gameConsoleTransmission.SendToClient("You died. Reason: " + handler.ServerLogsText, "yellow");
                SpectatorRole spectatorRole;
                if ((spectatorRole = (__instance._hub.roleManager.CurrentRole as SpectatorRole)) != null)
                {
                    spectatorRole.ServerSetData(handler);
                }

                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerDeathEvent error: {0}", arg));
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
	public class PlayerHurtEventPatch
	{

		public static bool Prefix(ref DamageHandlerBase handler, ref bool __result, PlayerStats __instance)
		{

			try
			{
				Player Attacker;
				
			
				AttackerDamageHandler attackerhandler;
				if ((attackerhandler = (handler as AttackerDamageHandler)) != null && Round.TryGetPlayer(attackerhandler.Attacker.Hub, out var attackerpl))
				{
					Attacker = attackerpl;
				}
				else
				{
					Attacker = Server.LocalPlayer;

				}
				var pl = Round.GetPlayer(__instance._hub);
				if (pl != null && Attacker != null)
				{
					if(pl.GetTeamAdv == Attacker.GetTeamAdv && !ServerConsole.FriendlyFire)
					{
						__result = false;
						return false;
					}
				}

				
				StandardDamageHandler standarthandler;
				if ((standarthandler = (handler as StandardDamageHandler)) != null)
				{
					bool InstaKill = false;
					
					if (standarthandler.Damage <= -1 || Attacker.InstantKill) 
					{
						InstaKill = true;
						standarthandler.Damage = float.MaxValue;
					} 
					if (standarthandler.GetDamageType() == DamageType.Tesla)
					{
						standarthandler.Damage = Round.TeslaDamage;
					}

			


					var ev = new PlayerHurtEvent(pl, Attacker, standarthandler, InstaKill);

					EventManager.Manager.HandleEvent<IEventHandlerPlayerHurt>(ev);


					if (ev.InstaKill)
					{
						standarthandler.Damage = -1;
						return true;
					}

					if(ev.Damage <= 0f)
					{
						__result = false;
						return false;
					}
				}
				

				
			
				
			
				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerHurtEvent error: {0}", arg));
			}
			return true;
		}
	}

}
