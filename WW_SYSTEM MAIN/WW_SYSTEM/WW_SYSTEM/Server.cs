using Mirror.LiteNetLib4Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameCore;
using WW_SYSTEM.API;
using WW_SYSTEM.Events;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Utils;

namespace WW_SYSTEM
{
	public static class Server
	{

		public static Player LocalPlayer { get; internal set; }

		public static List<string> IgnoreTokenUserIds = new List<string>();

		public static Dictionary<string, PlayerInfo> IgnoreTokenIps = new Dictionary<string, PlayerInfo>();

		public static PLAYER_PERMISSION OverwatchSeePerm = PLAYER_PERMISSION.NONE;

		public static ushort Port
		{
            get
            {
                if (ServerConsole.PortOverride != 0)
                {
                    return ServerConsole.PortOverride;
                }
                return LiteNetLib4MirrorTransport.Singleton.port;
            }
        }

		public static void CfgReload()
		{
			ConfigFile.ReloadGameConfigs(false);
			ServerStatic.RolesConfig.Reload();
			ServerStatic.SharedGroupsConfig = ((ConfigSharing.Paths[4] == null) ? null : new YamlConfig(ConfigSharing.Paths[4] + "shared_groups.txt"));
			ServerStatic.SharedGroupsMembersConfig = ((ConfigSharing.Paths[5] == null) ? null : new YamlConfig(ConfigSharing.Paths[5] + "shared_groups_members.txt"));
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			ServerConsole.AddLog("#Permission file reloaded.", ConsoleColor.Gray);
			Round.RefreshConfig();
		}
		


		public static string EnterConsoleCommand(string cmds, CommandSender sender = null)
		{
			string[] array = cmds.Split(new char[]
		  {
			' '
		  });
			ServerConsoleCommandEvent serverConsoleCommandEvent = new ServerConsoleCommandEvent();
			serverConsoleCommandEvent.Command = array[0];
			serverConsoleCommandEvent.Args = array;
			serverConsoleCommandEvent.Handled = false;
			EventManager.Manager.HandleEvent<IEventHandlerCallConsoleCommand>(serverConsoleCommandEvent);
			if (serverConsoleCommandEvent.Handled)
			{
				ServerConsole.AddLog(serverConsoleCommandEvent.Output, ConsoleColor.Gray);
				if (sender != null) sender.Print(serverConsoleCommandEvent.Output);
				return serverConsoleCommandEvent.Output;
			}
			return ServerConsole.EnterCommand(cmds, sender);
		}

		public static List<Player> Players = new List<Player>();


	}
}
