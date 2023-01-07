using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using UnityEngine; 
using Mirror;
using CustomPlayerEffects;
using WW_SYSTEM.Discord;
using MEC;
using InventorySystem;
using InventorySystem.Items.Armor;
using CommandSystem.Commands.RemoteAdmin;
using CommandSystem;
using GameCore;
using Utils;
using InventorySystem.Items.Radio;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using VoiceChat;

namespace WW_SYSTEM.Patches
{
	public class MiscPatches
	{

		

		[HarmonyPatch(typeof(ConfigFile), nameof(ConfigFile.ReloadGameConfigs))]
		public class ConfigsPatch
		{
			public static void Postfix(bool firstTime)
			{
				

				Round.RefreshConfig();

		
			}
		}

		[HarmonyPatch(typeof(PlayerList), nameof(PlayerList.RefreshTitleSafe))]
		public class ServerNamePatch
		{
			public static bool Prefix()
			{
				var name = string.Empty;
				if(string.IsNullOrEmpty(PlayerList.Title.Value))
				{
					name = ServerConsole.singleton.RefreshServerNameSafe();
					
				}else
				{
					if (!ServerConsole.singleton.NameFormatter.TryProcessExpression(PlayerList.Title.Value, "player list title", out name))
					{
						ServerConsole.AddLog(name, ConsoleColor.Gray);
						return true;
					}
				}
			
				
				
				EventGetListName ev = new EventGetListName(name);
				EventManager.Manager.HandleEvent<IEventHandlerPlayerListGetName>(ev);
				PlayerList.ServerName = ev.Name;
				return false;



			}
		}

		[HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
		public class RadioPatch
		{
			public static bool Prefix()
			{

				if (Round.unlimited_radio_battery) return false;

				return true;
			}
		}

		[HarmonyPatch(typeof(DisruptorAction), nameof(DisruptorAction.ServerAuthorizeShot))]
		public class DisruptorPatch
		{
			public static bool Prefix(DisruptorAction __instance)
			{
				if (Round.TryGetPlayer(__instance._firearm.Owner, out var player))
				{
					var AmmoCount = player.GetAmmo(AmmoType.Ammo12gauge);
					var weaponCount = __instance._firearm.Status.Ammo;
					var takeCount = (5 - weaponCount) * 2;

					if (player.InfinityDisruptor)
					{
						__instance._firearm.Status = new FirearmStatus(6, __instance._firearm.Status.Flags, __instance._firearm.Status.Attachments);
					}
					else if (AmmoCount > 0)
					{
						if(AmmoCount >= 5 - takeCount)
						{
							player.SetAmmo(AmmoType.Ammo12gauge, (int)(AmmoCount - takeCount));
							__instance._firearm.Status = new FirearmStatus(5, __instance._firearm.Status.Flags, __instance._firearm.Status.Attachments);
						}
						else
						{
							player.SetAmmo(AmmoType.Ammo12gauge, 0);
							__instance._firearm.Status = new FirearmStatus((byte)(weaponCount + AmmoCount), __instance._firearm.Status.Flags, __instance._firearm.Status.Attachments);
						}
			
					}
					
					
				}

				return true;
			}
		}





		[HarmonyPatch(typeof(Corroding), nameof(Corroding.OnTick))]
		public class CorrodingPatch
		{
			public static bool Prefix(Corroding __instance)
			{

				var type = typeof(Corroding);
				if(Round.PlayerEffectsDamage.TryGetValue(type, out var value))
				{

					__instance._damagePerTick = value;

				}
				return true;


			}
		}




		
		[HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.TargetUpdateCommandList))]
		public class CommandSyncPatch
		{
			public static void Prefix(ref QueryProcessor.CommandData[] commands, QueryProcessor __instance)
			{
		 

				List<QueryProcessor.CommandData> NewCommands = commands.ToList();

				foreach (var item in MainLoader.commandShell.GetCommandsForRa(__instance))
				{
					NewCommands.Add(item);
				}

			
				commands = NewCommands.ToArray();

			
			}
		}

		[HarmonyPatch(typeof(BodyArmorUtils), nameof(BodyArmorUtils.RemoveEverythingExceedingLimits))]
		public class AmmoDropPatch
		{
			public static void Prefix(Inventory inv, BodyArmor armor, ref bool removeItems,ref bool removeAmmo)
			{

				if (inv.TryGetComponent<Player>(out var pl))
				{
					if (pl.IgnoreAmmoLimits) removeAmmo = false;
					if (pl.IgnoreItemsLimits) removeItems = false;
				
				}


			}
		}


		[HarmonyPatch(typeof(Misc), nameof(Misc.CheckPermission), new Type[] { typeof(ICommandSender), typeof(PlayerPermissions[]) })]
		public class CheckPermsPatch
		{
			public static bool Prefix(ICommandSender sender, PlayerPermissions[] perms, ref bool __result)
			{

				CommandSender commandSender;
				var result = (commandSender = (sender as CommandSender)) != null && PermissionsHandler.IsPermitted(commandSender.Permissions, perms);

				PlayerCommandSender playerCommandSender;
				if (!result && (playerCommandSender = (sender as PlayerCommandSender)) != null && playerCommandSender.ReferenceHub.TryGetPlayer(out var pl))
				{
					var AllPerms = new List<PLAYER_PERMISSION>();

					foreach (var item in perms)
					{
						AllPerms.Add((PLAYER_PERMISSION)(int)item);


					}

					result = pl.IsPermitted(AllPerms);
				}

				__result = result;
				return false;
			}
		}

		[HarmonyPatch(typeof(Misc), nameof(Misc.CheckPermission), new Type[] { typeof(ICommandSender), typeof(PlayerPermissions) })]
		public class CheckPermsPatch1
		{
			public static bool Prefix(ICommandSender sender, PlayerPermissions perm, ref bool __result)
			{
				CommandSender commandSender;
				var result = (commandSender = (sender as CommandSender)) != null && (commandSender.FullPermissions || PermissionsHandler.IsPermitted(commandSender.Permissions, perm));

				PlayerCommandSender playerCommandSender;
				if (!result && (playerCommandSender = (sender as PlayerCommandSender)) != null && playerCommandSender.ReferenceHub.TryGetPlayer(out var pl))
				{
					

					result = pl.IsPermitted((PLAYER_PERMISSION)(int)perm);
				}

				__result = result;
				return false;


			}
		}







		[HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.LoadMutes))]
		internal static class MuteHandlerClear
		{
			private static void Prefix() => VoiceChatMutes.Mutes?.Clear();
		}

		[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Awake))]
		internal static class AddCustomStats
		{
			private static void Prefix(PlayerStats __instance) 
			{
				__instance.StatModules[0] = new CustomHealthStat();
			}
		}

		[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Awake))]
		internal static class StatsFix
		{
			private static void Postfix(PlayerStats __instance)
			{
				var type = typeof(HealthStat);
				__instance._dictionarizedTypes.Add(type, __instance.StatModules[0]);
			}
		}



		




	}
}
