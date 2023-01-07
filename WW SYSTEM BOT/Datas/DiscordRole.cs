using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace WW_SYSTEM_BOT.Datas
{
    public class DiscordRole
    {
        public ulong RoleId = 0;
        public string RoleName = "";
        private int ColorR = 0;
        private int ColorG = 0;
        private int ColorB = 0;
        public DiscordRole(ulong RoleId, string RoleName, DiscordColor color)
        {
            this.RoleId = RoleId;
            this.RoleName = RoleName;
            SetColor(color);
        }

        private void SetColor(DiscordColor color)
        {
            ColorR = color.R;
            ColorG = color.G;
            ColorB = color.B;
        }

        public string GetRoleString()
        {
            return $"{RoleId}/{RoleName}/{ColorR}/{ColorG}/{ColorB}";
        }
    }
}
