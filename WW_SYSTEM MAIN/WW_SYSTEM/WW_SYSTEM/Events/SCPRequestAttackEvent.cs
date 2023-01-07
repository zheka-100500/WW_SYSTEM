using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class SCPRequestAttackEvent : Event
    {

        public RoleTypeId Role { get; set; }

        public Player Player { get; set; }

        public bool Allow { get; set; }

        public bool SendHitMarker { get; set; }

        public SCPRequestAttackEvent(Player Player, RoleTypeId Role, bool Allow, bool SendHitMarker)
        {
            this.Role = Role;
            this.Player = Player;
            this.Allow = Allow;
            this.SendHitMarker = SendHitMarker;
        }



        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerSCPRequestAttack)handler).OnSCPRequestAttack(this);
        }
    }
}
