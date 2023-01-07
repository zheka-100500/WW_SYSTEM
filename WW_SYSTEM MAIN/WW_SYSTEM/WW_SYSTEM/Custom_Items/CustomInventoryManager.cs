using InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items.Items;
namespace WW_SYSTEM.Custom_Items
{
    public class CustomInventoryManager : MonoBehaviour
    {
        public Player pl;




        public bool HasItem(int id)
        {
            foreach (var item in pl.Items)
            {
                if (CustomItemManager.ItemsInRound.ContainsKey(item.Key))
                {
                    if (CustomItemManager.ItemsInRound[item.Key].ID == id) return true;
                }
            }
            return false;
        }

        public bool AddItem(int id)
        {
            if (CustomItemManager.TryGetItem(id, out var item))
            {

                if (pl.Items.Count >= 8) return false;

                var invitem = pl.inv.ServerAddItem(item.ItemType);
                CustomItemManager.ItemsInRound.Add(invitem.ItemSerial, item);
                return true;
            }

            return false;
        }

        public ushort CurItem
        {
            get
            {
                return pl.CurItemIdentifier.SerialNumber;
            }
        }



        public bool CurIsCustom()
        {
            if (CustomItemManager.ItemsInRound.ContainsKey(CurItem))
            {
                return true;
            }
            return false;
        }

        public bool CurIsCustom(CustomItemType type)
        {
           
            if (CustomItemManager.ItemsInRound.ContainsKey(CurItem))
            {
               if(CustomItemManager.ItemsInRound[CurItem].CustomItemType == type)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryGetCurItem(out CustomItem item)
        {
            if (CurIsCustom())
            {

                item = CustomItemManager.ItemsInRound[CurItem];
                return true;
            }
            item = null;
            return false;
        }

    }
}
