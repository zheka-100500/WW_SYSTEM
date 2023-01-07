using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using Utils.Networking;
using UnityEngine;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.FirstPersonControl;
using CustomPlayerEffects;
using PlayerStatsSystem;

namespace WW_SYSTEM.Patches
{
    public class SCPRequestAttackEventPatch
    {

        //[HarmonyPatch(typeof(Scp939Role), nameof(Scp939.ServerReceivedAttackMsg))]
        //public class SCPRequestAttackEventPatch1
        //{
        //	public static bool Prefix(NetworkConnection conn, Scp939AttackMessage msg)
        //	{
        //		ReferenceHub referenceHub;
        //		if (ReferenceHub.TryGetHubNetID(conn.identity.netId, out referenceHub))
        //              {
        //			Scp939 scp;
        //			if ((scp = (referenceHub.scpsController.CurrentScp as Scp939)) == null)
        //			{
        //				return false;
        //			}
        //			if (scp.CurrentBiteCooldown > 0f)
        //			{
        //				return false;
        //			}

        //			if (Round.TryGetPlayer(referenceHub, out var pl))
        //                  {
        //				SCPRequestAttackEvent ev = new SCPRequestAttackEvent(pl, pl.Role, true, false);

        //                      if (ev.SendHitMarker && !ev.Allow)
        //                      {
        //					scp.CurrentBiteCooldown = 1f;
        //					Hitmarker.SendHitmarker(referenceHub, 1.5f);
        //					new Scp939OnHitMessage(referenceHub).SendToAuthenticated(0);
        //				}

        //				if (!ev.Allow) return false;

        //			}



        //		}



        //		return true;
        //	}
        //}

        [HarmonyPatch(typeof(Scp173SnapAbility), nameof(Scp173SnapAbility.ServerProcessCmd))]
        public class SCPRequestAttackEventPatch2
        {
            public static bool Prefix(NetworkReader reader, Scp173SnapAbility __instance)
            {

                try
                {
                    byte[] buffer = new byte[reader.Length];
                    reader.buffer.ToArray().CopyTo(buffer, 0);
                    NetworkReader r = new NetworkReader(buffer);

                    var targetHub = r.ReadReferenceHub();
                    if (__instance._observersTracker.IsObserved)
                    {
                        return false;
                    }
                    IFpcRole fpcRole;
                    if (targetHub == null || (fpcRole = (targetHub.roleManager.CurrentRole as IFpcRole)) == null)
                    {
                        return false;
                    }
                    if (__instance.IsSpeeding)
                    {
                        return false;
                    }

                    if (Round.TryGetPlayer(targetHub, out var pl))
                    {
                        SCPRequestAttackEvent ev = new SCPRequestAttackEvent(pl, pl.Role, true, false);

                        if (ev.SendHitMarker && !ev.Allow)
                        {
                            Hitmarker.SendHitmarker(__instance.Owner, 1f);
                            Scp173AudioPlayer scp173AudioPlayer;
                            if (__instance.ScpRole.SubroutineModule.TryGetSubroutine<Scp173AudioPlayer>(out scp173AudioPlayer))
                            {
                                scp173AudioPlayer.ServerSendSound(Scp173AudioPlayer.Scp173SoundId.Snap);
                            }
                        }

                        if (!ev.Allow) return false;

                    }
                    return true;
                }
                catch (Exception ex)
                {

                    Logger.Error("Scp173SnapAbility::patch", ex.Message);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Scp049AttackAbility), nameof(Scp049AttackAbility.ServerProcessCmd))]
        public class SCPRequestAttackEventPatch3
        {
            public static bool Prefix(NetworkReader reader, Scp049AttackAbility __instance)
            {
                try
                {
                    byte[] buffer = new byte[reader.Length];
                    reader.buffer.ToArray().CopyTo(buffer,0);
                    NetworkReader r = new NetworkReader(buffer);

                    if (!__instance.Cooldown.IsReady || __instance._resurrect.IsInProgress)
                    {
                        return false;
                    }
                    var target = r.ReadReferenceHub();
                    if (target == null || !__instance.IsTargetValid(target))
                    {
                        return false;
                    }

                    if (Round.TryGetPlayer(__instance.Owner, out var pl))
                    {
                        SCPRequestAttackEvent ev = new SCPRequestAttackEvent(pl, pl.Role, true, false);

                        if (ev.SendHitMarker && !ev.Allow)
                        {
                            //	Hitmarker.SendHitmarker(__instance.Hub, 1.35f);
                            Hitmarker.SendHitmarker(__instance.Owner, 1f);
                            __instance.Cooldown.Trigger(1.5f);

                        }

                        if (!ev.Allow) return false;

                    }
                    return true;
                }
                catch (Exception ex)
                {

                    Logger.Error("Scp049AttackAbility::Patch", ex.Message);
                }
                return true;
            }
        }

    }
}
