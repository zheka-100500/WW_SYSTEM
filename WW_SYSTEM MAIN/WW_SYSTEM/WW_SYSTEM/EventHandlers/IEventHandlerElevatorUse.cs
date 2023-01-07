using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.EventHandlers
{
	public interface IEventHandlerElevatorUse : IEventHandler
	{
	
		void OnElevatorUse(PlayerElevatorUseEvent ev);
	}
}
