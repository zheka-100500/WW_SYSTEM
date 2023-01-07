using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items;
using WW_SYSTEM.Custom_Items.Items;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerRequestReceived))]
	public class PlayerReloadEventPatch
	{

			public static bool Prefix(NetworkConnection conn, RequestMessage msg)
			{

			Player pl;
			if (!Round.TryGetPlayer(conn.identity.gameObject, out pl))
			{
				return true;
			}
			var cur = pl.inv.CurItem;
			if (msg.Serial != cur.SerialNumber)
			{
				return true;
			}
		
			if(msg.Request == RequestType.Reload)
			{
				PlayerReloadEvent ev = new PlayerReloadEvent(pl, cur);
				EventManager.Manager.HandleEvent<IEventHandlerReload>(ev);

				if (!ev.Allow) return false;

				if(CustomItemManager.TryGetItem(cur.SerialNumber, CustomItemType.WEAPON, out var item))
				{
					var weapon = item as CustomWeapon;

					if (!weapon.Reload(pl, cur)) return false;


				}
			}





			return true;
			}
		
	}

}
