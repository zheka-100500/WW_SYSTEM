using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Events
{
	public abstract class PlayerItemEvent : PlayerEvent
	{
		
		public PlayerItemEvent(Player player, ItemType change, bool allow) : base(player)
		{
		
			this.Allow = allow;
			this.ChangeTo = change;
		}





		public ItemType ChangeTo { get; set; }


		public bool Allow { get; set; }
	}
}
