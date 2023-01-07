using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Custom_Items.Items
{
    public class CustomItem
    {
        public string Name = "";

        public CustomItemType CustomItemType = CustomItemType.NONE;
        public ItemType ItemType = ItemType.None;

        private int _id = -1;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == -1) _id = value;
            }
        }


        public bool ShowPickupMsg = true;

        public virtual void Load()
        {

        }

        public virtual void UnLoad()
        {

        }

    }
        

    
}
