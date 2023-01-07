using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Coin;
using Mirror;
using PlayerRoles;
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
	[HarmonyPatch(typeof(ElevatorManager), nameof(ElevatorManager.ServerReceiveMessage))]
	public class PlayerElevatorUseEventPatch
	{

		public static bool Prefix(NetworkConnection conn, ElevatorManager.ElevatorSyncMsg msg, ElevatorManager __instance)
		{
			try
			{
				ReferenceHub referenceHub;
                if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out referenceHub))
                {
                    return false;
                }
                if (!referenceHub.IsAlive())
                {
                    return false;
                }
                ElevatorManager.ElevatorGroup elevatorGroup;
                int lvl;
                msg.Unpack(out elevatorGroup, out lvl);
                ElevatorChamber elevatorChamber;
                if (!ElevatorManager.SpawnedChambers.TryGetValue(elevatorGroup, out elevatorChamber) || elevatorChamber == null)
                {
                    return false;
                }
                if (!elevatorChamber.IsReady)
                {
                    return false;
                }

                foreach (ElevatorPanel elevatorPanel in elevatorChamber.AllPanels)
				{
					if (elevatorPanel.AssignedChamber.AssignedGroup == elevatorGroup && (elevatorPanel.AssignedChamber.ActiveLocks == DoorLockReason.None || referenceHub.serverRoles.BypassMode) && elevatorPanel.VerificationRule.ServerCanInteract(referenceHub, elevatorPanel))
					{
 
                        PlayerElevatorUseEvent playerElevatorUseEvent = new PlayerElevatorUseEvent(__instance.gameObject.GetComponent<Player>(), elevatorChamber, elevatorChamber.CurrentDestination.transform.position.ToVector(), true, Round.ElevatorSpeed);
						EventManager.Manager.HandleEvent<IEventHandlerElevatorUse>(playerElevatorUseEvent);
						if (playerElevatorUseEvent.AllowUse)
						{
                            ElevatorManager.TrySetDestination(elevatorGroup, lvl, false);
							break;
                        }
					}
				}
				
				
				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerElevatorUseEvent error: {0}", arg));
			}
			return true;
		}
	}

}
