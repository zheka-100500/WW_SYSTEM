using GameCore;
using HarmonyLib;
using InventorySystem.Disarming;
using InventorySystem.Items;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Networking;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
	public class PlayerHandcuffedEventPatch
	{
		public static bool Prefix(NetworkConnection conn, DisarmMessage msg)
		{
			try
			{
                if (!NetworkServer.active || !DisarmingHandlers.ServerCheckCooldown(conn))
                {
                    return false;
                }
                ReferenceHub referenceHub;
                if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out referenceHub))
                {
                    return false;
                }
                if (!msg.PlayerIsNull)
				{
					if ((msg.PlayerToDisarm.transform.position - referenceHub.transform.position).sqrMagnitude > 20f)
					{
						return false;
					}
					if (msg.PlayerToDisarm.inventory.CurInstance != null && msg.PlayerToDisarm.inventory.CurInstance.TierFlags != ItemTierFlags.Common)
					{
						return false;
					}
				}
				bool flag = !msg.PlayerIsNull && msg.PlayerToDisarm.inventory.IsDisarmed();
				bool flag2 = !msg.PlayerIsNull && referenceHub.CanDisarm(msg.PlayerToDisarm);
				if (flag && !msg.Disarm)
				{
					if (!referenceHub.inventory.IsDisarmed())
					{
						if(Round.TryGetPlayer(referenceHub, out var Owner) && Round.TryGetPlayer(msg.PlayerToDisarm, out var target))
                        {
							PlayerHandcuffedEvent ev = new PlayerHandcuffedEvent(target, Owner, DisarmType.Release, true);
							EventManager.Manager.HandleEvent<IEventHandlerHandcuffed>(ev);
							if (!ev.Allow) return false;
						}
						

						msg.PlayerToDisarm.inventory.SetDisarmedStatus(null); 
					}
				}
				else
				{
					if (flag || !flag2 || !msg.Disarm)
					{
						referenceHub.networkIdentity.connectionToClient.Send<DisarmedPlayersListMessage>(DisarmingHandlers.NewDisarmedList, 0);
						return false;
					}
					if (msg.PlayerToDisarm.inventory.CurInstance == null || msg.PlayerToDisarm.inventory.CurInstance.CanHolster())
					{
						if (Round.TryGetPlayer(referenceHub, out var Owner) && Round.TryGetPlayer(msg.PlayerToDisarm, out var target))
						{
							PlayerHandcuffedEvent ev = new PlayerHandcuffedEvent(target, Owner, DisarmType.Disarm, true);
							EventManager.Manager.HandleEvent<IEventHandlerHandcuffed>(ev);
							if (!ev.Allow) return false;
						}

						msg.PlayerToDisarm.inventory.SetDisarmedStatus(referenceHub.inventory);
					}
				}
				DisarmingHandlers.NewDisarmedList.SendToAuthenticated(0);


				return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerHandcuffedEventPatch error: {0}", arg));
			}
			return true;
		}

	}

}
