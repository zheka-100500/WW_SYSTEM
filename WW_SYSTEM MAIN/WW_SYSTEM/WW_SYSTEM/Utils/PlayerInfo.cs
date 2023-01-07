using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Utils
{
    [Serializable]
    public class PlayerInfo
    {
        public string Pass;
        public string UserId;
        public string NickName;
        public PlayerInfo(string pass, string userId, string nickName)
        {
            Pass = pass;
            UserId = userId;
            NickName = nickName;

        }
    }
}
