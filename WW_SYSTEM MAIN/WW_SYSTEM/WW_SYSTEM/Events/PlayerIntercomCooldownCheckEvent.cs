using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerIntercomCooldownCheckEvent : PlayerEvent
	{
	
		public float CurrentCooldown { get; set; }


		public PlayerIntercomCooldownCheckEvent(Player player, float currCooldownTime) : base(player)
		{
			this.CurrentCooldown = currCooldownTime;
		}

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerIntercomCooldownCheck)handler).OnIntercomCooldownCheck(this);
		}
	}
}
