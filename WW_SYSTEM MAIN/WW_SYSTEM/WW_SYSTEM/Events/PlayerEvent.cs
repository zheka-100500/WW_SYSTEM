using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Events
{
	public abstract class PlayerEvent : Event
	{
	
		public PlayerEvent(Player player)
		{
			this.player = player;
		}


		public Player Player
		{
			get
			{
				return this.player;
			}
		}

		
		private Player player;
	}
}
