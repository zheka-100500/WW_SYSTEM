using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Permissions
{
    [Serializable]
    public class PMData
    {
        public List<PlayerPMData> Admins = new List<PlayerPMData>();
        public List<GroupPMData> Groups = new List<GroupPMData>();
        public PMData(List<PlayerPMData> admins, List<GroupPMData> groups)
        {
            Admins = admins;
            Groups = groups;
        }
    }
}
