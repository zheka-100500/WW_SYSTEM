
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Translation;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
	public class WaitingForPlayersPatch
	{

		public static void Prefix(string q, ConsoleColor color)
		{
			string text = "Waiting for players...";
			 if (q.ToUpper() == text.ToUpper())
			{


				WaitingForPlayersEvent ev = new WaitingForPlayersEvent();
				EventManager.Manager.HandleEvent<IEventHandlerWaitingForPlayers>(ev);

				var ccm = ReferenceHub.LocalHub.characterClassManager;
					if (ccm.GetComponent<Player>() == null)
					{
					ccm.gameObject.AddComponent<Player>();
					ccm.gameObject.GetComponent<Player>().LoadComponents();
					}
					Round.WarheadLocked = false;
				IdleMode.PauseIdleMode = true;
				Translator.ReloadAllTranslations();
				WW_SYSTEM.Permissions.PermissionsManager.Reload();
				WW_SYSTEM.UpdateSystem.WW_UPDATE.CheckUpdate();
				try
				{
					Custom_Map.MapEditor.DestroyAllSpawnedObjects();
				}
				catch (Exception)
				{

				}
			


				Round.ReloadMaxHps();

				IdleMode.PauseIdleMode = false;
			}

		}
	}
}
