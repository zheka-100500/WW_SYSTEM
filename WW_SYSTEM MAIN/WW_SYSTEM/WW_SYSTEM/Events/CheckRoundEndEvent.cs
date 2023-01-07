using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class CheckRoundEndEvent : Event
	{
	
		public CheckRoundEndEvent(ROUND_END_STATUS status, int MTF_AND_SCIENCE, int CHAOS_AND_CLASSD, int SCPS, bool EndRound) : base()
		{
			
			this.MTF_AND_SCIENCE = MTF_AND_SCIENCE;
			this.CHAOS_AND_CLASSD = CHAOS_AND_CLASSD;
			this.SCPS = SCPS;
			this.EndRound = EndRound;
			Status = status;
		}

		public int MTF_AND_SCIENCE { get; }
		public int CHAOS_AND_CLASSD { get; }
		public int SCPS { get; }
		public bool EndRound;



	
		public ROUND_END_STATUS Status { get; set; }


		public override void ExecuteHandler(IEventHandler handler)
		{
			
			((IEventHandlerCheckRoundEnd)handler).OnCheckRoundEnd(this);
		}



	}
}
