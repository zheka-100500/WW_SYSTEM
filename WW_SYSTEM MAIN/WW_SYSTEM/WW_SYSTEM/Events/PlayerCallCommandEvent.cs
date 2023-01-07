using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerCallCommandEvent : PlayerEvent
	{
	
		public string ReturnMessage { get; set; }

		public string Color { get; set; }


		public string Command { get; }

	
		public PlayerCallCommandEvent(Player player, string command, string returnMessage, string color) : base(player)
		{
			this.ReturnMessage = returnMessage;
			this.Color = color;
			this.Command = command;

		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerCallCommand)handler).OnCallCommand(this);
		}
	}
}
