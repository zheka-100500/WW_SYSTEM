using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Discord.BOT;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class BotRequestEvent : Event
    {
		public string Cmds { get; set; }
		public string Output { get; set; }
		public DiscordUser User { get; }
		public bool SendOutput { get; set; }
		public ulong Channel { get; set; }

		public BotRequestEvent(string Cmds, DiscordUser User, string Output, bool SendOutput, ulong Channel)
		{
			this.Cmds = Cmds;
			this.Output = Output;
			this.User = User;
			this.SendOutput = SendOutput;
			this.Channel = Channel;

		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerBotRequest)handler).OnBotRequest(this);
		}
	}
}
