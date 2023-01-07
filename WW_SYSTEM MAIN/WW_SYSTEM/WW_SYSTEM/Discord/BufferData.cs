using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord
{
    public class BufferData
    {

        public string Url { get; set; }
        public Webhook Webhook { get; set; }

        public BufferData(string Url, Webhook Webhook)
        {
            this.Url = Url;
            this.Webhook = Webhook;
        }

    }
}
