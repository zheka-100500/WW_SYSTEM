using System;
using System.IO;
using GameCore;
using MEC;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Events;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using WW_SYSTEM.Discord;
using WW_SYSTEM.AntiCheat;
using WW_SYSTEM.Translation;
using WW_SYSTEM.Utils;

namespace WW_SYSTEM
{

	public class MainLoader
	{
		
		public static void EntryPointForLoader()
		{
			Logger.Info("WW SYSTEM", "PATHING SCP....");
			try
			{
				instance = new Harmony("WW_SYSTEM.Patches");
				instance.PatchAll();
			}
			catch (Exception ex)
			{
				Logger.Error("WW SYSTEM", "PATHING ERROR: " + ex);

				return;
			}
			Logger.Info("WW SYSTEM", "PATHING DONE!");


			Logger.Info("WW SYSTEM", "Initialising ROUND DATA...");
			Round.Init();
			Logger.Info("WW SYSTEM", "ROUND DATA STARTED");
			AntiCheat.AntiCheat.Init();
			Security.ConnectionSecurity.Init();
			Logger.Info("WW SYSTEM","Loading plugins");
			string PluginsPatch = Path.Combine(appData, "Plugins");
			if (!Directory.Exists(PluginsPatch))
			{
				Directory.CreateDirectory(PluginsPatch);
			}
			
			CustomNetworkManager.Modded = true;
			VanillaFixes.VanillaCommands.LoadCmds();
			LoadPlugins();

		}
		public static void LoadPlugins()
		{
			

			try
			{

				
				string PluginsPatch = Path.Combine(appData, "Plugins");
				manager = new PluginManager();
			
				commandShell = new CommandShell();
		
				Logger.Info("WW SYSTEM INIT", "PLUGINS PATH: " + PluginsPatch);
				manager.LoadPlugins(PluginsPatch);
				manager.EnablePlugins(commandShell);
				WebhookManager.StartBufffer();
			}
			catch (Exception ex)
			{

				Logger.Error("WW SYSTEM INIT", ex.Message);
				Logger.Error("WW SYSTEM INIT", ex.StackTrace);
			}

			LogsArchiver.CheckLogs();
		}

		public static PluginManager manager;
		private static string appData = FileManager.GetAppFolder(true, true, "");
		private static Harmony instance;
		public static CommandShell commandShell;
	}
}
