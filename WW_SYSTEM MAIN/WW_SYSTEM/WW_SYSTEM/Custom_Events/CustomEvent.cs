using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Custom_Events
{
    public abstract class CustomEvent
    {


        public int Id;

        public string EventName;

        public int RequedPlayers;

        public void Info(string data)
        {


            Logger.Info(EventName, data);
        }

        public void Warn(string data)
        {


            Logger.Warn(EventName, data);
        }

        public void Error(string data)
        {


            Logger.Error(EventName, data);
        }


        public virtual void OnEventStarted()
        {

        }

        public virtual void OnEventStoped()
        {

        }

        public virtual void OnPlayerJoin(Player pl)
        {

        }

        public virtual void OnRoundStart(int RoundCount)
        {

        }
        public virtual void OnRoundEnd(int RoundCount)
        {

        }

        public virtual void OnPlayerDie(Player player, Player killer, StandardDamageHandler damageHandler)
        {

        }

        public virtual void OnPlayerLeave(Player pl)
        {

        }
    }
}
