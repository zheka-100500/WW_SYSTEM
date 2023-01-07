using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Discord.BOT
{
    public enum DiscordStatus
    {
        //
        // Summary:
        //     User is offline.
        Offline = 0,
        //
        // Summary:
        //     User is online.
        Online = 1,
        //
        // Summary:
        //     User is idle.
        Idle = 2,
        //
        // Summary:
        //     User asked not to be disturbed.
        DoNotDisturb = 4,
        //
        // Summary:
        //     User is invisible. They will appear as Offline to anyone but themselves.
        Invisible = 5
    }
}
