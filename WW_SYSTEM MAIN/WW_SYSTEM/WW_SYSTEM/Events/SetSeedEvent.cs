using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class SetSeedEvent : Event
	{
		
		public int Seed { get; set; }

		public SetSeedEvent(int seed)
		{
			this.Seed = seed;
		}


		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSetSeed)handler).OnSetSeed(this);
		}
	}
}
