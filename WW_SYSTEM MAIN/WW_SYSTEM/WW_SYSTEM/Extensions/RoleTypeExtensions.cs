using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class RoleTypeExtensions
    {

        public static bool IsTeam(this RoleTypeId type, TEAMTYPE Team)
        {
            bool Result = false;
            switch (Team)
            {
                case TEAMTYPE.Mtf:
                    if (type == RoleTypeId.FacilityGuard || type == RoleTypeId.NtfCaptain || type == RoleTypeId.NtfPrivate || type == RoleTypeId.NtfSergeant || type == RoleTypeId.NtfSpecialist)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.Science:
                    if(type == RoleTypeId.Scientist)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.Chaos:
                    if (type == RoleTypeId.ChaosConscript || type == RoleTypeId.ChaosMarauder || type == RoleTypeId.ChaosRepressor || type == RoleTypeId.ChaosRifleman)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.Classd:
                    if (type == RoleTypeId.ClassD)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.MtfAndScience:
                    if (type == RoleTypeId.FacilityGuard || type == RoleTypeId.NtfCaptain || type == RoleTypeId.NtfPrivate || type == RoleTypeId.NtfSergeant || type == RoleTypeId.NtfSpecialist || type == RoleTypeId.Scientist)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.ChaosAndClassd:
                    if (type == RoleTypeId.ChaosConscript || type == RoleTypeId.ChaosMarauder || type == RoleTypeId.ChaosRepressor || type == RoleTypeId.ChaosRifleman || type == RoleTypeId.ClassD)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.SCP:
                    if (type == RoleTypeId.Scp049 || type == RoleTypeId.Scp0492 || type == RoleTypeId.Scp079 || type == RoleTypeId.Scp096 || type == RoleTypeId.Scp106 || type == RoleTypeId.Scp173 || type == RoleTypeId.Scp939)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.NONE:
                    if (type == RoleTypeId.None)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.Spectator:
                    if (type == RoleTypeId.Spectator)
                    {
                        Result = true;
                    }
                    goto END;
                case TEAMTYPE.Other:
                    if (type == RoleTypeId.Tutorial)
                    {
                        Result = true;
                    }
                    goto END;
                default:
                    Result = false;
                    goto END;
            }

        END:
            return Result;
        }

        public static TEAMTYPE GetTeam(this RoleTypeId type, bool AdvancedTeam = false)
        {
            var Result = TEAMTYPE.NONE;
            switch (type)
            {
                case RoleTypeId.None:
                    Result = TEAMTYPE.NONE;
                    break;
                case RoleTypeId.Scp106:
                case RoleTypeId.Scp049:
                case RoleTypeId.Scp173:
                case RoleTypeId.Scp079:
                case RoleTypeId.Scp096:
                case RoleTypeId.Scp939:
                case RoleTypeId.Scp0492:
                    Result = TEAMTYPE.SCP;
                    break;
                case RoleTypeId.ClassD:
                    Result = AdvancedTeam ? TEAMTYPE.ChaosAndClassd : TEAMTYPE.Classd;
                    break;
                case RoleTypeId.Spectator:
                    Result = TEAMTYPE.Spectator;
                    break;
                case RoleTypeId.NtfSergeant:
                case RoleTypeId.NtfPrivate:
                case RoleTypeId.NtfCaptain:
                case RoleTypeId.NtfSpecialist:
                case RoleTypeId.FacilityGuard:
                    Result = AdvancedTeam ? TEAMTYPE.MtfAndScience : TEAMTYPE.Mtf;
                    break;
                case RoleTypeId.Scientist:
                    Result = AdvancedTeam ? TEAMTYPE.MtfAndScience : TEAMTYPE.Science;
                    break;
                case RoleTypeId.Tutorial:
                    Result = TEAMTYPE.Other;
                    break;
                case RoleTypeId.ChaosConscript:
                case RoleTypeId.ChaosRifleman:
                case RoleTypeId.ChaosRepressor:
                case RoleTypeId.ChaosMarauder:
                    Result = AdvancedTeam ? TEAMTYPE.ChaosAndClassd : TEAMTYPE.Chaos;
                    break;
            }

            return Result;
        }

        public static int GetScpNumber(this RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.Scp173:
                    return 173;
                case RoleTypeId.Scp106:
                    return 106;
                case RoleTypeId.Scp049:
                    return 049;
                case RoleTypeId.Scp079:
                    return 079;
                case RoleTypeId.Scp096:
                    return 096;
                case RoleTypeId.Scp0492:
                    return 0492;
                case RoleTypeId.Scp939:
                    return 939;
                default:
                    return -1;
            }
        }

        public static string GetScpNumberAsString(this RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.Scp173:
                    return "173";
                case RoleTypeId.Scp106:
                    return "106";
                case RoleTypeId.Scp049:
                    return "049";
                case RoleTypeId.Scp079:
                    return "079";
                case RoleTypeId.Scp096:
                    return "096";
                case RoleTypeId.Scp0492:
                    return "049-2";
                case RoleTypeId.Scp939:
                    return "939";
                default:
                    return "-1";
            }
        }


    }
}
