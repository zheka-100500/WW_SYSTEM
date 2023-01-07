using System;
using System.Collections.Generic;
using System.Text;

namespace WW_SYSTEM_BOT
{
    [Serializable]
    public class DiscordRequest
    {
        public string NickName;
        public ulong Id;
        public string Request;

        public DiscordRequest(string NickName, ulong Id, string Request)
        {
            this.NickName = NickName;
            this.Id = Id;
            this.Request = Request;
        }

    }
}
