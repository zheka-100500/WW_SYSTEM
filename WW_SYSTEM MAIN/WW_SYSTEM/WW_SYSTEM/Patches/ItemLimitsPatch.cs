using HarmonyLib;
using Hints;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Events;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;

namespace WW_SYSTEM.Patches
{
	public class ItemLimitsPatch
	{

		
		public class ReceiveRequestPatch
		{

		}

		

		public static void ShowWarning(ReferenceHub hub, byte AmmoType, ulong MaxAmmo)
        {
			hub.hints.Show(new TranslationHint(HintTranslations.MaxAmmoAlreadyReached, new HintParameter[]
				{
					new AmmoHintParameter(AmmoType),
					new PackedULongHintParameter(MaxAmmo)
				}, new HintEffect[]
				{
					HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2)
				}, 2f));
		}

		public static void ShowWarning(ReferenceHub hub, ItemCategory item, byte MaxItems)
		{
			hub.hints.Show(new TranslationHint(HintTranslations.MaxItemCategoryReached, new HintParameter[]
				{
					new ItemCategoryHintParameter(item),
					new ByteHintParameter(MaxItems)
				}, HintEffectPresets.FadeInAndOut(0.25f, 1f, 0f), 1.5f));
		}




	}


}
