using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore;
using Interactables.Interobjects.DoorUtils;
using LightContainmentZoneDecontamination;
using MapGeneration;
using MapGeneration.Distributors;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using RemoteAdmin;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using InventorySystem;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using CustomPlayerEffects;
using PlayerRoles;
using System.Data;
using Utils.Networking;
using Footprinting;
using Subtitles;
using WW_SYSTEM.Triggers;
using PlayerRoles.PlayableScps.Scp079;

namespace WW_SYSTEM.API
{
	public static class Round
	{



		public static void Init()
		{
			Logger.Info("ROUND", "STARTED!");
			RefreshConfig();


		}


		public static List<GameObject> GetAllPrefabs => NetworkClient.prefabs.Values.ToList();

		public static bool TryGetPrefab(string Name, out GameObject Prefab)
		{
			foreach (var item in GetAllPrefabs)
			{
				if(item.name.ToUpper() == Name.ToUpper())
				{
					Prefab = item;
					return true;

				}
			}
			Prefab = null;
			return false;

		}

		#region GetDoors

		public static List<ROOM_DOOR> GetDoors()
		{
			List<ROOM_DOOR> result = new List<ROOM_DOOR>();

			foreach (var item in GameObject.FindObjectsOfType<DoorVariant>().ToList().Where(door => door != null).ToList())
			{
				result.Add(new ROOM_DOOR(item));
			}

			return result;

		}

		public static List<ROOM_DOOR> GetDoors(string DoorName)
		{
			List<ROOM_DOOR> result = new List<ROOM_DOOR>();

			foreach (var item in GetDoors())
			{
				if (item.DOORNAME.Contains(DoorName))
				{
					result.Add(item);
				}
				if (item.OBJNAME.Contains(DoorName))
				{
					result.Add(item);
				}

			}

			return result;

		}

		public static List<ROOM_DOOR> GetDoors(DOOR_PERMISSION_TYPE PermType)
		{
			List<ROOM_DOOR> result = new List<ROOM_DOOR>();

			foreach (var item in GetDoors())
			{
				if (item.PERMISSION_TYPE == PermType)
				{
					result.Add(item);
				}


			}

			return result;

		}

		public static bool TryGetDoor(string DoorName, out ROOM_DOOR door)
		{
			foreach (var item in GetDoors())
			{
				if (item.DOORNAME.Contains(DoorName))
				{
					door = item;
					return true;
				}
				if (item.OBJNAME.Contains(DoorName))
				{
					door = item;
					return true;
				}

			}
			door = null;
			return false;



		}


		public static ROOM_DOOR GetDoor(string DoorName)
		{
			foreach (var item in GetDoors())
			{
				if (item.DOORNAME.Contains(DoorName))
				{
					return item;
				}
				if (item.OBJNAME.Contains(DoorName))
				{
					return item;
				}

			}
			return null;



		}

		#endregion

		public static void SetMaxHP(RoleTypeId type, int maxhp, bool WriteToCfg = true)
		{
			if (MaxHps.ContainsKey(type))
			{
				MaxHps[type] = maxhp;
			}
			else
			{
				MaxHps.Add(type, maxhp);
			}
		
			
			if (WriteToCfg)
			{
				ConfigFile.ServerConfig.SetString(type + "_HP", maxhp.ToString());
			}
		}

		public static int GetMaxHp(RoleTypeId type)
		{
			if (MaxHps.ContainsKey(type))
			{
				return MaxHps[type];
			}
			return -1;
		}

		public static void ReloadMaxHps()
		{
			MaxHps.Clear();
            foreach (var item in Enum.GetValues(typeof(RoleTypeId)))
			{
                var type = (RoleTypeId)item;

				PlayerRoleBase role;
                if (PlayerRoleLoader.TryGetRoleTemplate(type, out role))
				{
                    IHealthbarRole healthbarRole;
                    if ((healthbarRole = (role as IHealthbarRole)) != null)
                    {
						try
						{
							
                            var maxhp = ConfigFile.ServerConfig.GetInt(type + "_HP", (int)healthbarRole.MaxHealth);
                            Logger.Info("ROUND", $"Set hp for {type} value {maxhp} def {(int)healthbarRole.MaxHealth}");

                            SetMaxHP(type, maxhp, false);
                        }
						catch (Exception ex)
						{
							Logger.Error("ERROR", $"FAILED TO GET MAX HP FOR: {type} : " + ex);
							
						}
						continue;
                    }
                }



                var maxhp1 = ConfigFile.ServerConfig.GetInt(type + "_HP", 1);

					SetMaxHP(type, maxhp1, false);
			}
		}

		public static void RefreshConfig()
		{
			
			ElevatorSpeed = 8;
			IntercomTextMuted = "YOU ARE MUTED BY ADMIN";
			IntercomTextAdmin = "ADMIN IS USING THE INTERCOM NOW";
			IntercomTextRestart = "RESTARTING";
			IntercomTextTransmitBYPASS = "TRANSMITTING... BYPASS MODE";
			IntercomTextTransmit = "TRANSMITTING... TIME LEFT - ";
			IntercomTextReady = "READY";

			Logger.Info("ROUND", "UPDATING CFG...");

		

	

	
			ElevatorSpeed = ConfigFile.ServerConfig.GetInt("Elevator_speed", 5);
			Logger.Info("ROUND", "SETUP ELEVATOR SPEED TO: " + ElevatorSpeed);
			UseLevelSYSTEM = ConfigFile.ServerConfig.GetBool("ENABLE_LEVEL_SYSTEM", false);
			Logger.Info("ROUND", "SETUP STATUS LEVEL SYSTEM TO: " + (UseLevelSYSTEM ? "ENABLED" : "DISABLED"));
			
			Scp049Speak = ConfigFile.ServerConfig.GetBool("scp_049_speak", false);
			Logger.Info("ROUND", "SETUP SCP-049 SPEAK MODE TO: " + (Scp049Speak ? "ENABLED" : "DISABLED"));
			unlimited_radio_battery = ConfigFile.ServerConfig.GetBool("unlimited_radio_battery", false);
			Logger.Info("ROUND", "SETUP UNLIMITED RADIO BATTERY TO: " + (unlimited_radio_battery ? "ENABLED" : "DISABLED"));
			Loot_Spawn_Multipler = ConfigFile.ServerConfig.GetInt("Loot_Spawn_Multipler", 1);
			Logger.Info("ROUND", "SETUP LOOT MULTIPLER TO: " + Loot_Spawn_Multipler);
			TeslaDamage = ConfigFile.ServerConfig.GetFloat("Tesla_Damage", (float)UnityEngine.Random.Range(200, 300));
			Logger.Info("ROUND", "SETUP TESLA DAMAGE TO: " + TeslaDamage);

			UseItemPhysics = ConfigFile.ServerConfig.GetBool("Use_Item_Physics", false);
			Logger.Info("ROUND", "SETUP ITEM PHYSICS TO: " + UseItemPhysics);
			ScpsIgnoreTutorial = ConfigFile.ServerConfig.GetBool("ScpsIgnoreTutorial", false);
			Logger.Info("ROUND", "SETUP SCPS IGNORE TUTORIAL TO: " + ScpsIgnoreTutorial);
			Logger.Info("ROUND", "UPDATING CFG COMPLETED!");



		}






		public static void Mute(string UserId)
		{
			if (TryGetPlayer(UserId, out Player pl))
			{
				pl.Mute(MuteType.Voice);
			}

		}

		public static void UnMute(string UserId)
		{
			if (TryGetPlayer(UserId, out Player pl))
			{
				pl.UnMute(MuteType.Voice);
			}
		}

		public static void IcomMute(string UserId)
		{

			if (TryGetPlayer(UserId, out Player pl))
			{
				pl.Mute(MuteType.Intercom);
			}

		}

		public static void IcomUnMute(string UserId)
		{

			if (TryGetPlayer(UserId, out Player pl))
			{
				pl.UnMute(MuteType.Intercom);
			}

		}

		public static bool IsMuted(string UserId)
		{
            if (TryGetPlayer(UserId, out Player pl))
            {
				return pl.IsMuted(MuteType.Voice) || pl.IsMuted(MuteType.Intercom);
            }
			return false;
        }

		public static int TimeToRespawnMtf => Mathf.RoundToInt(RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);

		public static void AddMtfRespawnTime(float time)
		{

			RespawnManager.Singleton._timeForNextSequence += time;
		}
		public static void TakeMtfRespawnTime(float time)
		{

			RespawnManager.Singleton._timeForNextSequence -= time;
		}

		public static void ForceSpawnTeam(SpawnableTeamType type)
		{
			if (type == SpawnableTeamType.None) return;
			RespawnManager.Singleton.ForceSpawnTeam(type);
		}

		public static void StartSpawnTeamAnim(SpawnableTeamType type)
		{
			if (type == SpawnableTeamType.None) return;
			RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, type);
		}

		public static void EndSpawnTeamAnim(SpawnableTeamType type)
		{
			if (type == SpawnableTeamType.None) return;
			RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, type);
		}

	
		public static GameObject SpawnWorkbench(Vector3 position, Vector3 rotation, Vector3 size)
		{
			GameObject gameObject = GameObject.Instantiate<GameObject>(NetworkManager.singleton.spawnPrefabs.Find((GameObject p) => p.gameObject.name == "Work Station"));
			Offset networkposition = default(Offset);
			networkposition.position = position;
			networkposition.rotation = rotation;
			networkposition.scale = size;
			gameObject.gameObject.transform.localScale = size;
			gameObject.transform.position = networkposition.position;
			//gameObject.transform.rotation = networkposition.rotation;
			NetworkServer.Spawn(gameObject);
			//	gameObject.GetComponent<WorkStation>().Networkposition = networkposition;
			return gameObject;
		}

		public static bool OfflineBan(string ip, string id, string Nick, int duration, string reason, string AdminNick)
		{
			ServerConsole.AddLog("[BAN] BAN OFFLINE USER: " + Nick + " ADMIN: " + AdminNick);
			if (id == "owner@waer-world")
			{
				return false;
			}
			string text = "";
			try
			{
				if (ConfigFile.ServerConfig.GetBool("online_mode", false))
				{
					text = id;
				}
			}
			catch
			{
				ServerConsole.AddLog("Failed during issue of User ID ban (1)!");
			}
			if (duration > 0)
			{
				string originalName = string.IsNullOrEmpty(Nick) ? "(no nick)" : Nick;
				long issuanceTime = TimeBehaviour.CurrentTimestamp();
				long banExpieryTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
				try
				{
					if (text != null)
					{
						ServerConsole.AddLog("[BAN] BAN OFFLINE NICK AND STEAMID");
						BanHandler.IssueBan(new BanDetails
						{
							OriginalName = originalName,
							Id = text,
							IssuanceTime = issuanceTime,
							Expires = banExpieryTime,
							Reason = reason,
							Issuer = AdminNick
						}, BanHandler.BanType.UserId);
					}
				}
				catch
				{
					ServerConsole.AddLog("Failed during issue of User ID ban (2)!");
					return false;
				}
				try
				{
					if (ConfigFile.ServerConfig.GetBool("ip_banning", false) && !string.IsNullOrEmpty(ip))
					{
						ServerConsole.AddLog("[BAN] BAN OFFLINE IP");
						BanHandler.IssueBan(new BanDetails
						{
							OriginalName = originalName,
							Id = ip,
							IssuanceTime = issuanceTime,
							Expires = banExpieryTime,
							Reason = reason,
							Issuer = AdminNick
						}, BanHandler.BanType.IP);
					}
				}
				catch
				{
					ServerConsole.AddLog("Failed during issue of IP ban!");
					return false;
				}
				return true;
			}
			return true;
		}


	

		public static void SetHpForClass(RoleTypeId type, int value)
		{
			ConfigFile.ServerConfig.SetString(type.ToString() + "_HP", value.ToString());
		}




		public static int RoundTimeSeconds
		{
			get
			{
				return Convert.ToInt32(RoundStart.RoundLength.TotalSeconds);
			}
		}


		public static int RoundTimeMinutes
		{
			get
			{
				return Convert.ToInt32(RoundStart.RoundLength.TotalMinutes);
			}
		}


		public static int RoundTimeHour
		{
			get
			{
				return Convert.ToInt32(RoundStart.RoundLength.TotalHours);
			}
		}


		public static string RoundTime(bool hour = true, bool minute = true, bool seconds = true)
		{
			string text = "";
			int RoundTime = RoundTimeSeconds;
			if (hour)
			{
				text += Time.ConvertTime(RoundTime, FormatTime.Hours) + ":";
			}
			if (minute)
			{
				text += Time.ConvertTime(RoundTime, FormatTime.Minutes) + ":";
			}
			if (seconds)
			{
				text += Time.ConvertTime(RoundTime, FormatTime.Seconds);
			}
			return text;
		}

		public static void SetIntercomText(IntercomType type, string text, bool WriteToConfig = true)
		{
			if (type == IntercomType.ADMIN)
			{
				IntercomTextAdmin = text;
				if (WriteToConfig)
				{
					ConfigFile.ServerConfig.SetString("IntercomTextAdmin", text.Replace("\n", ""));
				}
				return;
			}
			if (type == IntercomType.BYPASS)
			{
				IntercomTextTransmitBYPASS = text;
				if (WriteToConfig)
				{
					ConfigFile.ServerConfig.SetString("IntercomTextTransmitBYPASS", text.Replace("\n", ""));
				}
				return;
			}
			if (type == IntercomType.MUTED)
			{
				IntercomTextMuted = text;
				if (WriteToConfig)
				{
					ConfigFile.ServerConfig.SetString("IntercomTextMuted", text.Replace("\n", ""));
				}
				return;
			}
			if (type == IntercomType.READY)
			{
				IntercomTextReady = text;
				if (WriteToConfig)
				{
					ConfigFile.ServerConfig.SetString("IntercomTextReady", text.Replace("\n", ""));
				}
				return;
			}
			if (type == IntercomType.RESTARTING)
			{
				IntercomTextRestart = text;
				if (WriteToConfig)
				{
					ConfigFile.ServerConfig.SetString("IntercomTextRestart", text.Replace("\n", ""));
				}
				return;
			}
			if (type == IntercomType.TRANSMITTING)
			{
				IntercomTextTransmit = text;
				if (WriteToConfig)
				{
					ConfigFile.ServerConfig.SetString("IntercomTextTransmit", text.Replace("\n", ""));
				}
				return;
			}
		}


		public static void BroadCast(string data, ushort time = 10, BroadcastType type = BroadcastType.Normal)
		{
			int p = (int)type;
			Broadcast.Singleton.RpcClearElements();
			Broadcast.Singleton.RpcAddElement(data, time, (Broadcast.BroadcastFlags)p);
		}


		public static void CustomCassie(string text)
		{
			CassieCustomAnnouncementEvent cassieCustomAnnouncementEvent = new CassieCustomAnnouncementEvent(text, false, true);
			EventManager.Manager.HandleEvent<IEventHandlerCassieCustomAnnouncement>(cassieCustomAnnouncementEvent);
			if (!cassieCustomAnnouncementEvent.Allow)
			{
				return;
			}
			string words = cassieCustomAnnouncementEvent.Words;
			float num = WWRandom.RandomFloat(1, 2.5f);
			NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(words, UnityEngine.Random.Range(0.08f, 0.1f) * num, UnityEngine.Random.Range(0.07f, 0.09f) * num);
		}


	


		public static bool WarheadDetonated
		{
			get
			{
				return AlphaWarheadController.Detonated;
			}
		}


		public static bool DetonationInProgress
		{
			get
			{
				return AlphaWarheadController.InProgress;
			}
		}

	


		public static List<Player> GetPlayers()
		{
			return Server.Players;
		}


		public static int PlayersCount
		{
			get
			{

				return Server.Players.Count;
			}
		}

		public static void StartWarhead()
		{
			AlphaWarheadController.Singleton.StartDetonation();
		}


		public static void StartWarheadAdv(bool Force = false, bool EnableCancel = true, ReferenceHub Activator = null, bool suppressSubtitles = false)
		{
			WarheadLocked = !EnableCancel;
			if (DetonationInProgress) return;
			WarheadStartEvent warheadStartEvent = new WarheadStartEvent((Activator != null) ? Activator.GetComponent<Player>() : null, AlphaWarheadController.TimeUntilDetonation, AlphaWarheadController.Singleton.Info.ResumeScenario, true);
			EventManager.Manager.HandleEvent<IEventHandlerWarheadStartCountdown>(warheadStartEvent);
			if (warheadStartEvent.Cancel && !Force)
			{
				return;
			}
			var controller = AlphaWarheadController.Singleton;
            if (controller.Info.InProgress || controller.CooldownEndTime > NetworkTime.time || controller.IsLocked)
            {
                return;
            }
            controller._alreadyDetonated = false;
            controller._doorsAlreadyOpen = false;
            ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent, false);
            controller._triggeringPlayer = new Footprint(Activator != null ? Activator : null);
            AlphaWarheadSyncInfo info = controller.Info;
            info.StartTime = NetworkTime.time;
            controller.NetworkInfo = info;
            if (suppressSubtitles)
            {
                return;
            }
            SubtitleType subtitle = controller.Info.ResumeScenario ? SubtitleType.AlphaWarheadResumed : SubtitleType.AlphaWarheadEngage;
            new SubtitleMessage(new SubtitlePart[]
            {
            new SubtitlePart(subtitle, new string[]
            {
                controller.CurScenario.TimeToDetonate.ToString()
            })
            }).SendToAuthenticated(0);

        }


		public static void StopWarhead()
		{
			AlphaWarheadController.Singleton.CancelDetonation();
		}


		public static void DetonateWarhead()
		{
			AlphaWarheadController.Singleton.Detonate();
		}


		public static void EndRound()
		{
			ForceEndRound = true;

		}



		#region GetPlayer

		public static Player GetPlayer(RoleTypeId type)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.Role == type)
				{
					return player;
				}
			}
			return null;
		}

		public static Player GetPlayer(uint NetId)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.inv.netId == NetId)
				{
					return player;
				}
			}
			return null;
		}

		public static Player GetPlayer(GameObject obj)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.gameObject == obj)
				{
					return player;
				}
			}
			return null;
		}

		public static Player GetPlayer(ReferenceHub hub)
		{

			return GetPlayer(hub.characterClassManager.UserId);
		}


		public static Player GetPlayer(int id)
		{
			foreach (Player pl in GetPlayers())
			{
				if (pl.id == id)
				{
					return pl;
				}
			}
			return null;
		}

		public static Player GetPlayer(string UserIdOrNick)
		{
			foreach (Player pl in GetPlayers())
			{
				if (pl.UserId == UserIdOrNick)
				{
					return pl;
				}
				else if (pl.Nick == UserIdOrNick)
				{
					return pl;
				}
			}
			return null;
		}

		#endregion

		#region GetPlayers
		public static List<Player> GetPlayers(RoleTypeId type)
		{
			List<Player> list = new List<Player>();
			foreach (Player Player in GetPlayers())
			{
				if (Player.Role == type)
				{
					list.Add(Player);
				}
			}
			return list;
		}

		public static List<Player> GetPlayers(TEAMTYPE type)
		{
			List<Player> list = new List<Player>();
			foreach (Player pl in GetPlayers())
			{
				if (pl.CheckForTeam(type))
				{
					list.Add(pl);
				}
			}
			return list;
		}

		public static List<Player> GetPlayers(RoomType room)
		{
			List<Player> list = new List<Player>();
			foreach (Player pl in GetPlayers())
			{
				if (pl.CurRoom == null) continue;
				if (pl.CurRoom.Type == room)
				{
					list.Add(pl);
				}
			}
			return list;
		}

		public static List<Player> GetPlayers(ZoneType zone)
		{
			List<Player> list = new List<Player>();
			foreach (Player pl in GetPlayers())
			{
				if (pl.CurrentZone == zone)
				{
					list.Add(pl);
				}
			}
			return list;
		}

		public static List<Player> GetPlayers(List<int> Ids)
		{
			List<Player> list = new List<Player>();
			foreach (var id in Ids)
			{
				if (TryGetPlayer(id, out var pl))
				{
					list.Add(pl);
				}
			}
			return list;
		}

		public static List<Player> GetPlayers(List<string> UserIdsOrNiks)
		{
			List<Player> list = new List<Player>();
			foreach (var id in UserIdsOrNiks)
			{
				if (TryGetPlayer(id, out var pl))
				{
					list.Add(pl);
				}
			}
			return list;
		}

		#endregion

		#region TryGetPlayer
		public static bool TryGetPlayer(RoleTypeId type, out Player pl)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.Role == type)
				{
					pl = player;
					return true;
				}
			}
			pl = null;
			return false;
		}
		public static bool TryGetPlayer(ReferenceHub hub, out Player pl)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.Hub == hub)
				{
					pl = player;
					return true;
				}
			}
			pl = null;
			return false;
		}


		public static bool TryGetPlayer(int id, out Player pl)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.id == id)
				{
					pl = player;
					return true;
				}
			}
			pl = null;
			return false;
		}

		public static bool TryGetPlayer(GameObject obj, out Player pl)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.PlayerObj == obj)
				{
					pl = player;
					return true;
				}
			}
			pl = null;
			return false;
		}

		public static bool TryGetPlayer(uint NetId, out Player pl)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.inv.netId == NetId)
				{
					pl = player;
					return true;
				}
			}
			pl = null;
			return false;
		}

		public static bool TryGetPlayer(string UserIdOrNick, out Player pl)
		{
			foreach (Player player in GetPlayers())
			{
				if (player.UserId == UserIdOrNick)
				{
					pl = player;
					return true;
				}
				else if (player.Nick == UserIdOrNick)
				{
					pl = player;
					return true;
				}
			}
			pl = null;
			return false;
		}
		#endregion

		public static bool LCZDecontaminated
		{
			get
			{
				return DecontaminationController.Singleton._decontaminationBegun;
			}
		}




		public static void CustomCassieSL(string text)
		{
			CassieCustomAnnouncementEvent cassieCustomAnnouncementEvent = new CassieCustomAnnouncementEvent(text, false, true);
			EventManager.Manager.HandleEvent<IEventHandlerCassieCustomAnnouncement>(cassieCustomAnnouncementEvent);
			if (!cassieCustomAnnouncementEvent.Allow)
			{
				return;
			}
			string words = cassieCustomAnnouncementEvent.Words;

			NineTailedFoxAnnouncer.singleton.AddPhraseToQueue(words, false, false, false);

		}


		public static void EnableFlickering(float duration = 10f)
		{


			foreach (var item in FlickerableLightController.Instances)
			{
				item.ServerFlickerLights(duration);
			}

		}

		public static void EnableFlickering(float duration = 10f, RoomType type = RoomType.NONE)
		{


			foreach (var item in GetRooms(type))
			{
				item.EnableFlickering(duration);
			}

		}

		public static void EnableFlickering(float duration = 10f, ZoneType type = ZoneType.NONE)
		{

			foreach (var item in GetRooms(type))
			{
				item.EnableFlickering(duration);
			}

		}

		public static void SetWarheadLightColor(Color color)
		{


			foreach (var item in FlickerableLightController.Instances)
			{

				item.Network_warheadLightColor = color;
				item.Network_warheadLightOverride = true;
			}

		}


		public static void ClearBroadcasts()
		{
			Broadcast.Singleton.RpcClearElements();

		}


		public static void Ban(GameObject user, int duration, string reason, string AdminNick)
		{
			OfflineBan(user.gameObject.GetComponent<Player>().Conn.address, user.gameObject.GetComponent<Player>().UserId, user.gameObject.GetComponent<Player>().Nick, duration, reason, AdminNick);
			ServerConsole.Disconnect(user, "Вы были забанены причина: " + reason);
		}






		public static List<Vector> GetSpawnPoints(RoleTypeId classID)
		{
			var result = new List<Vector>();
			if(HumanRole.SpawnpointsForRoles.TryGetValue(classID, out var value))
			{
				if(value != null && value.Length > 0)
				{
					foreach (var item in value)
					{
						if(item.TryGetSpawnpoint(out var pos, out var rot))
						{
							result.Add(pos.ToVector());

                        }
                        

                    }
				}

			}
			return result;
		}


	


		public static Vector GetRandomSpawnPoint(RoleTypeId role)
		{
		
			return GetSpawnPoints(role).Random();
		}

		#region SpawnItem

		public static ItemPickupBase SpawnItem(ItemType type, Vector3 pos)
		{
			if(InventoryItemLoader.AvailableItems.TryGetValue(type, out var item))
			{
				PickupSyncInfo psi = new PickupSyncInfo
				{
					ItemId = type,
					Serial = ItemSerialGenerator.GenerateNext(),
					Weight = item.Weight,
				};
				var PickUp = ReferenceHub.LocalHub.inventory.ServerCreatePickup(item, psi, true);

				PickUp.transform.position = pos;
				PickUp.RefreshPositionAndRotation();
				return PickUp;
			}
			return null;
		}

		public static ItemPickupBase SpawnItem(ItemType type, Vector3 pos, float Weight)
		{
			if (InventoryItemLoader.AvailableItems.TryGetValue(type, out var item))
			{
				PickupSyncInfo psi = new PickupSyncInfo
				{
					ItemId = type,
					Serial = ItemSerialGenerator.GenerateNext(),
					Weight = Weight,
				};
				var PickUp = ReferenceHub.LocalHub.inventory.ServerCreatePickup(item, psi, true);

				PickUp.transform.position = pos;
				PickUp.RefreshPositionAndRotation();
				return PickUp;
			}
			return null;
		}

		public static ItemPickupBase SpawnItem(ItemType type, Vector3 pos, float Weight, Quaternion rot)
		{
			if (InventoryItemLoader.AvailableItems.TryGetValue(type, out var item))
			{
				PickupSyncInfo psi = new PickupSyncInfo
				{
					ItemId = type,
					Serial = ItemSerialGenerator.GenerateNext(),
					Weight = Weight,
				};
				var PickUp = ReferenceHub.LocalHub.inventory.ServerCreatePickup(item, psi, true);

				PickUp.transform.position = pos;
				PickUp.transform.rotation = rot;
				PickUp.RefreshPositionAndRotation();
				return PickUp;
			}
			return null;
		}

		public static ItemPickupBase SpawnItem(ItemType type, Vector3 pos, Quaternion rot)
		{
			if (InventoryItemLoader.AvailableItems.TryGetValue(type, out var item))
			{
				PickupSyncInfo psi = new PickupSyncInfo
				{
					ItemId = type,
					Serial = ItemSerialGenerator.GenerateNext(),
					Weight = item.Weight
				};
				var PickUp = ReferenceHub.LocalHub.inventory.ServerCreatePickup(item, psi, true);

				PickUp.transform.position = pos;
				PickUp.transform.rotation = rot;
				PickUp.RefreshPositionAndRotation();
				return PickUp;
			}
			return null;
		}
		#endregion

		public static Scp079Generator[] GetGenerators()
		{
			Scp079Generator[] array = UnityEngine.Object.FindObjectsOfType<Scp079Generator>();
			Scp079Generator[] array2 = new Scp079Generator[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i];
			}
			return array2;
		}

		
		public static RoomIdentifier[] Get079InteractionRooms()
		{
			List<RoomIdentifier> list = new List<RoomIdentifier>();

			foreach (var item in Scp079InteractableBase.AllInstances)
			{
				list.Add(item.Room);
			}
		
			return list.ToArray();
		}


		#region Rooms

		[Obsolete("This method will be removed in future versions please use: GetRooms()")]
		public static List<ROOM> GetRoomsOld()
		{
			List<ROOM> list = new List<ROOM>();
			foreach (RoomIdentifier room in Get079InteractionRooms())
			{
				list.Add(new ROOM(room));
			}
			return list;
		}

		public static List<ROOM> GetRooms()
		{
			List<ROOM> Result = new List<ROOM>();

			//  foreach (var item in Get079InteractionRooms())
			//  {
			//	Result.Add(new ROOM(item));
			//  }
			

			foreach (var item in RoomIdentifier.AllRoomIdentifiers)
			{
				Result.Add(new ROOM(item));
			}
			return Result;
		}

		public static List<ROOM> GetRooms(string Name)
		{
			List<ROOM> Result = new List<ROOM>();

			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if (item.Name.ToUpper().Contains(Name.ToUpper()))
				{
					Result.Add(item);
				}
			}
			return Result;
		}

		public static List<ROOM> GetRooms(ZoneType zone)
		{
			List<ROOM> Result = new List<ROOM>();

			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if (item.Zone == zone)
				{
					Result.Add(item);
				}
			}
			return Result;
		}

		public static List<ROOM> GetRooms(RoomType type)
		{
			List<ROOM> Result = new List<ROOM>();
			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if (item.Type == type)
				{
					Result.Add(item);
				}
			}
			return Result;
		}

		public static ROOM GetRoom(RoomType type)
		{
			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if(item.Type == type)
				{
					return item;
				}
			}
			return null;
		}

		public static ROOM GetRoom(Vector3 position)
		{
			RoomIdentifier room = RoomIdUtils.RoomAtPosition(position);
			if(room != null)
			{
				return new ROOM(room);
			}
			return null;
		}

		public static ROOM GetRoom(string Name)
		{
			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if (item.Name == Name)
				{
					return item;
				}
			}
			return null;
		}



		public static bool TryGetRoom(RoomType type, out ROOM room)
		{
			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if (item.Type == type)
				{
					room = item;
					return true;
				}
			}
			room = null;
			return false;
		}

		public static bool TryGetRoom(string Name, out ROOM room)
		{
			List<ROOM> Rooms = GetRooms();

			foreach (var item in Rooms)
			{
				if (item.Name == Name)
				{
					room = item;
					return true;
				}
			}
			room = null;
			return false;
		}


		#endregion

		public static void Kick(GameObject user, string reason, string AdminNick)
		{
			ServerConsole.Disconnect(user, "Вы были кикнуты администратором: " + AdminNick + " причина: " + reason);
		}

		#region GetAlive
		public static int GetAlive(TEAMTYPE type)
		{
			return GetPlayers(type).Count;
		}



	
		public static int GetAlive(RoleTypeId type)
		{
			return GetPlayers(type).Count;
		}

		#endregion

		#region Params

		public static void RemoveParameter(int id)
		{

			

			using (List<ServerParam>.Enumerator enumerator = Parameters.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ServerParam playerParam = enumerator.Current;
					if (playerParam.ParamId == id)
					{
						Parameters.Remove(playerParam);
					}
				}
			}
		}


		public static ServerParam GetParameter(int id)
		{
			foreach (ServerParam playerParam in Parameters)
			{
				if (playerParam.ParamId == id)
				{
					return playerParam;
				}
			}
			return null;
		}


		public static void AddParameter(int ParamId)
		{
			Parameters.Add(new ServerParam(ParamId, "", false, 0, 0f));
		}

		public static void AddParameter(int ParamId, string ParamStringData, bool ParamBoolData, int ParamIntData, float ParamFloatData)
		{
			Parameters.Add(new ServerParam(ParamId, ParamStringData, ParamBoolData, ParamIntData, ParamFloatData));
		}

		public static void AddParameter(int ParamId, string ParamStringData = "DATA")
		{
			Parameters.Add(new ServerParam(ParamId, ParamStringData, false, 0, 0f));
		}

		public static void AddParameter(int ParamId, bool ParamBoolData = false)
		{
			Parameters.Add(new ServerParam(ParamId, "", ParamBoolData, 0, 0f));
		}

		public static void AddParameter(int ParamId, int ParamIntData = 0)
		{
			Parameters.Add(new ServerParam(ParamId, "", false, ParamIntData, 0f));
		}

		public static void AddParameter(int ParamId, float ParamFloatData = 0f)
		{
			Parameters.Add(new ServerParam(ParamId, "", false, 0, ParamFloatData));
		}



		public static void UpdateParameter(int ParamId, string ParamStringData = "DATA")
		{

			int index = Parameters.IndexOf(Parameters.Find((ServerParam p) => p.ParamId == ParamId));
			Parameters[index].ParamStringData = ParamStringData;


		}

		public static void UpdateParameter(int ParamId, bool ParamBoolData = false)
		{
			int index = Parameters.IndexOf(Parameters.Find((ServerParam p) => p.ParamId == ParamId));
			Parameters[index].ParamBoolData = ParamBoolData;
		}

		public static void UpdateParameter(int ParamId, int ParamIntData = 0)
		{
			int index = Parameters.IndexOf(Parameters.Find((ServerParam p) => p.ParamId == ParamId));
			Parameters[index].ParamIntData = ParamIntData;
		}

		public static void UpdateParameter(int ParamId, float ParamFloatData = 0f)
		{
			int index = Parameters.IndexOf(Parameters.Find((ServerParam p) => p.ParamId == ParamId));
			Parameters[index].ParamFloatData = ParamFloatData;
		}

		public static void ShowComponentsInConsole(GameObject o)
		{

			Logger.Info(o.name, "COMPONENTS:");
			foreach (var item in o.GetComponents<Component>())
			{

				var compname = item.GetType().FullName;
				if (!compname.Contains("Transform"))
					Logger.Info(o.name, compname);


			}

			foreach (var item in o.GetComponentsInChildren<Component>())
			{

				var compname = item.GetType().FullName;
				if (!compname.Contains("Transform"))
					Logger.Info($"{o.name} CHILD", compname);

			}
			foreach (var item in o.GetComponentsInParent<Component>())
			{

				var compname = item.GetType().FullName;
				if (!compname.Contains("Transform"))
					Logger.Info($"{o.name} PARENT", compname);

			}

		}

		public static LayerMask DetectionMask
		{
			get
			{
				if(detectionMask == 0)
				{
				var grenade = InventoryItemLoader.AvailableItems[ItemType.GrenadeHE];
					ThrowableItem throwableItem;
					if ((throwableItem = (grenade as ThrowableItem)) != null && !(throwableItem.Projectile == null))
					{
						var projectile = throwableItem.Projectile;
						ExplosionGrenade explosion;
						if((explosion = (projectile as ExplosionGrenade)) != null)
						{
							detectionMask = explosion._detectionMask;

				
						}
					}
				}
				return detectionMask;

			}
		}


		public static bool InProgress
		{
			get
			{
				if (ReferenceHub.LocalHub == null) return false;
				if (RoundSummary.singleton == null) return false;

				try
				{
					return RoundSummary.RoundInProgress();

				}
				catch (Exception ex)
				{
					Logger.Error("Round::InProgress", ex.Message);
					return false;
					
				}
			}
		}

		public static List<PLAYER_PERMISSION> PositionPerms = new List<PLAYER_PERMISSION>();

		public static Dictionary<Type, float> PlayerEffectsDamage = new Dictionary<Type, float>();

		private static LayerMask detectionMask = 0;

		public static Dictionary<RoleTypeId, int> MaxHps = new Dictionary<RoleTypeId, int>();

		#endregion
		public static List<string> OverWatchEnabledUserIds = new List<string>();

		public static List<ServerParam> Parameters = new List<ServerParam>();

		private static string IntercomTextMuted;

		private static string IntercomTextAdmin;

		public static float TeslaDamage = UnityEngine.Random.Range(200f, 300f);

		private static string IntercomTextRestart;

		public static bool UseItemPhysics = false;

		public static bool ScpsIgnoreTutorial = false;

		private static string IntercomTextTransmitBYPASS;

		public static bool TeslaIgnore079 = false;

		private static string IntercomTextTransmit;

		public static bool WarheadLocked
		{
			get
			{
				return AlphaWarheadController.Singleton.IsLocked;
			}set
			{
				AlphaWarheadController.Singleton.IsLocked = value;
			}
		}

		public static int RoundCount = 0;

		private static string IntercomTextReady;

		public static int ElevatorSpeed = 5;

		public static bool Scp049Speak = false;

		public static bool UseLevelSYSTEM;

		public static bool unlimited_radio_battery = false;

		public static int Loot_Spawn_Multipler = 1;

		public static bool ForceEndRound;

		
	
	}





	public class ServerParam
	{
		public ServerParam(int ParamId, string ParamStringData, bool ParamBoolData, int ParamIntData, float ParamFloatData)
		{
			this.ParamId = ParamId;
			this.ParamStringData = ParamStringData;
			this.ParamBoolData = ParamBoolData;
			this.ParamIntData = ParamIntData;
			this.ParamFloatData = ParamFloatData;
		}


		public int ParamId;


		public string ParamStringData;


		public bool ParamBoolData;


		public int ParamIntData;


		public float ParamFloatData;
	}

}


