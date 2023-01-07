using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class PlayerMuteEvent : Event
    {

        public string UserID { get; }
        public MuteType Type { get; set; }
        public bool Allow { get; set; }
        public bool IsUnmute { get; }

        public PlayerMuteEvent(string UserID, MuteType Type, bool IsUnmute, bool Allow)
        {

            this.UserID = UserID;
            this.Type = Type;
            this.Allow = Allow;
            this.IsUnmute = IsUnmute;
        }

        public override void ExecuteHandler(IEventHandler handler)
        {

            ((IEventHandlerPlayerMute)handler).OnMute(this);
        }
    }
}
