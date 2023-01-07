using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerIntercomEvent : PlayerEvent
	{
		



		public PlayerIntercomEvent(Player player) : base(player)
		{
		
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerIntercom)handler).OnIntercom(this);
		}
	}
}
