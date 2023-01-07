using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Permissions
{
    [Serializable]
    public class GroupPMData
    {

        public string GroupPrefix = "";
        public string GroupColor = "";
        public string GroupName = "";
        public byte KickPower;
        public byte RequiredKickPower;
        public List<string> GroupPerms = new List<string>();

        public GroupPMData(string groupName, string groupPrefix, string groupColor, byte kickPower, byte requiredKickPower, List<string> groupPerms)
        {
            KickPower = kickPower;
            RequiredKickPower = requiredKickPower;
            GroupPrefix = groupPrefix;
            GroupColor = groupColor;
            GroupName = groupName;
            GroupPerms = groupPerms;
        }


    }
}
