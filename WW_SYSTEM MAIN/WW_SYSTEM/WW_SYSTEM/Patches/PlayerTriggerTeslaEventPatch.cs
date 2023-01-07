using HarmonyLib;
using MapGeneration;
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
using Time = UnityEngine.Time;

namespace WW_SYSTEM.Patches
{
	public class PlayerTriggerTeslaEventPatch
	{


		[HarmonyPatch(typeof(TeslaGateController), "FixedUpdate")]
		public class PlayerTriggerTeslaNewEventPatch
		{

			public static bool Prefix(TeslaGateController __instance)
			{




				try
				{

					if (NetworkServer.active)
					{
						// Dictionary<GameObject, ReferenceHub> allHubs = ReferenceHub.GetAllHubs();
						// using (List<TeslaGate>.Enumerator enumerator = __instance.TeslaGates.GetEnumerator())
						//  {
						// while (enumerator.MoveNext())
						// {

						foreach (var teslaGate in __instance.TeslaGates.Where(tesla => tesla.isActiveAndEnabled))
						{
							if (teslaGate.InactiveTime > 0f)
							{
								teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
							}
							else
							{

								bool flag = false;
								bool flag2 = false;
								bool TriggerablePlayerDetected = false;
								foreach (Player pl in Server.Players.Where(player => player.CurRoomIdentifier != null && player.CurRoomIdentifier.Name == RoomName.HczTesla && player.Role != RoleTypeId.Spectator))
								{

									if (Round.TeslaIgnore079 && pl.Role == RoleTypeId.Scp079) continue;

									if (!flag)
									{
										flag = teslaGate.PlayerInIdleRange(pl.Hub);
									}
									if (!flag2 && teslaGate.PlayerInRange(pl.Hub) && !teslaGate.InProgress)
									{
										flag2 = true;
									}


									PlayerTriggerTeslaEvent playerTriggerTeslaEvent = new PlayerTriggerTeslaEvent(pl, (flag || flag2));
									EventManager.Manager.HandleEvent<IEventHandlerPlayerTriggerTesla>(playerTriggerTeslaEvent);

									if (!playerTriggerTeslaEvent.Triggerable)
									{
										if (TriggerablePlayerDetected) continue;

										flag = false;
										flag2 = false;

									}
									else if (!TriggerablePlayerDetected)
									{
										TriggerablePlayerDetected = true;
									}
								}

								if (flag2)
								{
									teslaGate.ServerSideCode();
								}
								if (flag != teslaGate.isIdling)
								{
									teslaGate.ServerSideIdle(flag);
								}
							}
						}
						
					}


					foreach (TeslaGate teslaGate2 in __instance.TeslaGates)
					{
						teslaGate2.ClientSideCode();
					}


					return false;
				}
				catch (Exception arg)
				{


					Logger.Error("[EVENT MANAGER]", string.Format("PlayerTriggerTeslaEvent error: {0}", arg));
				}
				return true;
			}
		}
	}
}
