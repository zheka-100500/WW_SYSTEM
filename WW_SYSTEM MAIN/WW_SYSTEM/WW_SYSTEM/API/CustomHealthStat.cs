using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public class CustomHealthStat : HealthStat
    {

        public float MaxHP = 0f;

        public override float MaxValue => MaxHP;

        public override void ClassChanged()
        {
            MaxHP = Round.GetMaxHp(Hub.roleManager.CurrentRole.RoleTypeId);
            base.ClassChanged();
        }


    }
}
