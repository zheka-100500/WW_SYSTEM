using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Events
{
	public class PlayerSpawnEvent : PlayerEvent
	{
		public Vector SpawnPos { get; set; }
		public float Rot { get; set; }

		public PlayerSpawnEvent(Player player, Vector SpawnPos, float rot) : base(player)
		{
			this.Rot = rot;
			this.SpawnPos = SpawnPos;
		}

		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerSpawn)handler).OnSpawn(this);
		}
	}
}
