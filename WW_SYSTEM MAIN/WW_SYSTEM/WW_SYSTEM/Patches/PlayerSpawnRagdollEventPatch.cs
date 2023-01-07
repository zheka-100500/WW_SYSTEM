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
using Mirror;
using WW_SYSTEM.Translation;
using PlayerStatsSystem;
using PlayerRoles.Ragdolls;
using static PlayerArms;
using PlayerRoles;

namespace WW_SYSTEM.Patches
{
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    public class PlayerSpawnRagdollPatch
    {

        public static bool Prefix(ReferenceHub owner, DamageHandlerBase handler, ref BasicRagdoll __result)
        {

            try
            {
                if (!NetworkServer.active || owner == null)
                {
                    __result = null;
                    return false;
                }
                IRagdollRole ragdollRole;
                if ((ragdollRole = (owner.roleManager.CurrentRole as IRagdollRole)) == null)
                {
                    __result = null;
                    return false;
                }

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

                Player pl = Round.GetPlayer(owner);

                PlayerSpawnRagdollEvent ev = new PlayerSpawnRagdollEvent(pl, pl.Role, pl.GetPosition().ToVector(), pl.GetRotations().ToVector(), Attacker, handler, true);
                EventManager.Manager.HandleEvent<IEventHandlerSpawnRagdoll>(ev);
                if(ev.Allow)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ragdollRole.Ragdoll.gameObject);
                    BasicRagdoll basicRagdoll;
                    if (gameObject.TryGetComponent<BasicRagdoll>(out basicRagdoll))
                    {
                        Transform transform = ragdollRole.Ragdoll.transform;
                        basicRagdoll.NetworkInfo = new RagdollData(owner, handler, transform.localPosition, transform.localRotation);
                    }
                    else
                    {
                        basicRagdoll = null;
                    }
                    NetworkServer.Spawn(gameObject);
                    __result = basicRagdoll;
                }else
                {
                    __result = null;
                }

                return false;
            }
            catch (Exception ex)
            {

                Logger.Error("[EVENT MANAGER]", string.Format("PlayerSpawnRagdollPatch error: {0}", ex));
            }
            return true;
        }
    }
}
