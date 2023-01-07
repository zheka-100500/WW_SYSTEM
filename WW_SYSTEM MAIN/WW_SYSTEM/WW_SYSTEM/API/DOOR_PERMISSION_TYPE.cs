using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public enum DOOR_PERMISSION_TYPE
    {
        NONE = -1,
        SCP_OVERRIDE,
        CHCKPOINT_ACC,
        INCOM_ACC,
        CONT_LVL_1,
        CONT_LVL_2,
        CONT_LVL_3,
        ARMORY_LVL_1,
        ARMORY_LVL_2,
        ARMORY_LVL_3,
        WARHEAD,
        GATES

    }
}
