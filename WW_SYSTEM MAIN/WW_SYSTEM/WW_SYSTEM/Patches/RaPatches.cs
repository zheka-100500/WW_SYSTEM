using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RemoteAdmin;
using RemoteAdmin.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utf8Json.Internal.DoubleConversion;
using Utils;
using VoiceChat;
using WW_SYSTEM.API;
using WW_SYSTEM.Translation;

namespace WW_SYSTEM.Patches
{
	public class RaPatches
	{
		[HarmonyPatch(typeof(RaPlayerList), nameof(RaPlayerList.ReceiveData), new Type[] { typeof(CommandSender), typeof(string) })]
		public class RaPlayerListPatch
		{

			public static bool Prefix(CommandSender sender, string data, RaPlayerList __instance)
			{

				try
				{
                    string[] array = data.Split(new char[]
            {
                ' '
            });
                    if (array.Length != 3)
                    {
                        return false;
                    }
                    int num;
                    int num2;
                    if (!int.TryParse(array[0], out num) || !int.TryParse(array[1], out num2))
                    {
                        return false;
                    }
                    if (!Enum.IsDefined(typeof(RaPlayerList.PlayerSorting), num2))
                    {
                        return false;
                    }
                    bool flag = num == 1;
                    bool flag2 = array[2].Equals("1");
                    RaPlayerList.PlayerSorting sortingType = (RaPlayerList.PlayerSorting)num2;
                    bool viewHiddenBadges = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenBadges);
                    bool viewHiddenGlobalBadges = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenGlobalBadges);
                    PlayerCommandSender playerCommandSender;
                    if ((playerCommandSender = (sender as PlayerCommandSender)) != null && playerCommandSender.ServerRoles.Staff)
                    {
                        viewHiddenBadges = true;
                        viewHiddenGlobalBadges = true;
                    }
                    var SenderPl = Round.GetPlayer(playerCommandSender.ReferenceHub);
					if(SenderPl == null)
					{
						return true;
					}
					System.Text.StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("\n");
                    foreach (Player pl in Round.GetPlayers())
					{
						var referenceHub = pl.Hub;
						if (referenceHub.Mode != ClientInstanceMode.DedicatedServer && referenceHub.Mode != ClientInstanceMode.Unverified)
						{

							if (pl.Overwatch && !SenderPl.IsPermitted(Server.OverwatchSeePerm)) continue;

							bool flag3 = false;
                            stringBuilder.Append(__instance.GetPrefix(referenceHub, viewHiddenBadges, viewHiddenGlobalBadges));
                            stringBuilder.Append(flag3 && !pl.HideRaRole ? "<link=RA_OverwatchEnabled><color=white>[</color><color=#03f8fc></color><color=white>]</color></link> " : string.Empty);
                            stringBuilder.Append(pl.IsPermitted("WW_SYSTEM_DEVELOPER") ? "<color=white>[</color><color=red>WWS</color><color=white>]</color> " : string.Empty);
                            stringBuilder.Append("<color={RA_ClassColor}>(").Append(referenceHub.PlayerId).Append(") ");
                            stringBuilder.Append(referenceHub.nicknameSync.CombinedName.Replace("\n", string.Empty).Replace("RA_", string.Empty)).Append("</color>");
                            stringBuilder.AppendLine();
                        }
					}
                    sender.RaReply(string.Format("${0} {1}", __instance.DataId, StringBuilderPool.Shared.ToStringReturn(stringBuilder)), true, !flag, string.Empty);
                    return false;
				}
				catch (Exception arg)
				{


					Logger.Error("[EVENT MANAGER]", string.Format("RaPlayerListPatch error: {0}", arg));
					return true;
				}
				
			}
		}



		[HarmonyPatch(typeof(RaPlayer), nameof(RaPlayer.ReceiveData), new Type[] { typeof(CommandSender), typeof(string) })]
		public class RaPlayerPatch
		{

			public static bool Prefix(CommandSender sender, string data, RaPlayer __instance)
			{

				try
				{
					string[] array = data.Split(new char[]
					{
				' '
					});
					if (array.Length != 2)
					{
						return false;
					}
					int num;
					if (!int.TryParse(array[0], out num))
					{
						return false;
					}
					bool flag = num == 1;
					PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
					if (!flag && playerCommandSender != null && !playerCommandSender.ServerRoles.Staff && !CommandProcessor.CheckPermissions(sender, PlayerPermissions.PlayerSensitiveDataAccess))
					{
						return false;
					}
					string[] array2;
					List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(new ArraySegment<string>(array.Skip(1).ToArray<string>()), 0, out array2, false);
					if (list.Count == 0)
					{
						return false;
					}
					bool flag2 = PermissionsHandler.IsPermitted(sender.Permissions, 18007046UL);
					if (playerCommandSender != null && (playerCommandSender.ServerRoles.Staff || playerCommandSender.ServerRoles.RaEverywhere))
					{
						flag2 = true;
					}
					if (list.Count > 1)
					{
						System.Text.StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("<color=white>");
						stringBuilder.Append("Selecting multiple players:");
						stringBuilder.Append("\nPlayer ID: <color=green><link=CP_ID></link></color>");
						stringBuilder.Append("\nIP Address: " + ((!flag) ? "<color=green><link=CP_IP></link></color>" : "[REDACTED]"));
						stringBuilder.Append("\nUser ID: " + (flag2 ? "<color=green><link=CP_USERID></link></color>" : "[REDACTED]"));
						stringBuilder.Append("</color>");
						string text = string.Empty;
						string text2 = string.Empty;
						string text3 = string.Empty;
						foreach (ReferenceHub referenceHub in list)
						{
							text = text + referenceHub.PlayerId + ".";
							if (!flag)
							{
								text2 = text2 + ((referenceHub.networkIdentity.connectionToClient.IpOverride != null) ? referenceHub.networkIdentity.connectionToClient.OriginalIpAddress : referenceHub.networkIdentity.connectionToClient.address) + ",";
							}
							if (flag2)
							{
								text3 = text3 + referenceHub.characterClassManager.UserId + ".";
							}
						}
						if (text.Length > 0)
						{
							RaClipboard.Send(sender, RaClipboard.RaClipBoardType.PlayerId, text);
						}
						if (text2.Length > 0)
						{
							RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, text2);
						}
						if (text3.Length > 0)
						{
							RaClipboard.Send(sender, RaClipboard.RaClipBoardType.UserId, text3);
						}
						sender.RaReply(string.Format("${0} {1}", __instance.DataId, stringBuilder), true, true, string.Empty);
						StringBuilderPool.Shared.Return(stringBuilder);
						return false;
					}
					ServerLogs.AddLog(ServerLogs.Modules.DataAccess, string.Format("{0} accessed IP address of player {1} ({2}).", sender.LogName, list[0].PlayerId, list[0].nicknameSync.MyNick), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					bool flag3 = PermissionsHandler.IsPermitted(sender.Permissions, PlayerPermissions.GameplayData);
					CharacterClassManager characterClassManager = list[0].characterClassManager;
					NicknameSync nicknameSync = list[0].nicknameSync;
					NetworkConnectionToClient connectionToClient = list[0].networkIdentity.connectionToClient;
					ServerRoles serverRoles = list[0].serverRoles;
					PlayerCommandSender playerCommandSender2;
					if ((playerCommandSender2 = (sender as PlayerCommandSender)) != null)
					{
						playerCommandSender2.ReferenceHub.queryProcessor.GameplayData = flag3;
					}
					System.Text.StringBuilder stringBuilder2 = StringBuilderPool.Shared.Rent("<color=white>");
					stringBuilder2.Append("Nickname: " + nicknameSync.CombinedName);
                    stringBuilder2.Append(string.Format("\nPlayer ID: {0} <color=green><link=CP_ID></link></color>", list[0].PlayerId));
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.PlayerId, string.Format("{0}", list[0].PlayerId));
                    if (connectionToClient == null)
					{
						stringBuilder2.Append("\nIP Address: null");
					}
					else if (!flag)
					{
						stringBuilder2.Append("\nIP Address: " + connectionToClient.address + " ");
						if (connectionToClient.IpOverride != null)
						{
							RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, connectionToClient.OriginalIpAddress ?? "");
							stringBuilder2.Append(" [routed via " + connectionToClient.OriginalIpAddress + "]");
						}
						else
						{
							RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, connectionToClient.address ?? "");
						}
						stringBuilder2.Append(" <color=green><link=CP_IP></link></color>");
					}
					else
					{
						stringBuilder2.Append("\nIP Address: [REDACTED]");
					}
					stringBuilder2.Append("\nUser ID: " + (flag2 ? (string.IsNullOrEmpty(characterClassManager.UserId) ? "(none)" : (characterClassManager.UserId + " <color=green><link=CP_USERID></link></color>")) : "<color=#D4AF37>INSUFFICIENT PERMISSIONS</color>"));
					if (flag2)
					{
						RaClipboard.Send(sender, RaClipboard.RaClipBoardType.UserId, characterClassManager.UserId ?? "");
						if (characterClassManager.SaltedUserId != null && characterClassManager.SaltedUserId.Contains("$"))
						{
							stringBuilder2.Append("\nSalted User ID: " + characterClassManager.SaltedUserId);
						}
						if (!string.IsNullOrEmpty(characterClassManager.UserId2))
						{
							stringBuilder2.Append("\nUser ID 2: " + characterClassManager.UserId2);
						}
					}
					stringBuilder2.Append("\nServer role: " + serverRoles.GetColoredRoleString(false));
					bool flag4 = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenBadges);
					bool flag5 = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenGlobalBadges);
					if (playerCommandSender != null && playerCommandSender.ServerRoles.Staff)
					{
						flag4 = true;
						flag5 = true;
					}
					bool flag6 = !string.IsNullOrEmpty(serverRoles.HiddenBadge);
					bool flag7 = !flag6 || (serverRoles.GlobalHidden && flag5) || (!serverRoles.GlobalHidden && flag4);
					if (flag7)
					{
						if (flag6)
						{
							stringBuilder2.Append("\n<color=#DC143C>Hidden role: </color>" + serverRoles.HiddenBadge);
							stringBuilder2.Append("\n<color=#DC143C>Hidden role type: </color>" + (serverRoles.GlobalHidden ? "GLOBAL" : "LOCAL"));
						}
						if (serverRoles.RaEverywhere)
						{
							stringBuilder2.Append("\nStudio Status: <color=#BCC6CC>Studio GLOBAL Staff (management or global moderation)</color>");
						}
						else if (serverRoles.Staff)
						{
							stringBuilder2.Append("\nStudio Status: Studio Staff");
						}
					}
                    int flags = (int)VoiceChatMutes.GetFlags(list[0]);
                    if (flags != 0)
                    {
                        stringBuilder2.Append("\nMUTE STATUS:");
                        foreach (object obj in Enum.GetValues(typeof(VcMuteFlags)))
                        {
                            int num2 = (int)obj;
                            if (num2 != 0 && (flags & num2) == num2)
                            {
                                stringBuilder2.Append(" <color=#F70D1A>");
                                stringBuilder2.Append((VcMuteFlags)num2);
                                stringBuilder2.Append("</color>");
                            }
                        }
                    }
                    stringBuilder2.Append("\nActive flag(s):");
					if(Round.TryGetPlayer(list[0], out var pl))
					{
						if (!pl.HideRaRole)
						{
							if (characterClassManager.GodMode)
							{
								stringBuilder2.Append(" <color=#659EC7>[GOD MODE]</color>");
							}
							if (list[0].playerStats.GetModule<AdminFlagsStat>().HasFlag(AdminFlags.Noclip))
							{
								stringBuilder2.Append(" <color=#DC143C>[NOCLIP ENABLED]</color>");
							}
							else if (FpcNoclip.IsPermitted(list[0]))
							{
								stringBuilder2.Append(" <color=#E52B50>[NOCLIP UNLOCKED]</color>");
							}
                            if (serverRoles.DoNotTrack)
                            {
                                stringBuilder2.Append(" <color=#BFFF00>[DO NOT TRACK]</color>");
                            }
                            if (serverRoles.BypassMode)
							{
								stringBuilder2.Append(" <color=#BFFF00>[BYPASS MODE]</color>");
							}
							if (flag7 && serverRoles.RemoteAdmin)
							{
								stringBuilder2.Append(" <color=#43C6DB>[RA AUTHENTICATED]</color>");
							}
							if (pl.IsPermitted("WW_SYSTEM_DEVELOPER"))
							{
								stringBuilder2.Append(" <color=red>[WW SYSTEM DEVELOPER]</color>");
							}
						
						}

						if (serverRoles.IsInOverwatch)
						{
							stringBuilder2.Append("\nActive flag: <color=#008080>OVERWATCH MODE</color>");
						}
						else if (flag3)
						{
							var stat = list[0].playerStats.GetModule<CustomHealthStat>();
							var name = Translator.MainTranslation.GetTranslation(pl.Role);
                            PlayerRoleBase playerRoleBase;
                            var defaultName = PlayerRoleLoader.AllRoles.TryGetValue(pl.Role, out playerRoleBase) ? playerRoleBase.RoleName : "None";

                            stringBuilder2.Append("\nClass: ");
							stringBuilder2.Append(name.Contains("NO_TRANSLATION") ? defaultName : name);
							stringBuilder2.Append(" <color=#fcff99>[HP: ").Append($"({stat.CurValue}/{stat.MaxValue})").Append("]</color>");
                            stringBuilder2.Append(" <color=green>[AHP: ").Append(CommandProcessor.GetRoundedStat<AhpStat>(pl.Hub)).Append("]</color>");
                            stringBuilder2.Append(" <color=#977dff>[HS: ").Append(CommandProcessor.GetRoundedStat<HumeShieldStat>(pl.Hub)).Append("]</color>");
							if (Round.PositionPerms.Count <= 0 || sender.CheckPermission(Round.PositionPerms.ToVanilla().ToArray()))
							{
                                stringBuilder2.Append("\nPosition: ").Append(pl.Hub.transform.position.ToPreciseString());
                            }
						}
						else
						{
							stringBuilder2.Append("\n<color=#D4AF37>Some fields were hidden. GameplayData permission required.</color>");
						}

					}

                    stringBuilder2.Append("</color>");
                    sender.RaReply(string.Format("${0} {1}", __instance.DataId, StringBuilderPool.Shared.ToStringReturn(stringBuilder2)), true, true, string.Empty);
                    RaPlayerQR.Send(sender, false, string.IsNullOrEmpty(characterClassManager.UserId) ? "(no User ID)" : characterClassManager.UserId);
                    return false;
				}
				catch (Exception arg)
				{


					Logger.Error("[EVENT MANAGER]", string.Format("RaPlayerListPatch error: {0}", arg));
					return true;
				}
			
			}
		}


	}
}
