using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
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
	[HarmonyPatch(typeof(DoorVariant), "ServerInteract")]
	public class PlayerDoorAccessEventPatch
	{

		public static bool Prefix(ReferenceHub ply, byte colliderId, DoorVariant __instance)
		{

			try
			{
				if (!NetworkServer.active)
				{
					Debug.LogWarning("[Server] function 'System.Void Interactables.Interobjects.DoorUtils.DoorVariant::ServerInteract(ReferenceHub,System.Byte)' called on client");
					return false;
				}

				bool Allow = ply.GetRoleId() == RoleTypeId.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.CurInstance, ply);

                if (__instance.ActiveLocks > 0 && !ply.serverRoles.BypassMode)
                {
                    DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)__instance.ActiveLocks);
                    if ((!mode.HasFlagFast(DoorLockMode.CanClose) || !mode.HasFlagFast(DoorLockMode.CanOpen)) && (!mode.HasFlagFast(DoorLockMode.ScpOverride) || !ply.IsSCP(true)) && (mode == DoorLockMode.FullLock || (__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanClose)) || (!__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                    {
                        __instance.LockBypassDenied(ply, colliderId);
                        return false;
                    }
                }
                if (__instance.AllowInteracting(ply, colliderId))
				{
					PlayerDoorAccessEvent playerDoorAccessEvent = new PlayerDoorAccessEvent(ply.characterClassManager.GetComponent<Player>(), new ROOM_DOOR(__instance))
					{
						Allow = Allow,
						Destroy = false,
						ForceDeny = false
					};
					EventManager.Manager.HandleEvent<IEventHandlerDoorAccess>(playerDoorAccessEvent);


					if (playerDoorAccessEvent.ForceDeny) return false;

                    if (playerDoorAccessEvent.Allow)
                    {
						__instance.NetworkTargetState = !__instance.TargetState;
						__instance._triggerPlayer = ply;
						
					}
					if (playerDoorAccessEvent.Destroy || playerDoorAccessEvent.Player.BreakDoors)
					{
						IDamageableDoor damageableDoor;
						if ((damageableDoor = (__instance as IDamageableDoor)) != null)
						{
							if(!damageableDoor.IsDestroyed && __instance.GetExactState() < 1f)
                            {
								damageableDoor.ServerDamage(float.MaxValue, DoorDamageType.ServerCommand);
							}
						}
					}



					if (!playerDoorAccessEvent.Allow && !playerDoorAccessEvent.Destroy)
					{
						__instance.PermissionsDenied(ply, colliderId);
						DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
					}

				}
			

				
				return false;
			}
			catch (Exception arg)
			{

				Logger.Error("[EVENT MANAGER]", string.Format("PlayerDoorAccessEvent error: {0}", arg));
			}
			return true;
		}
	}

}
