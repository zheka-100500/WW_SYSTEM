using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items.Items;
using InventorySystem;
using InventorySystem.Items;

namespace WW_SYSTEM.Custom_Items
{
    public static class CustomItemManager
    {
        public static Dictionary<int, CustomItem> AllItems = new Dictionary<int, CustomItem>();
        public static Dictionary<ushort, CustomItem> ItemsInRound = new Dictionary<ushort, CustomItem>();


  

        public static bool HasItem(int id)
        {
            return AllItems.ContainsKey(id);
        }

        public static bool ItemIsCustom(ushort Serial)
        {
            return ItemsInRound.ContainsKey(Serial);
        }

        

        public static bool TryGetItem(ushort Serial, CustomItemType type, out CustomItem customItem)
        {
            if (ItemIsCustom(Serial))
            {
                var item = ItemsInRound[Serial];
                if(item.CustomItemType == type)
                {
                    customItem = item;
                    return true;
                }
            }
            customItem = null;
            return false;
        }
        public static bool TryGetItem(ushort Serial, out CustomItem customItem)
        {
            if (ItemIsCustom(Serial))
            {
                var item = ItemsInRound[Serial];
                    customItem = item;
                    return true;
            }
            customItem = null;
            return false;
        }

        public static int GetItemIndex(CustomItem item)
        {
            foreach (var i in AllItems)
            {
                if(i.Value == item)
                {
                    return i.Key;
                }
            }
            return -1;
        }

        public static bool TryGetItem(int id, out CustomItem item)
        {
            if (HasItem(id))
            {
                item = AllItems[id];
                return true;
            }
            item = null;
            return false;
        }

        public static bool TryGetItem(string name, out CustomItem item)
        {
            foreach (var i in AllItems)
            {
                if(i.Value.Name == name)
                {
                    item = i.Value;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public static int AddCustomItem(CustomItem item)
        {
            foreach (var i in AllItems)
            {
                if (i.Value == item) return -1;
            }
            int id = AllItems.Count + 1;
            item.Load();
            item.ID = id;
            if((item as CustomGrenade) != null)
            {
                item.CustomItemType = CustomItemType.GRENADE;
            }
            if ((item as CustomMedical) != null)
            {
                item.CustomItemType = CustomItemType.MEDICAL;
            }
            if ((item as CustomWeapon) != null)
            {
                item.CustomItemType = CustomItemType.WEAPON;
            }
            AllItems.Add(id, item);
            return id;
        }

        public static void RemoveCustomItem(int id)
        {
            if (AllItems.ContainsKey(id))
            {
                AllItems[id].UnLoad();

                AllItems.Remove(id);
            }
        }


        public static bool SpawnItem(Vector3 pos, Quaternion rotation, int id)
        {
            if(TryGetItem(id, out var item))
            {
              var pickup = Round.SpawnItem(item.ItemType, pos, rotation);
                ItemsInRound.Add(pickup.NetworkInfo.Serial, item);
                return true;
            }
            return false;
        }


        public static void GiveItem(Player pl, int CustomItemID)
        {
            if (TryGetItem(CustomItemID, out var item))
            {
             var invitem = pl.inv.ServerAddItem(item.ItemType);
                ItemsInRound.Add(invitem.ItemSerial, item);
            }
        }

        public static int MsgDur = 5;
        public static string PickupMessage = "You picked up {item}";
        

    }
}
