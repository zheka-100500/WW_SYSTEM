using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Level_System
{
    [Serializable]
    public class PlayerLevel
    {
        public int CurExp = 0;
        public int MaxExp = 0;
        public int CurLvl = 0;

        [NonSerialized]
        public string OriginalPrefix = "";
        [NonSerialized]
        public string OriginalColor = "";

        public PlayerLevel(int CurExp, int MaxExp, int CurLvl)
        {
            this.CurExp = CurExp;
            this.MaxExp = MaxExp;
            this.CurLvl = CurLvl;
        }
    }
}
