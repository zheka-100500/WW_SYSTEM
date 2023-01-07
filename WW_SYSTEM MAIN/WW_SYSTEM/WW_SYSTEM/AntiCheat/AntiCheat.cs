using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;

namespace WW_SYSTEM.AntiCheat
{
    public static class AntiCheat
    {
        public static bool IsReady { get; private set; }

        public static bool EnableOutput { get; set; }

        private static Dictionary<AntiCheatRule, List<string>> Rules = new Dictionary<AntiCheatRule, List<string>>();



        public static bool UseWW_SYSTEMTeleportSystem = false;

        public static void Init()
        {
            if (IsReady) return;
            Rules.Clear();
            foreach (var item in Enum.GetValues(typeof(AntiCheatRule)))
            {
                Rules.Add((AntiCheatRule)item, new List<string>());
            }

            Logger.Info("AC", "READY!");
            IsReady = true;
        }

        public static void ClearRules()
        {
            foreach (var item in Rules)
            {
                item.Value.Clear();
            }
        }

        public static void AddAcLog(string msg)
        {
            if (!EnableOutput) return;

            Logger.Warn("AC", msg);
        }

        public static void AddRule(AntiCheatRule rule, string UserId = "@")
        {
            if (!Rules[rule].Contains(UserId))
            {
                Rules[rule].Add(UserId);
            }
        }

        public static void RemoveRule(AntiCheatRule rule, string UserId = "@")
        {
            if (Rules[rule].Contains(UserId))
            {
                Rules[rule].Remove(UserId);
            }
        }

        public static bool HasRule(AntiCheatRule rule, string UserId = "@")
        {
            return Rules[rule].Contains(UserId);
        }


    }
}
