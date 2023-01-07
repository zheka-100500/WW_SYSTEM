using System;
using System.Collections.Generic;
using System.Text;

namespace WW_SYSTEM_BOT
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
