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
using Time = UnityEngine.Time;
using Respawning;
using Respawning.NamingRules;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;

namespace WW_SYSTEM.Patches
{


    [HarmonyPatch(typeof(RoleSpawnpointManager), nameof(RoleSpawnpointManager.Init))]
    public class PlayerSpawnEventPatch
    {

        public static bool Prefix()
        {

            PlayerRoleManager.RoleChanged role = spawn;

            PlayerRoleManager.InvokeData(role);
            return false;
        }


       
        private static void spawn(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            IFpcRole fpcRole;
            if ((fpcRole = (newRole as IFpcRole)) == null)
            {
                return;
            }
            if (fpcRole.SpawnpointHandler == null)
            {
                return;
            }
            Vector3 position;
            float currentHorizontal;
            if (!fpcRole.SpawnpointHandler.TryGetSpawnpoint(out position, out currentHorizontal))
            {
                return;
            }
            PlayerSpawnEvent ev = new PlayerSpawnEvent(Round.GetPlayer(hub), position.ToVector(), currentHorizontal);
            EventManager.Manager.HandleEvent<IEventHandlerSpawn>(ev);

            hub.transform.position = ev.SpawnPos.ToVector3();
            fpcRole.FpcModule.MouseLook.CurrentHorizontal = ev.Rot;
        }


    }
}
