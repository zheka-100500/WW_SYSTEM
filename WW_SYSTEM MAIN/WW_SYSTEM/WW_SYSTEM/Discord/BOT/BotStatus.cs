using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    [Serializable]
    public class BotStatus
    {
        public string Status;
        public int Type;
        public BotStatus(string status, int type)
        {
            Status = status;
            Type = type;
        }
    }
}
