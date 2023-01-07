using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    [Serializable]
    public class DiscordRequest
    {
        public DiscordUser User;
        public string Request;
        public ulong Channel;
        public DiscordRequest(DiscordUser User, string Request, ulong Channel)
        {
            this.User = User;
            this.Request = Request;
            this.Channel = Channel;
        }

    }
}
