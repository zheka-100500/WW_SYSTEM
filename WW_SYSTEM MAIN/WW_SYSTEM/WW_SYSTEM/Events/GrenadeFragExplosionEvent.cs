using InventorySystem.Items.ThrowableProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class GrenadeFragExplosionEvent : Event
    {

        public Player Player {get; set;}
        public List<Collider> Colliders { get; set; }
        public bool Allow { get; set; }
        public ExplosionGrenade Grenade { get; }

        public float GetDoorDamage(Vector3 doorPos)
        {
            float time = Vector3.Distance(doorPos, Grenade.transform.position);
            return Grenade._doorDamageOverDistance.Evaluate(time);
        }

        public float GetPlayerDamage(Vector3 playerPos)
        {

                       float time = Vector3.Distance(playerPos, Grenade.transform.position);
       return Grenade._playerDamageOverDistance.Evaluate(time);
    }

        public GrenadeFragExplosionEvent(Player Player, List<Collider> Colliders, ExplosionGrenade Grenade, bool Allow)
        {
            this.Player = Player;
            this.Colliders = Colliders;
            this.Allow = Allow;
            this.Grenade = Grenade;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerFragExplosion)handler).OnFragExplosion(this);
        }
    }
}
