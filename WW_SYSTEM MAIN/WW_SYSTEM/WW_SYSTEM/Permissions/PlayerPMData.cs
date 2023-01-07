using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Permissions
{
    [Serializable]
    public class PlayerPMData
    {
        public string UserID = "";
        public string Prefix = "";
        public string Color = "";
        public bool RemoteAdmin = false;
        public byte KickPower;
        public byte RequiredKickPower;
        public List<string> Groups = new List<string>();
        public List<string> Perms = new List<string>();

        

        public PlayerPMData(string userID, string prefix, string color, byte kickPower, byte requiredKickPower, bool remoteAdmin, List<string> groups, List<string> perms)
        {
            KickPower = kickPower;
            RequiredKickPower = requiredKickPower;
            UserID = userID;
            Prefix = prefix;
            Color = color;
            Groups = groups;
            Perms = perms;
            RemoteAdmin = remoteAdmin;
        }

    }
}
