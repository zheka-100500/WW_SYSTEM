using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    public class DiscordRole
    {
        public ulong RoleId = 0;
        public string RoleName = "";
        public int ColorR = 0;
        public int ColorG = 0;
        public int ColorB = 0;


        public DiscordRole(string PacketData)
        {
        
            var datas = PacketData.Split(new char[] { '/' });
            if(datas.Length >= 5)
            {
                if(ulong.TryParse(datas[0], out var id))
                {
                    RoleId = id;
                }

                RoleName = datas[1];
                if (int.TryParse(datas[0], out var R))
                {
                    ColorR = R;
                }
                if (int.TryParse(datas[0], out var G))
                {
                    ColorG = G;
                }
                if (int.TryParse(datas[0], out var B))
                {
                    ColorB = B;
                }
            }
        }
    }
}
