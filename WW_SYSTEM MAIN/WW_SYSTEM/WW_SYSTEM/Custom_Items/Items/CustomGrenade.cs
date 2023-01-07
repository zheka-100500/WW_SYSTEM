using InventorySystem.Items.ThrowableProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Custom_Items.Items
{
    public class CustomGrenade : CustomItem
    {

        public virtual bool ExplodeFrag(Player Player, List<Collider> Colliders, ExplosionGrenade Grenade, LayerMask DetectionMask)
        {
            return true;
        }

        public virtual bool ExplodeFlash(Player Player, List<Player> Players, FlashbangGrenade Grenade, LayerMask BlindingMask)
        {
            return true;
        }
    }
}
