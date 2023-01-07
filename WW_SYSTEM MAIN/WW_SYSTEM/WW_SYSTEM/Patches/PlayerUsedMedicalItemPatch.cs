using HarmonyLib;
using InventorySystem.Items.Usables;
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
    [HarmonyPatch(typeof(Scp500), nameof(Scp500.OnEffectsActivated))]
    public class PlayerUsedMedicalItemPatch
    {
        public static bool Prefix(Scp500 __instance)
        {
            try
            {

                if (CustomItemManager.TryGetItem(__instance.ItemSerial, CustomItemType.MEDICAL, out var item))
                {
                    CustomMedical Medical;
                    if ((Medical = item as CustomMedical) != null)
                    {
                        if (!Medical.UseMedical(Round.GetPlayer(__instance.Owner))) return false;
                    }
                }

                if(Round.TryGetPlayer(__instance.Owner, out var pl))
                {
                    PlayerUsedMedicalItemEvent ev = new PlayerUsedMedicalItemEvent(pl, ItemType.SCP500);
                    EventManager.Manager.HandleEvent<IEventHandlerUsedMedicalItem>(ev);
                      
                }
             

                return true;
            }
            catch (Exception)
            {

                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Painkillers), nameof(Painkillers.OnEffectsActivated))]
    public class PlayerUsedMedicalItemPatch1
    {
        public static bool Prefix(Painkillers __instance)
        {
            try
            {

                if (CustomItemManager.TryGetItem(__instance.ItemSerial, CustomItemType.MEDICAL, out var item))
                {
                    CustomMedical Medical;
                    if ((Medical = item as CustomMedical) != null)
                    {
                        if (!Medical.UseMedical(Round.GetPlayer(__instance.Owner))) return false;
                    }
                }
                if (Round.TryGetPlayer(__instance.Owner, out var pl))
                {
                    PlayerUsedMedicalItemEvent ev = new PlayerUsedMedicalItemEvent(pl, ItemType.Painkillers);
                    EventManager.Manager.HandleEvent<IEventHandlerUsedMedicalItem>(ev);

                }
                return true;
            }
            catch (Exception)
            {

                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Medkit), nameof(Medkit.OnEffectsActivated))]
    public class PlayerUsedMedicalItemPatch2
    {
        public static bool Prefix(Medkit __instance)
        {
            try
            {

                if (CustomItemManager.TryGetItem(__instance.ItemSerial, CustomItemType.MEDICAL, out var item))
                {
                    CustomMedical Medical;
                    if ((Medical = item as CustomMedical) != null)
                    {
                        if (!Medical.UseMedical(Round.GetPlayer(__instance.Owner))) return false;
                    }
                }
                if (Round.TryGetPlayer(__instance.Owner, out var pl))
                {
                    PlayerUsedMedicalItemEvent ev = new PlayerUsedMedicalItemEvent(pl, ItemType.Medkit);
                    EventManager.Manager.HandleEvent<IEventHandlerUsedMedicalItem>(ev);

                }
                return true;
            }
            catch (Exception)
            {

                return true;
            }
        }
    }



    [HarmonyPatch(typeof(Adrenaline), nameof(Adrenaline.OnEffectsActivated))]
    public class PlayerUsedMedicalItemPatch3
    {
        public static bool Prefix(Adrenaline __instance)
        {
            try
            {

                if (CustomItemManager.TryGetItem(__instance.ItemSerial, CustomItemType.MEDICAL, out var item))
                {
                    CustomMedical Medical;
                    if ((Medical = item as CustomMedical) != null)
                    {
                        if (!Medical.UseMedical(Round.GetPlayer(__instance.Owner))) return false;
                    }
                }
                if (Round.TryGetPlayer(__instance.Owner, out var pl))
                {
                    PlayerUsedMedicalItemEvent ev = new PlayerUsedMedicalItemEvent(pl, ItemType.Adrenaline);
                    EventManager.Manager.HandleEvent<IEventHandlerUsedMedicalItem>(ev);

                }
                return true;
            }
            catch (Exception)
            {

                return true;
            }
        }
    }
}
