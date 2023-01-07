using HarmonyLib;
using InventorySystem;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(InventoryExtensions), "ServerDropEverything")]
	public class PlayerDropAllItemsEventPatch
	{

		public static bool Prefix(Inventory inv)
		{

			try
			{
				PlayerDropAllItemsEvent ev = new PlayerDropAllItemsEvent(inv._hub.characterClassManager.GetComponent<Player>(), true);
				EventManager.Manager.HandleEvent<IEventHandlerPlayerDropAllItems>(ev);
				return ev.Allow;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerDropAllItemsEvent error: {0}", arg));
			}
			return true;
		}
	}

}
