using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    public class DiscordUser
    {
        public ulong Id = 0;
        public string NickName = "";
        public string Discriminator = "";
        public string Avatar = "";
        public List<DiscordRole> Roles = new List<DiscordRole>();

        public DiscordUser(Packet p)
        {
            Id = p.ReadULong();
            NickName = p.ReadString();
            Discriminator = p.ReadString();
            Avatar = p.ReadString();

            foreach (var item in p.ReadStrings())
            {
                Roles.Add(new DiscordRole(item));
            }
        }
       
    }
}
