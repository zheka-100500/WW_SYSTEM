﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.EventHandlers
{
	public interface IEventHandlerScp096Enrage : IEventHandler
	{
		
		void OnScp096Enrage(Scp096EnrageEvent ev);
	}
}