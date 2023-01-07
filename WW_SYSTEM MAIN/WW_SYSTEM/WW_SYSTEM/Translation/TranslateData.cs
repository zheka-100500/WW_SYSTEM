using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Translation
{
    [Serializable]
    public class TranslateData
    {
        public List<Translate> Translates = new List<Translate>();

        public string GetTranslation(DamageType type)
        {
            return GetTranslation($"DAMAGE_{type}");
        }
        public string GetTranslation(TEAMTYPE team)
        {
            return GetTranslation($"TEAM_{team}");
        }
        public string GetTranslation(RoleTypeId role)
        {
            return GetTranslation($"ROLE_{role}");
        }

        public string GetTranslation(PLAYER_PERMISSION perm)
        {
            return GetTranslation($"PERM_{perm}");
        }

        public string GetTranslation(ExperienceType type)
        {
            return GetTranslation($"XP_079_{type}");
        }

        public string GetTranslation(string key)
        {
            foreach (var item in Translates)
            {
                if(item.TranslateKey.ToUpper() == key.ToUpper())
                {
                    return item.TranslateValue;
                }
            }
            return $"NO_TRANSLATION FOR {key.ToUpper()} (If you are an administrator, try load Defaults.)";
        }

        public Translate GetTranslationData(string key)
        {
            foreach (var item in Translates)
            {
                if (item.TranslateKey.ToUpper() == key.ToUpper())
                {
                    return item;
                }
            }
            return null;
        }

        public bool HasTranslation(string key)
        {
            key = key.ToUpper();
            foreach (var item in Translates)
            {
                if (item.TranslateKey.ToUpper() == key)
                {
                    return true;
                }
            }
            return false;
        }

        public TranslateData(List<Translate> translates)
        {
            Translates = translates;
          
        }

    }
}
