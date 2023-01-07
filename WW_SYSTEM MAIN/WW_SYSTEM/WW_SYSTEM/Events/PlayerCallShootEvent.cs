using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerCallShootEvent : PlayerEvent
	{


		public bool Allow { get; set; }





		public PlayerCallShootEvent(Player Player, bool Allow) : base(Player)
		{
			
			this.Allow = Allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerCallShoot)handler).OnCallShoot(this);
		}
	}
}
