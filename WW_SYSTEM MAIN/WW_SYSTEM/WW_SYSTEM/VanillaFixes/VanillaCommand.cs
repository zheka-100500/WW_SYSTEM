using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
namespace WW_SYSTEM.VanillaFixes
{
    public class VanillaCommand
    {

        public string Name { get; }
        public List<PLAYER_PERMISSION> Permissions { get; }
        public List<string> Aliases { get; }

        public VanillaCommand(string commandName, List<PLAYER_PERMISSION> permissions, List<string> aliases)
        {
            Name = commandName.ToUpper();
            Permissions = permissions;
            Aliases = aliases;
        }


        public bool CheckForCmdName(string cmd)
        {
            cmd = cmd.ToUpper();

            if (cmd == Name) return true;

            foreach (var alias in Aliases)
            {

                if (alias.ToUpper() == cmd) return true;
            }

            return false;
        }
    }
}
