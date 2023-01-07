using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class Player079UnlockDoorsEvent : PlayerEvent
	{

		public bool Allow { get; set; }

		
		public Player079UnlockDoorsEvent(Player player, bool allow) : base(player)
		{
			this.Allow = allow;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079UnlockDoors)handler).On079UnlockDoors(this);
		}
	}
}
