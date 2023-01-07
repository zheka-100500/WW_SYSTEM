using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Translation;

namespace WW_SYSTEM.API
{
    public static class DamageTypeExtensions
    {


        public static bool IsWeapon(this DamageType type)
        {
            switch (type)
            {
                case DamageType.GunCOM15:
                case DamageType.GunE11SR:
                case DamageType.GunCrossvec:
                case DamageType.GunFSP9:
                case DamageType.GunLogicer:
                case DamageType.GunRevolver:
                case DamageType.GunCOM18:
                case DamageType.GunAK:
                case DamageType.GunShotgun:
                case DamageType.Weapon:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsScp(this DamageType type)
        {
            switch (type)
            {
                case DamageType.Scp049:
                case DamageType.Scp207:
                case DamageType.Scp096:
                case DamageType.Scp173:
                case DamageType.Scp939:
                case DamageType.Scp0492:
                case DamageType.Scp106:
                case DamageType.Scp079:
                case DamageType.Zombie:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetDamageName(this DamageHandlerBase damageHandler)
        {
            var type = damageHandler.GetDamageType();
            if (type == DamageType.Custom)
            {
                CustomReasonDamageHandler Custom;
                if ((Custom = (damageHandler as CustomReasonDamageHandler)) != null)
                {
                    return Custom._deathReason;
                }
            }
            var Reason = Translator.MainTranslation.GetTranslation(type);
            if (Reason.Contains("NO_TRANSLATION"))
            {
                return type.ToString();
            }
            else
            {
                return Reason;
            }
        }

        public static DamageType GetDamageType(this DamageHandlerBase damageHandler)
        {

            if (damageHandler == null) return DamageType.Unknown;

            var reason = damageHandler.ServerLogsText.ToLower();
            if (reason.Contains("recontained")) return DamageType.Recontained;
            if (reason.Contains("alpha warhead")) return DamageType.Warhead;
            if (reason.Contains("scp-049")) return DamageType.Scp049;
            if (reason.Contains("unknown")) return DamageType.Unknown;
            if (reason.Contains("asphyxiated")) return DamageType.Asphyxiated;
            if (reason.Contains("bleeding")) return DamageType.Bleeding;
            if (reason.Contains("fall")) return DamageType.Falldown;
            if (reason.Contains("decayed")) return DamageType.PocketDecay;
            if (reason.Contains("melted")) return DamageType.Decontamination;
            if (reason.Contains("poison")) return DamageType.Poisoned;
            if (reason.Contains("scp-207")) return DamageType.Scp207;
            if (reason.Contains("severed")) return DamageType.SeveredHands;
            if (reason.Contains("micro h.i.d")) return DamageType.MicroHID;
            if (reason.Contains("tesla")) return DamageType.Tesla;
            if (reason.Contains("explosion")) return DamageType.Explosion;
            if (reason.Contains("scp-096")) return DamageType.Scp096;
            if (reason.Contains("scp-173")) return DamageType.Scp173;
            if (reason.Contains("scp-939")) return DamageType.Scp939;
            if (reason.Contains("scp-049-2")) return DamageType.Zombie;
            if (reason.Contains("crushed")) return DamageType.Crushed;
            if (reason.Contains("friendly fire")) return DamageType.FriendlyFireDetector;
            if (reason.Contains("hypothermia")) return DamageType.Hypothermia;

            ScpDamageHandler scphandler;
            if ((scphandler = (damageHandler as ScpDamageHandler)) != null)
            {
                switch (scphandler.Attacker.Role)
                {
                    case RoleTypeId.Scp173:
                        return DamageType.Scp173;
                    case RoleTypeId.Scp106:
                        return DamageType.Scp106;
                    case RoleTypeId.Scp049:
                        return DamageType.Scp049;
                    case RoleTypeId.Scp079:
                        return DamageType.Scp079;
                    case RoleTypeId.Scp096:
                        return DamageType.Scp096;
                    case RoleTypeId.Scp0492:
                        return DamageType.Scp0492;
                    case RoleTypeId.Scp939:
                        return DamageType.Scp939;
                    default:
                        break;
                }
            }

            CustomReasonDamageHandler Custom;
            if ((Custom = (damageHandler as CustomReasonDamageHandler)) != null)
            {
                return DamageType.Custom;
            }

            FirearmDamageHandler Gun;
            if ((Gun = (damageHandler as FirearmDamageHandler)) != null)
            {
                switch (Gun.WeaponType)
                {
                    case ItemType.GunCOM15:
                        return DamageType.GunCOM15;
                    case ItemType.GunE11SR:
                        return DamageType.GunE11SR;
                    case ItemType.GunCrossvec:
                        return DamageType.GunCrossvec;
                    case ItemType.GunFSP9:
                        return DamageType.GunFSP9;
                    case ItemType.GunLogicer:
                        return DamageType.GunLogicer;
                    case ItemType.GunCOM18:
                        return DamageType.GunCOM18;
                    case ItemType.GunRevolver:
                        return DamageType.GunRevolver;
                    case ItemType.GunAK:
                        return DamageType.GunAK;
                    case ItemType.GunShotgun:
                        return DamageType.GunShotgun;
                    default:
                        return DamageType.Weapon;
                }

            }
            return DamageType.Unknown;
        }
    }
}
