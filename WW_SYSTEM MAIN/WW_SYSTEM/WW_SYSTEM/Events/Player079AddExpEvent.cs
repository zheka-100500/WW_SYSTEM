using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class Player079AddExpEvent : PlayerEvent
	{
		
		public ExperienceType ExperienceType { get; }

	
		public float ExpToAdd { get; set; }

	
		public Player079AddExpEvent(Player player, ExperienceType experienceType, float expToAdd) : base(player)
		{
			this.ExperienceType = experienceType;
			this.ExpToAdd = expToAdd;
		}

		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler079AddExp)handler).On079AddExp(this);
		}
	}
}
