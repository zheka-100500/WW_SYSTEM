using HarmonyLib;
using Hints;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Translation;

namespace WW_SYSTEM.Patches
{
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    public class PlayerPickupItemEventPatch
    {
        public static bool Prefix(ItemSearchCompletor __instance)
        {

            try
            {
				var item = __instance.TargetPickup.Info.ItemId;
				if (Round.TryGetPlayer(__instance.Hub, out var pl))
				{

					PlayerPickupItemEvent ev = new PlayerPickupItemEvent(pl, __instance.TargetPickup.Info.ItemId, __instance.TargetPickup, __instance._category, true, true);
					EventManager.Manager.HandleEvent<IEventHandlerPlayerPickupItem>(ev);

					item = ev.Item;

					if (!ev.Allow)
					{
						if(ev.ShowDenyPickupMsg)
						pl.ShowMessage(Translator.GetTranslationForPlugin("WW_SYSTEM").GetTranslation("EV_PICKUP_DENIED"), 3);


						__instance.TargetPickup.Info.InUse = false;
						__instance.TargetPickup.NetworkInfo = __instance.TargetPickup.Info;

						return false;
					}

					if (CustomItemManager.TryGetItem(__instance.TargetPickup.Info.Serial, out var Customitem))
					{
						if (Customitem.ShowPickupMsg)
						{
							ev.Player.ShowMessage(CustomItemManager.PickupMessage.Replace("{item}", Customitem.Name), CustomItemManager.MsgDur);
						}
					}
				}


                __instance.Hub.inventory.ServerAddItem(__instance.TargetPickup.Info.ItemId, __instance.TargetPickup.Info.Serial, __instance.TargetPickup);
                __instance.TargetPickup.DestroySelf();
                __instance.CheckCategoryLimitHint();

                return false;
			}
            catch (Exception ex)
            {
				Logger.Error("PlayerPickupItemEventPatch", ex.ToString());
				return true;
			}
			


		}
    }
}
