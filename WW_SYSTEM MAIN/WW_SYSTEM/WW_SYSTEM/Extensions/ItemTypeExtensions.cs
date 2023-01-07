using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class ItemTypeExtensions
    {
        public static bool IsWeapon(this ItemType type)=> type.ToString().ToUpper().Contains("GUN");
        public static bool IsArmor(this ItemType type) => type.ToString().ToUpper().Contains("ARMOR");
        public static bool IsGrenade(this ItemType type)=> type.ToString().ToUpper().Contains("GRENADE");
        public static bool IsKeyCard(this ItemType type)=> type.ToString().ToUpper().Contains("KEYCARD");
        public static bool IsScp(this ItemType type) => type.ToString().ToUpper().Contains("SCP");
    

        public static AmmoType GetAmmo(this ItemType type)
        {
            switch (type)
            {
                case ItemType.Ammo556x45:
                    return AmmoType.Ammo556x45;
                case ItemType.Ammo44cal:
                    return AmmoType.Ammo44cal;
                case ItemType.Ammo762x39:
                    return AmmoType.Ammo762x39;
                case ItemType.Ammo9x19:
                    return AmmoType.Ammo9x19;
                case ItemType.Ammo12gauge:
                    return AmmoType.Ammo12gauge;
                default:
                    break;
            }

            return AmmoType.NONE;

        }

        public static ItemType GetItem(this AmmoType type)
        {

            switch (type)
            {
                case AmmoType.Ammo556x45:
                    return ItemType.Ammo556x45;
                case AmmoType.Ammo44cal:
                    return ItemType.Ammo44cal;
                case AmmoType.Ammo762x39:
                    return ItemType.Ammo762x39;
                case AmmoType.Ammo9x19:
                    return ItemType.Ammo9x19;
                case AmmoType.Ammo12gauge:
                    return ItemType.Ammo12gauge;
                default:
                    break;
            }

            return ItemType.None;

        }
    }
}
