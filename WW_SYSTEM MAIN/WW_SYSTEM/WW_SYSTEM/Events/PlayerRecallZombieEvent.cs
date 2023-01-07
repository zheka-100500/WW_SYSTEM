using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public class PlayerRecallZombieEvent : PlayerEvent
	{
		


		public Player SCP049 { get; }


		public PlayerRecallZombieEvent(Player player, Player SCP049) : base(player)
		{
			this.SCP049 = SCP049;
		}

	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerRecallZombie)handler).OnRecallZombie(this);
		}
	}
}
