using HarmonyLib;
using PlayerRoles;
using RemoteAdmin;
using Respawning;
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
	[HarmonyPatch(typeof(Escape), nameof(Escape.ServerHandlePlayer))]
	public class PlayerCheckEscapeEventPatch
	{

		public static bool Prefix(ReferenceHub hub)
		{

			try
			{
                RoleTypeId roleTypeId = RoleTypeId.None;
                Escape.EscapeScenarioType escapeScenarioType = Escape.ServerGetScenario(hub);
                switch (escapeScenarioType)
                {
                    case Escape.EscapeScenarioType.ClassD:
                    case Escape.EscapeScenarioType.CuffedScientist:
                        roleTypeId = RoleTypeId.ChaosConscript;
                        RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, 4f);
                        break;
                    case Escape.EscapeScenarioType.CuffedClassD:
                        roleTypeId = RoleTypeId.NtfPrivate;
                        RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, 3f);
                        break;
                    case Escape.EscapeScenarioType.Scientist:
                        roleTypeId = RoleTypeId.NtfSpecialist;
                        RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, 3f);
                        break;
                }

                if (Round.TryGetPlayer(hub, out var pl))
				{
					PlayerCheckEscapeEvent playerCheckEscapeEvent = new PlayerCheckEscapeEvent(pl, roleTypeId, true);
					EventManager.Manager.HandleEvent<IEventHandlerCheckEscape>(playerCheckEscapeEvent);
					if (!playerCheckEscapeEvent.AllowEscape || playerCheckEscapeEvent.Role == RoleTypeId.None)
					{
						return false;
					}
                    hub.connectionToClient.Send<Escape.EscapeMessage>(new Escape.EscapeMessage
                    {
                        ScenarioId = (byte)escapeScenarioType,
                        EscapeTime = (ushort)Mathf.CeilToInt(hub.roleManager.CurrentRole.ActiveTime)
                    }, 0);

                    Escape.OnServerPlayerEscape?.Invoke(hub);
                    hub.roleManager.ServerSetRole(roleTypeId, RoleChangeReason.Escaped);
                }

				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerCheckEscapeEvent error: {0}", arg));
			}
			return true;
		}
	}

}
