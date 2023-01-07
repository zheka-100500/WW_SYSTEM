using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{

   public class Player079CameraTeleportEvent : PlayerEvent
	{
	
		public Vector Camera { get; set; }


		public bool Allow { get; set; }




		public Player079CameraTeleportEvent(Player player, Vector camera, bool allow) : base(player)
		{
			this.Camera = camera;
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079CameraTeleport)handler).On079CameraTeleport(this);
		}
	}
}
