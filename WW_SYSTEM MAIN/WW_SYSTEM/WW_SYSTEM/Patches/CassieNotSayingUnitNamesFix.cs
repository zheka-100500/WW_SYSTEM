using HarmonyLib;
using Respawning.NamingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WW_SYSTEM.Patches
{
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.GetCassieUnitName))]
    public static class CassieNotSayingUnitNamesFix
    {
        private static bool Prefix(NineTailedFoxNamingRule __instance, string regular, ref string __result)
        {
            try
            {
                string[] array = Regex.Replace(regular, "<[^>]*?>", string.Empty).Split(new char[] { '-' });

                __result = $"NATO_{array[0][0]} {array[1]}";
            }
            catch
            {
                Logger.Error("CassieNotSayingUnitNamesFix", "Error, couldn't convert '" + regular + "' into a CASSIE-readable form.");
                __result = "ERROR";
            }

            return false;
        }
    }
}
