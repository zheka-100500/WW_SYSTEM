using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Translation;

namespace WW_SYSTEM.API
{
    public static class OtherExtensions
    {



        public static bool IsInteraction(this ExperienceType type)
        {
            return type == ExperienceType.USE_DOOR || type == ExperienceType.USE_ELEVATOR || type == ExperienceType.USE_LOCKDOWN || type == ExperienceType.USE_TESLAGATE;
        }

  

        public static Player Player(this ReferenceHub hub)
        {
            
            return Round.GetPlayer(hub);
        }

        public static bool TryGetPlayer(this ReferenceHub hub, out Player player)
        {
            if(Round.TryGetPlayer(hub, out var pl))
            {
                player = pl;
                return true;
            }
            player = null;
            return false;
        }

       

    }
}
