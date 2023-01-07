using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Level_System;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(ServerRoles), "SetGroup")]
	public class ServerRolesPatch
	{

		static void Postfix(ServerRoles __instance)
		{
			try
			{
				if (__instance.gameObject.TryGetComponent<Player>(out var pl))
				{
			
					if (Round.UseLevelSYSTEM)
					{
					
						if (LevelManager.Levels.ContainsKey(pl.UserId))
						{
							LevelManager.Levels[pl.UserId].OriginalPrefix = __instance.Network_myText;
							LevelManager.Levels[pl.UserId].OriginalColor = __instance.Network_myColor;
							LevelManager.SavePlayer(pl.UserId);
						}
					}
				}
				return;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("SetServerNameEvent error: {0}", arg));
			}
		}
	}

	[HarmonyPatch(typeof(ServerRoles), "TargetOpenRemoteAdmin")]
	public class ServerRolesPatch1
	{

		static void Postfix(ServerRoles __instance)
		{
			try
			{
				if (__instance.gameObject.TryGetComponent<Player>(out var pl))
				{

			
					pl.SendALLCommandsToRA();
				}
				return;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("ServerRolesPatch1 error: {0}", arg));
			}
		}
	}
}
