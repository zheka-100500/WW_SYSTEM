using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class Player079LevelUpEvent : PlayerEvent
	{
	
		public Player079LevelUpEvent(Player player, int LevelToUP) : base(player)
		{
			this.LevelToUP = LevelToUP;
		}

		public int LevelToUP { get; }
	
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079LevelUp)handler).On079LevelUp(this);
		}
	}
}
