using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public abstract class WarheadEvent : Event
	{
	
		public WarheadEvent(Player player, float timeLeft)
		{
			this.player = player;
			this.TimeLeft = timeLeft;
			this.Cancel = false;
		}


		public float TimeLeft { get; set; }

		
		public Player Activator
		{
			get
			{
				return this.player;
			}
		}

		public bool Cancel { get; set; }

		
		private Player player;
	}
}
