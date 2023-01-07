using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Custom_Items.Items
{
    public class CustomMedical : CustomItem
    {

        public virtual bool UseMedical(Player player)
        {
            return true;
        }
    }
}
