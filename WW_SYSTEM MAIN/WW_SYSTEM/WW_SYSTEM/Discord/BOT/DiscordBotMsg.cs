using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    [Serializable]
    public class DiscordBotMsg
    {
        public string SecretKey;
        public string Data;
        public DiscordBotMsgType Type;
        public DiscordBotMsg(string SecretKey, string Data, DiscordBotMsgType Type)
        {
            this.SecretKey = SecretKey;
            this.Data = Data;
            this.Type = Type;
        }
    }
}
