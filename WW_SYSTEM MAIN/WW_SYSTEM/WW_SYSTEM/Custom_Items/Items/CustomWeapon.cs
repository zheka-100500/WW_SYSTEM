using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Custom_Items.Items
{
    public class CustomWeapon : CustomItem
    {
        public virtual bool Shoot(Player player, float distance, IDestructible component, float damage, Firearm firearm, RaycastHit hit)
        {
            return true;
        }

        public virtual bool Reload(Player player, ItemIdentifier item)
        {
            return true;
        }

    }
}
