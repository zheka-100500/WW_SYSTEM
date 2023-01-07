using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using PlayerRoles;

namespace WW_SYSTEM.Events
{
	public class PlayerCheckEscapeEvent : PlayerEvent
	{
	
		public bool AllowEscape { get; set; }
		public RoleTypeId Role { get; set; }

	

		
		public PlayerCheckEscapeEvent(Player player, RoleTypeId role, bool allow) : base(player)
		{
			this.AllowEscape = allow;
            Role = role;

        }

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerCheckEscape)handler).OnCheckEscape(this);
		}
	}
}
