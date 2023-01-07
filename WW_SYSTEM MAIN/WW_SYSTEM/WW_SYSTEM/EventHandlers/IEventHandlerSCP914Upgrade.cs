using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.EventHandlers
{
	public interface IEventHandlerSCP914Upgrade : IEventHandler
	{

		void OnSCP914Upgrade(SCP914UpgradeEvent ev);
	}
}
