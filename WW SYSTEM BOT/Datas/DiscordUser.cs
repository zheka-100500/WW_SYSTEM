using System;
using System.Collections.Generic;
using System.Text;

namespace WW_SYSTEM_BOT.Datas
{
    public class DiscordUser
    {
        public ulong Id = 0;
        public string NickName = "";
        public string Discriminator = "";
        public string Avatar = "";
        public List<DiscordRole> Roles = new List<DiscordRole>();
        public DiscordUser(ulong Id, string NickName, string Discriminator, string Avatar)
        {
            this.Id = Id;
            this.NickName = NickName;
            this.Discriminator = Discriminator;
            this.Avatar = Avatar;
        }

        public Packet GetPacket(Packet p)
        {
 
            p.Write(Id);
            p.Write(NickName);
            p.Write(Discriminator);
            p.Write(Avatar);

            var R = new List<string>();
            foreach (var item in Roles)
            {
                R.Add(item.GetRoleString());
            }
            p.Write(R);

            return p;
        }


    }
}
