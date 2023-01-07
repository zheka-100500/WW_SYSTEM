using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
    public class PlayerLevelUpEvent : Event
    {
		public bool Allow { get; set; }
		 
		public int Level { get; set; }

		public int MaxXp { get; set; }

		public Player Player { get; }


		public PlayerLevelUpEvent(bool Allow, int Level, int MaxXp, Player Player)
		{
			this.Allow = Allow;
			this.Level = Level;
			this.Player = Player;
			this.MaxXp = MaxXp;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerLevelUp)handler).OnPlayerLevelUp(this);
		}
	}
}
