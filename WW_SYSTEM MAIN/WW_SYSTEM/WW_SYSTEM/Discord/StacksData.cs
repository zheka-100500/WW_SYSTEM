using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord
{
    class StacksData
    {
        public int Count;
        public Webhook Webhook;
        public StacksData(int Count, Webhook Webhook)
        {
            this.Count = Count;
            this.Webhook = Webhook;
        }
    }
}
