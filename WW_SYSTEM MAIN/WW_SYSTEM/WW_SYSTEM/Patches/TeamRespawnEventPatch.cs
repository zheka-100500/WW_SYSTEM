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
using GameCore;
using Respawning;
using Respawning.NamingRules;
using NorthwoodLib.Pools;
using PlayerRoles;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(RespawnManager), "Spawn")]
	public class TeamRespawnEventPatch
	{

		public static bool Prefix(RespawnManager __instance)
		{

			try
			{
                SpawnableTeamHandlerBase spawnableTeamHandlerBase;
                if (!RespawnManager.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out spawnableTeamHandlerBase) || __instance.NextKnownTeam == SpawnableTeamType.None)
                {
                    ServerConsole.AddLog("Fatal error. Team '" + __instance.NextKnownTeam + "' is undefined.", ConsoleColor.Red);
                    return false;
                }
      
                List<ReferenceHub> list = ReferenceHub.AllHubs.Where(new Func<ReferenceHub, bool>(__instance.CheckSpawnable)).ToList<ReferenceHub>();
                if (__instance._prioritySpawn)
                {
                    list = (from item in list
                            orderby item.roleManager.CurrentRole.ActiveTime descending
                            select item).ToList<ReferenceHub>();
                }
                else
                {
                    list.ShuffleList<ReferenceHub>();
                }
                int maxWaveSize = spawnableTeamHandlerBase.MaxWaveSize;
                int num = list.Count;
                if (num > maxWaveSize)
                {
                    list.RemoveRange(maxWaveSize, num - maxWaveSize);
                    num = maxWaveSize;
                }
                UnitNamingRule rule;
                if (num > 0 && UnitNamingRule.TryGetNamingRule(__instance.NextKnownTeam, out rule))
                {
                    UnitNameMessageHandler.SendNew(__instance.NextKnownTeam, rule);
                }
                list.ShuffleList<ReferenceHub>();
                List<ReferenceHub> list2 = ListPool<ReferenceHub>.Shared.Rent();
                Queue<RoleTypeId> queue = new Queue<RoleTypeId>();

                List<Player> players = new List<Player>();
				foreach (var item in list)
				{
					if(Round.TryGetPlayer(item, out var pl))
                    {
						players.Add(pl);
					}
				

				}

				bool IsChaos = false;

				switch (__instance.NextKnownTeam)
				{
					case SpawnableTeamType.ChaosInsurgency:
						IsChaos = true;

						break;
					case SpawnableTeamType.NineTailedFox:
						IsChaos = false;
						break;
				}

				TeamRespawnEvent ev = new TeamRespawnEvent(players, IsChaos, true);
				EventManager.Manager.HandleEvent<IEventHandlerTeamRespawn>(ev);
				list.Clear();
				if (!ev.Allow)
				{
				
					__instance.NextKnownTeam = SpawnableTeamType.None;
					return false;
				}
			
				foreach (var item in ev.PlayersToRespawn)
				{
					list.Add(item.Hub);

				}
                if (ev.UseCustomQueue)
                {
					queue = ev.CustomQueue;

				}
                else
                {
					queue = new Queue<RoleTypeId>();
					spawnableTeamHandlerBase.GenerateQueue(queue, list.Count);
				}

                foreach (ReferenceHub referenceHub in list)
                {
                    try
                    {
                        RoleTypeId newRole = queue.Dequeue();
                        referenceHub.roleManager.ServerSetRole(newRole, RoleChangeReason.Respawn);
                        list2.Add(referenceHub);
                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Concat(new string[]
                        {
                        "Player ",
                        referenceHub.LoggedNameFromRefHub(),
                        " respawned as ",
                        newRole.ToString(),
                        "."
                        }), ServerLogs.ServerLogType.GameEvent, false);
                    }
                    catch (Exception ex)
                    {
                        if (referenceHub != null)
                        {
                            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + referenceHub.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent, false);
                        }
                        else
                        {
                            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent, false);
                        }
                    }
                }
                if (list2.Count > 0)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Concat(new object[]
                    {
                    "RespawnManager has successfully spawned ",
                    list2.Count,
                    " players as ",
                    __instance.NextKnownTeam.ToString(),
                    "!"
                    }), ServerLogs.ServerLogType.GameEvent, false);
                    RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                }

                RespawnManager.ServerOnRespawned?.Invoke(__instance.NextKnownTeam, list2);
                ListPool<ReferenceHub>.Shared.Return(list2);
                __instance.NextKnownTeam = SpawnableTeamType.None;
                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("TeamRespawnEvent error: {0}", arg));
			}
			return true;
		}
	}

}
