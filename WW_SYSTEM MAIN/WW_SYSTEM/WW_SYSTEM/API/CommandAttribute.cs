using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        int min_arg_count = 0;
        int max_arg_count = -1;

        public int MinArgCount
        {
            get { return min_arg_count; }
            set { min_arg_count = value; }
        }

        public int MaxArgCount
        {
            get { return max_arg_count; }
            set { max_arg_count = value; }
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string[] Usage { get; set; }

        public string[] Aliases { get; set; }

        PLAYER_PERMISSION permissions = PLAYER_PERMISSION.NONE;

        string custom_permissions = "";

        RoleTypeId role = RoleTypeId.None;

        TEAMTYPE team = TEAMTYPE.NONE;

        bool hideInHelp = false;

        public PLAYER_PERMISSION Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }

        public string Custom_Permissions
        {
            get { return custom_permissions; }
            set { custom_permissions = value; }
        }

        public RoleTypeId Role
        {
            get { return role; }
            set { role = value; }
        }
        public TEAMTYPE Team
        {
            get { return team; }
            set { team = value; }
        }

        public bool HideInHelp
        {
            get { return hideInHelp; }
            set { hideInHelp = value; }
        }

        public CommandType type { get; set; }

        public CommandAttribute(string command_name = null, CommandType type = CommandType.GameConsole)
        {
            Name = command_name;
            this.type = type;

        }
    }
}
