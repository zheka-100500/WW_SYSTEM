using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.EventHandlers
{
	public interface IEventHandlerSCP914ChangeKnob : IEventHandler
	{
		
		void OnSCP914ChangeKnob(PlayerSCP914ChangeKnobEvent ev);
	}
}
