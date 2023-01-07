using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RemoteAdmin;
using CustomPlayerEffects;
using Hints;
using WW_SYSTEM.Level_System;
using WW_SYSTEM.Discord;
using WW_SYSTEM.Custom_Items;
using System.Reflection;
using InventorySystem;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem.Disarming;
using System.Linq;
using MEC;
using MapGeneration;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using WW_SYSTEM.Translation;
using WW_SYSTEM.Permissions;
using RoundRestarting;
using PlayerStatsSystem;
using Achievements;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using VoiceChat;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using Utils.Networking;

namespace WW_SYSTEM.API
{
	public class Player : NetworkBehaviour
	{

		public Player() {

		
		
		}


		public bool IsLocalPlayer { get; private set; }

		public bool IsDiscordBot = false;
		public bool HideRaRole = false;

		public void LoadComponents()
		{
			if(isLocalPlayer)
			{
				Server.LocalPlayer = this;
				IsLocalPlayer = true;
			}
			Hub = GetComponent<ReferenceHub>();
			CCM = GetComponent<CharacterClassManager>();
			GetRoles = GetComponent<ServerRoles>();
			stats = Hub.playerStats;
			inv = GetComponent<Inventory>();
			NicknameSync = GetComponent<NicknameSync>();
			CIM = gameObject.AddComponent<CustomInventoryManager>();
			QueryProcessor = GetComponent<QueryProcessor>();
			CIM.pl = this;

			
		}



		public ulong Permissions {

			get {
				if (IsLocalPlayer)
				{

					return ServerStatic.PermissionsHandler.FullPerm;
				}

				return GetRoles.Permissions;
			}
		
		
		}


		public bool IsPermitted(List<PLAYER_PERMISSION> PermsToCheck)
		{
			if (IsLocalPlayer)
			{

				return true;
			}


			foreach (var item in PermsToCheck)
			{
				if (item == PLAYER_PERMISSION.NONE) continue;
				if(!IsPermitted(item))
				{
					return false;
				}
			}

			return true;
		}

		public bool IsPermitted(PLAYER_PERMISSION PermToCheck)
		{
			if (IsLocalPlayer)
			{

				return true;
			}
			if (PermToCheck == PLAYER_PERMISSION.NONE) return true;

			var Result = (Permissions & (ulong)PermToCheck) > 0UL;

			if (!Result) return IsPermitted(PermToCheck.ToString());

			return true;
		}
		public bool IsPermitted(string Permission)
		{
			if (isLocalPlayer)
			{

				return true;
			}

			if(Perms != null && Perms.Count > 0)
			{
				if (Perms.Contains("*") && Permission.ToUpper() != "WW_SYSTEM_DEVELOPER") return true;

				return Perms.Contains(Permission.ToUpper());
			}
		   

			return false;
		}

		public bool IsStuck(Vector3 pos) {

			bool result = false;
			foreach (Collider collider in Physics.OverlapBox(pos, new Vector3(0.4f, 1f, 0.4f), new Quaternion(0f, 0f, 0f, 0f)))
			{
				if (!collider.name.Contains("Hitbox") && !collider.name.Contains("mixamorig") && !collider.name.Equals("Player") && !collider.name.Equals("PlyCenter") && !collider.name.Equals("Antijumper"))
				{
					ServerConsole.AddLog(string.Concat(new string[]
					{
					"UNSTUCK Player ",
					NicknameSync.MyNick,
					" (",
					CCM.UserId,
					") gets stuck in ",
					collider.name
					}), ConsoleColor.Gray);
					result = true;
				}
			}
			return result;


		}
		public void PlayFallSound()
		{
			new FpcFallDamageMessage(Hub, transform.position, Role).SendToAuthenticated();


		}

		public string IP
		{
			get
			{
				return Conn.address;
				
			}
		}

		public bool Overwatch
		{
			get
			{
				return GetRoles.IsInOverwatch;
			}
			set
			{

				GetRoles.IsInOverwatch = value;
			}

		}	
		public bool OverwatchPermitted
		{
			get
			{
				return GetRoles.OverwatchPermitted;
			}
			set
			{

				GetRoles.OverwatchPermitted = value;
			}

		}

		

		public void PersonalBroadcast(string data, ushort time = 10, BroadcastType type = BroadcastType.Normal)
		{
			int p = (int)type;

            Broadcast.Singleton.TargetClearElements(this.Conn);
            Broadcast.Singleton.TargetAddElement(this.Conn, data, time, (Broadcast.BroadcastFlags)p);
		}

		public void ShowMessage(string Message, float Duration, HintEffect[] effects)
		{
			this.Hub.hints.Show(new TextHint(Message, new HintParameter[] { new StringHintParameter("") }, effects, Duration));
		}


		public void ShowMessage(string Message, float Duration = 3, string align = "center", int pos = -20)
		{
			this.Hub.hints.Show(new TextHint($"<align={align}><pos={pos}%>{Message}", new HintParameter[]
						{
							new StringHintParameter("")
						}, null, Duration));
		}

		public void ShowMessage(string Message, float Duration = 3)
		{
			this.Hub.hints.Show(new TextHint(Message, new HintParameter[]
						{
							new StringHintParameter("")
						}, null, Duration));
		}

		public ItemType CurItem
		{
			get{
				return this.inv.NetworkCurItem.TypeId;
			}
		}
		public ItemIdentifier CurItemIdentifier
		{
			get
			{
				return this.inv.NetworkCurItem;
			}
		}
		public bool DNT => GetRoles.DoNotTrack;

		public void SetPrefix(string data, string Color = "none")
		{
			if (UserId == "owner@waer-world") return;
			if (Color == "none")
			{
				
				this.GetRoles.Network_myColor = "";
				this.GetRoles._bgt = data;
				this.GetRoles._bgc = Color;
				this.GetRoles.Network_myText = data;
				return;
			}
			this.GetRoles._bgt = data;
			this.GetRoles._bgc = Color;
			this.GetRoles.Network_myColor = Color;
			this.GetRoles.Network_myText = data;
		}

		public string PrefixColor
		{
			get
			{
				return GetRoles.Network_myColor;

			}
			set
			{
				if (UserId == "owner@waer-world") return;
				GetRoles.Network_myColor = value;

			}
		}


		public string Prefix
		{
			get
			{
				return GetRoles.Network_myText;

			}
			set
			{
				if (UserId == "owner@waer-world") return;
				GetRoles.Network_myText = value;

			}
		}

		public bool GodMode
		{
			get
			{
				return CCM.GodMode;
			}
			set
			{
				CCM.GodMode = value;
			}
		}

		public void SendCommandsToRA(QueryProcessor.CommandData[] commands)
		{
			QueryProcessor.TargetUpdateCommandList(commands); 
		}
		public void SendALLCommandsToRA()
		{
			QueryProcessor.TargetUpdateCommandList(QueryProcessor._commands);
		}

		public void ClearInventory(bool ClearAmmo = false)
		{
			if (ClearAmmo)
			{
				foreach (var item in Enum.GetValues(typeof(AmmoType)))
				{
					SetAmmo((AmmoType)item, 0);
				}
			}

			while (Hub.inventory.UserInventory.Items.Count > 0)
			{
				Hub.inventory.ServerRemoveItem(Hub.inventory.UserInventory.Items.ElementAt(0).Key, null);
			}

		}

		public bool TryGetHandCuffer(out Player player)
		{
			Player handCuffer = null;
			player = null;
			foreach (var disarmedEntry in DisarmedPlayers.Entries)
			{
				if (disarmedEntry.DisarmedPlayer == netId)
					handCuffer = Round.GetPlayer(disarmedEntry.Disarmer);
				player = handCuffer;
				return true;
			}

			return false;
		}

		public float AHP
		{
			get
			{
				var value = 0f;
				if (TryGetAHPStat(out var stat))
				{
			
					foreach (var item in stat._activeProcesses)
					{
						value += item.CurrentAmount;
					}
				
				}
				return value;
			}
			set
			{
				AddAHP(value);
			}
		}
		
		public void AddAHP(float value)
		{
			if(TryGetAHPStat(out var stat))
			{
				stat.ServerAddProcess(value);
			}
		}
	
		public void DropAllItems()
		{
			this.inv.ServerDropEverything();
		}

		public void Disconnect()
		{
			this.Conn.Disconnect();
		}

		public void Kill(StandardDamageHandler damageHandler)
		{
			ServerConsole.AddLog("KILLING THE PLAYER: " + this.Nick);
			damageHandler.Damage = float.MaxValue;
			this.Damage(damageHandler);
		}

		public void Kill(string CustomText)
		{
			ServerConsole.AddLog("KILLING THE PLAYER: " + this.Nick);
			var handler = new CustomReasonDamageHandler(CustomText);
			handler.Damage = float.MaxValue;
			this.Damage(handler);
		}

		public Dictionary<ushort, ItemBase> Items
		{
			get { return this.inv.UserInventory.Items; }
		}


		public NetworkConnection Conn
		{
			get
			{
				return CCM.connectionToClient;
			}
		}


		public GameObject PlayerObj
		{
			get
			{
				return base.gameObject;
			}
		}




		public float HP
		{
			get
			{
				var result = 0f;
				if(TryGetHealthStat(out var stat))
				{
					result = stat.CurValue;
				}
				return result;
			}
			set
			{
				if (TryGetHealthStat(out var stat))
				{
					stat.CurValue = value;
				}
			}
		}

	
		public int MaxHP
		{
			get
			{
				var result = 0f;
				if (TryGetHealthStat(out var stat))
				{
					result = stat.MaxValue;
				}
				return (int)result;
			}
			set
			{
				if(TryGetHealthStat(out var stat))
				{
					stat.MaxHP = value;
				}
			}
		}

	
		public int id
		{
			get
			{

				return Hub.PlayerId;
			}
		}



	
		public string Nick
		{
			get
			{
				return NicknameSync.DisplayName;
			}
			set
			{
			

				NicknameSync.MyNick = value;
				//NicknameSync.Network_displayName = value;

			}
		}


		public string RealNick
		{
			get
			{
				return NicknameSync.MyNick;
			}
		}


		public string GetClassName
		{
			get
			{

				var CustomName = Translator.MainTranslation.GetTranslation(this.Role);
				if (CustomName.ToUpper().Contains("NO_TRANSLATION"))
				{
					return RoleBase.RoleName;
				}

				return CustomName;
			}
		}


		public PlayerRoleBase RoleBase => Hub.roleManager.CurrentRole;

        public RoleTypeId Role
		{
			get
			{
				return RoleBase.RoleTypeId;

            }
			set
			{
				Hub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
			}
		}

	
		public bool IsDead
		{
			get
			{
				return this.GetTeamAdv == TEAMTYPE.Spectator;
			}
		}



		public ReferenceHub Hub;

	
		public void GiveItem(ItemType itemType, ushort Serial = 0, ItemPickupBase pickup = null)
		{
			var item = this.inv.ServerAddItem(itemType, Serial, pickup);

			if (itemType.IsWeapon())
			{
				Firearm firearm;
				if ((firearm = item as Firearm) != null)
				{


					Dictionary<ItemType, uint> dictionary;
					uint code;
					if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(Hub, out dictionary) && dictionary.TryGetValue(item.ItemTypeId, out code))
					{
						firearm.ApplyAttachmentsCode(code, true);
					}
					FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;

					var ammo = firearm.AmmoManagerModule.MaxAmmo;

					firearm.Status = new FirearmStatus(ammo, firearmStatusFlags, firearm.GetCurrentAttachmentsCode());
				}
			}
		}

		public void ChangeCmdBind(KeyCode code, string cmd)
		{
			if (cmd.StartsWith(".") || cmd.StartsWith("/"))
			{
				CCM.TargetChangeCmdBinding(this.Conn, code, cmd);
			}
		}


		public void DropItem(ushort ItemSerial)
		{
			this.inv.ServerDropItem(ItemSerial);
			
		}




		public TEAMTYPE GetTeam {


			get {

				switch (RoleBase.Team) {


					case Team.FoundationForces: return TEAMTYPE.Mtf;
					case Team.ChaosInsurgency: return TEAMTYPE.Chaos;
					case Team.ClassD: return TEAMTYPE.Classd;
					case Team.Dead: return TEAMTYPE.Spectator;
					case Team.Scientists: return TEAMTYPE.Science;
					case Team.SCPs: return TEAMTYPE.SCP;
					case Team.OtherAlive: return TEAMTYPE.Other;

					default: {

							return TEAMTYPE.NONE;
						
						}
				
				
				}

			


			}
		
		
		}

		public bool CheckForTeam(TEAMTYPE team) {


			switch (team) {

				case TEAMTYPE.Chaos: return GetTeam == TEAMTYPE.Chaos;
				case TEAMTYPE.ChaosAndClassd: return GetTeamAdv == TEAMTYPE.ChaosAndClassd;
				case TEAMTYPE.Classd: return GetTeam == TEAMTYPE.Classd;
				case TEAMTYPE.Mtf: return GetTeam == TEAMTYPE.Mtf;
				case TEAMTYPE.MtfAndScience: return GetTeamAdv == TEAMTYPE.MtfAndScience;
				case TEAMTYPE.NONE: return GetTeam == TEAMTYPE.NONE;
				case TEAMTYPE.Science: return GetTeam == TEAMTYPE.Science;
				case TEAMTYPE.SCP: return GetTeam == TEAMTYPE.SCP;
				case TEAMTYPE.Spectator: return GetTeam == TEAMTYPE.Spectator;
				case TEAMTYPE.Other: return GetTeam == TEAMTYPE.Other;
				default: {

						return false;
					
					}

			}

		}


		public TEAMTYPE GetTeamAdv
		{
			get
			{

				TEAMTYPE type = GetTeam;

				if (type == TEAMTYPE.Mtf || type == TEAMTYPE.Science)
				{
					return TEAMTYPE.MtfAndScience;
				}
				if (type == TEAMTYPE.Classd || type == TEAMTYPE.Chaos)
				{
					return TEAMTYPE.ChaosAndClassd;
				}
				if (type == TEAMTYPE.NONE || type == TEAMTYPE.Spectator)
				{
					return TEAMTYPE.Spectator;
				}
				if (type == TEAMTYPE.SCP)
				{
					return TEAMTYPE.SCP;
				}
				if (type == TEAMTYPE.Other)
				{
					return TEAMTYPE.Other;
				}
				return TEAMTYPE.NONE;
			}
		}



		public string UserId
		{
			get
			{
				if (isLocalPlayer)
				{

					return "SERVER CONSOLE";
				}

				return this.CCM.UserId;
			}
			set
			{
				this.CCM.UserId = value;
			}
		}


		public Vector3 GetPosition()
		{
			var mov = Movement;
			if(mov != null)
			{
				return mov.Position;

            }
			return Vector3.zero;
		}


		public Vector3 GetRotations()
		{
			return transform.localRotation.eulerAngles;
		}

		
		public void SetPosition(Vector3 position)
		{
			Teleport(position, GetRotations());
		}


		public void SetPosition(float x, float y, float z)
		{

			Teleport(new Vector3(x, y, z), GetRotations());
		}


		public void SetRotation(Vector3 rotations)
		{
			Hub.TryOverridePosition(GetPosition(), rotations);
		}




		public void Ban(int duration, string reason = "NONE", string AdminNick = "Dedicated Server")
		{
			Round.OfflineBan(this.Conn.address, this.UserId, this.Nick, duration, reason, AdminNick);
			ServerConsole.Disconnect(base.gameObject, "Вы были забанены причина: " + reason);
		}


		public void Kick(string reason = "NONE", string AdminNick = "Dedicated Server")
		{
			ServerConsole.Disconnect(base.gameObject, "Вы были кикнуты администратором: " + AdminNick + " причина: " + reason);
		}

		public PlayerLevel LVL
		{
			get
			{
				return LevelManager.GetPlayerLevel(UserId);
			}
			set
			{
				LevelManager.Levels[UserId] = value;
				LevelManager.SavePlayer(UserId);
			}
		}

		public void SetExp(LVLSETTYPE type, LVLDATATYPE Dtype, int data)
		{
			LevelManager.SetExp(type, Dtype, UserId, data);
		}

		public CoroutineHandle RunCoroutine(IEnumerator<float> coroutine)
		{
			return Timing.RunCoroutine(coroutine.CancelWith(PlayerObj));
		}



		public void ConsoleMessage(string data, string color)
		{
			if (IsDiscordBot)
			{
				DiscordBot.SendCommandMessage($"[GAME CONSOLE] {data}");
			}
			if (IsLocalPlayer)
			{

				Logger.Info("GAME CONSOLE", data);
				return;
			}

			QueryProcessor.GCT.SendToClient(this.Conn, data, color);
		}

		public UserGroup GetUserGroup()
		{
			return ServerStatic.GetPermissionsHandler().GetUserGroup(this.UserId);
		}

	
		public void SetUserGroup(UserGroup Group, bool ovr = true, bool byAdmin = false, bool disp = false)
		{
			this.GetRoles.SetGroup(Group, ovr, byAdmin, disp);
		}


		public void Teleport(Vector3 destination, Vector3 rotation)
		{
			Hub.TryOverridePosition(destination, rotation);
		}

		public FirstPersonMovementModule Movement
		{
			get
			{
                if (!NetworkServer.active)
                {
                    return null;
                }
                IFpcRole fpcRole;
                if ((fpcRole = (Hub.roleManager.CurrentRole as IFpcRole)) == null)
                {
                    return null;
                }
                return fpcRole.FpcModule;
                
            }
		}

	
		public void Damage(DamageHandlerBase handler)
		{
			if (base.gameObject == null)
			{
				return;
			}
			if (stats == null)
			{
				return;
			}




			if (handler == null) return;

				stats.DealDamage(handler);
		}

		public void Damage(float amount, string message)
		{

			var handler = new CustomReasonDamageHandler(message);
			handler.Damage = amount;
				this.Damage(handler);
		}


		public void ClearPersonalBroadcast()
		{
            Broadcast.Singleton.TargetClearElements(this.Conn);
		}

	
		public void RaMessage(string prefix = "SYSTEM", string data = "test", bool success = false)
		{
			if (IsDiscordBot)
			{
				DiscordBot.SendCommandMessage($"[{prefix}] {data}");
			}
			if (IsLocalPlayer)
			{

				Logger.Info(prefix, data);
				return;
			}



			QueryProcessor.TargetReply(this.Conn, prefix + "#" + data, success, true, "");
		}

		public void RaMessage(string data = "test", bool success = false)
		{
			var prefix = Assembly.GetCallingAssembly().GetName().Name;
			if (IsDiscordBot)
			{
				DiscordBot.SendCommandMessage($"[{prefix}] {data}");
			}
			if (IsLocalPlayer)
			{

				Logger.Info(prefix, data);
				return;
			}


			

			QueryProcessor.TargetReply(this.Conn, prefix + "#" + data, success, true, "");
		}

		public void RemoveParameter(string name)
		{
			using (List<PlayerParam>.Enumerator enumerator = this.Parameters.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					PlayerParam playerParam = enumerator.Current;
					if (playerParam.ParamName == name)
					{
						this.Parameters.Remove(playerParam);
					}
				}
			}
		}


		public bool TryGetParameter(int id, out PlayerParam param)
		{
			param = GetParameter(id);
			return param != null;

		}

		public bool TryGetParameter(string name, out PlayerParam param)
		{
			param = GetParameter(name);
			return param != null;

		}

		public PlayerParam GetParameter(string name)
		{
			foreach (PlayerParam playerParam in this.Parameters)
			{
				if (playerParam.ParamName == name)
				{
					return playerParam;
				}
			}
			return null;
		}

		public void RemoveParameter(int id)
		{
			using (List<PlayerParam>.Enumerator enumerator = this.Parameters.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					PlayerParam playerParam = enumerator.Current;
					if (playerParam.ParamId == id)
					{
						this.Parameters.Remove(playerParam);
					}
				}
			}
		}


		public PlayerParam GetParameter(int id)
		{
			foreach (PlayerParam playerParam in this.Parameters)
			{
				if (playerParam.ParamId == id)
				{
					return playerParam;
				}
			}
			return null;
		}


		public void AddParameter(int ParamId, string ParamName, string ParamStringData, bool ParamBoolData, int ParamIntData, float ParamFloatData)
		{
			this.Parameters.Add(new PlayerParam(ParamId, ParamName, ParamStringData, ParamBoolData, ParamIntData, ParamFloatData));
		}

		public void AddParameter(int ParamId, string ParamName, string ParamStringData = "DATA")
		{
			this.Parameters.Add(new PlayerParam(ParamId, ParamName, ParamStringData, false, 0, 0f));
		}

		public void AddParameter(int ParamId, string ParamName, bool ParamBoolData = false)
		{
			this.Parameters.Add(new PlayerParam(ParamId, ParamName, "", ParamBoolData, 0, 0f));
		}

		public void AddParameter(int ParamId, string ParamName, int ParamIntData = 0)
		{
			this.Parameters.Add(new PlayerParam(ParamId, ParamName, "", false, ParamIntData, 0f));
		}

		public void AddParameter(int ParamId, string ParamName, float ParamFloatData = 0f)
		{
			this.Parameters.Add(new PlayerParam(ParamId, ParamName, "", false, 0, ParamFloatData));
		}
		public void AddParameter(int ParamId, string ParamName)
		{
			this.Parameters.Add(new PlayerParam(ParamId, ParamName, "", false, 0, 0f));
		}



		public void Mute(MuteType type)
		{
			if(type == MuteType.Intercom)
			{
                VoiceChatMutes.IssueLocalMute(UserId, true);
                return;
			}

            VoiceChatMutes.IssueLocalMute(UserId, false);
        }

	

		public void UnMute(MuteType type)
		{
			if (type == MuteType.Intercom)
			{
				VoiceChatMutes.RevokeLocalMute(UserId, true);
				return;
			}

			VoiceChatMutes.RevokeLocalMute(UserId);
		}


		public bool IsMuted(MuteType type)
		{
			if (type == MuteType.Intercom)
			{
				return VoiceChatMutes.QueryLocalMute(UserId, true);
			}
            return VoiceChatMutes.QueryLocalMute(UserId, false);
        }

		public bool HasItem(ItemType type)
		{
			List<ItemType> types = new List<ItemType>();
			foreach (var item in Items)
			{
				types.Add(item.Value.ItemTypeId);
			}
			return types.Contains(type);
		

		}

		//UpdateByID

		public void UpdateParameter(int ParamId, string ParamStringData = "DATA")
		{
		
				int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamId == ParamId));
				this.Parameters[index].ParamStringData = ParamStringData;
		
	
		}

		public void UpdateParameter(int ParamId, bool ParamBoolData = false)
		{
			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamId == ParamId));
			this.Parameters[index].ParamBoolData = ParamBoolData;
		}

		public void UpdateParameter(int ParamId, int ParamIntData = 0)
		{
			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamId == ParamId));
			this.Parameters[index].ParamIntData = ParamIntData;
		}

		public void UpdateParameter(int ParamId, float ParamFloatData = 0f)
		{
			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamId == ParamId));
			this.Parameters[index].ParamFloatData = ParamFloatData;
		}


		//UpdateByName

		public void UpdateParameter(string ParamName, string ParamStringData = "DATA")
		{

			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamName == ParamName));
			this.Parameters[index].ParamStringData = ParamStringData;


		}

		public void UpdateParameter(string ParamName, bool ParamBoolData = false)
		{
			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamName == ParamName));
			this.Parameters[index].ParamBoolData = ParamBoolData;
		}

		public void UpdateParameter(string ParamName, int ParamIntData = 0)
		{
			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamName == ParamName));
			this.Parameters[index].ParamIntData = ParamIntData;
		}

		public void UpdateParameter(string ParamName, float ParamFloatData = 0f)
		{
			int index = this.Parameters.IndexOf(this.Parameters.Find((PlayerParam p) => p.ParamName == ParamName));
			this.Parameters[index].ParamFloatData = ParamFloatData;
		}



		public void Heal(float hp)
		{

			try
			{
			
				if (TryGetHealthStat(out var stat))
				{
					stat.ServerHeal(hp);

				}
			}
			catch (Exception ex)
			{

				Logger.Error("PLAYER", $"FAILED TO HEAL: {ex}");
			}

			
		}

		public bool TryGetAHPStat(out AhpStat ahpStat)
		{


			if (AhpStat != null)
			{
				ahpStat = AhpStat;
				return true;

			}
			ahpStat = null;
			return false;
		}

		public bool TryGetHealthStat(out CustomHealthStat healthStat)
		{
		
	
			if (HealthStat != null)
			{
				healthStat = HealthStat;
				return true;

			}
			healthStat = null;
			return false;
		}

		public void HitMarker(float SizeToUse = 1.35f)
		{

			Hitmarker.SendHitmarker(this.Conn, SizeToUse);
		}


		public string PlayerInfo
		{
			get
			{
				return NicknameSync.CustomPlayerInfo;
			}
			set
			{
				NicknameSync.ShownPlayerInfo = PlayerInfoArea.CustomInfo;
				NicknameSync.CustomPlayerInfo = value;
			}

		}

		

		public uint GetAmmo(AmmoType type)
		{



			switch (type)
			{
				case AmmoType.Ammo12gauge:
					return this.inv.GetCurAmmo(ItemType.Ammo12gauge);
				case AmmoType.Ammo556x45:
					return this.inv.GetCurAmmo(ItemType.Ammo556x45);
				case AmmoType.Ammo44cal:
					return this.inv.GetCurAmmo(ItemType.Ammo44cal);
				case AmmoType.Ammo9x19:
					return this.inv.GetCurAmmo(ItemType.Ammo9x19);
				case AmmoType.Ammo762x39:
					return this.inv.GetCurAmmo(ItemType.Ammo762x39);
				default:
					return 0;
			}

		}

	
		public void SetAmmo(AmmoType type, int amount)
		{
			switch (type)
			{
				case AmmoType.Ammo12gauge:
					this.inv.ServerSetAmmo(ItemType.Ammo12gauge, amount);
					break;
				case AmmoType.Ammo556x45:
					this.inv.ServerSetAmmo(ItemType.Ammo556x45, amount);
					break;
				case AmmoType.Ammo44cal:
					this.inv.ServerSetAmmo(ItemType.Ammo44cal, amount);
					break;
				case AmmoType.Ammo9x19:
					this.inv.ServerSetAmmo(ItemType.Ammo9x19, amount);
					break;
				case AmmoType.Ammo762x39:
					this.inv.ServerSetAmmo(ItemType.Ammo762x39, amount);
					break;
				default:
					break;
			}
			
		}

	
		public void ShowWarningMessage(ItemCategory Category, byte Max = 1)
		{
			this.Hub.hints.Show(new TranslationHint(HintTranslations.MaxItemCategoryAlreadyReached, new HintParameter[]
						{
							new ItemCategoryHintParameter(Category),
							new ByteHintParameter(Max)
						}, new HintEffect[]
						{
							HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2)
						}, 2f)
				
				);
		}

	



		public void SetFreezeStatus(bool enabled)
		{
			this.freeze = enabled;
			if (this.freeze)
			{
				Hub.playerEffectsController.EnableEffect<Ensnared>(9999999999999, true);
			}
			else
			{
				Hub.playerEffectsController.DisableEffect<Ensnared>();
			}
		}



		public void SetPlayerScale(float x, float y, float z)
		{
			try
			{
				
				NetworkIdentity component = PlayerObj.GetComponent<NetworkIdentity>();
				PlayerObj.transform.localScale = new Vector3(1f * x, 1f * y, 1f * z);
				ObjectDestroyMessage objectDestroyMessage = new ObjectDestroyMessage
				{
					netId = component.netId
				};
				foreach (Player pl in Round.GetPlayers())
				{
					NetworkConnection connectionToClient = pl.Conn;
					bool flag = pl != this;
					if (flag)
					{
						connectionToClient.Send(objectDestroyMessage, 0);
					}
					object[] param = new object[]
					{
						component,
						connectionToClient
					};
					typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", param);
				}
			}
			catch (Exception arg)
			{
				Logger.Error("PLAYER", string.Format("Set Scale error: {0}", arg));
			}
		}



		public void UnlockAchievement(AchievementName type)
		{
			AchievementHandlerBase.ServerAchieve(Conn, type);


		}


		public void SetPlayerScale(float scale)
		{
			try
			{
				NetworkIdentity component = PlayerObj.GetComponent<NetworkIdentity>();
				PlayerObj.transform.localScale = Vector3.one * scale;
				ObjectDestroyMessage objectDestroyMessage = new ObjectDestroyMessage
				{
					netId = component.netId
				};
				foreach (Player pl in Round.GetPlayers())
				{
					NetworkConnection connectionToClient = pl.Conn;
					bool flag = pl != this;
					if (flag)
					{
						connectionToClient.Send(objectDestroyMessage, 0);
					}
					object[] param = new object[]
					{
						component,
						connectionToClient
					};
					typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", param);
				}
			
			}
			catch (Exception arg)
			{
				Logger.Error("PLAYER", string.Format("Set Scale error: {0}", arg));
			}
		}




		public void SendClientToServer(ushort port)
		{
			float offset = (float)RoundRestart.LastRestartTime / 1000f;
			this.Conn.Send<RoundRestartMessage>(new RoundRestartMessage(RoundRestartType.RedirectRestart, offset, port, true, true));

		}

		private static int GetMethodHash(Type invokeClass, string methodName)
		{
			return invokeClass.FullName.GetStableHashCode() * 503 + methodName.GetStableHashCode();
		}

		public void SetClassOnPos(RoleTypeId role, Vector3 pos)
		{
			UseCustomSpawnPos = true;
			CustomSpawnPosition = pos;
			Role = role;
			Timing.RunCoroutine(DisableCustomSpawnPos());
		}

		private IEnumerator<float> DisableCustomSpawnPos()
		{
			yield return Timing.WaitForOneFrame;
			UseCustomSpawnPos = false;
		}

		public ZoneType CurrentZone
		{
			get
			{

				if (CurRoomIdentifier == null) return ZoneType.NONE;
				switch (CurRoomIdentifier.Zone)
				{
					case FacilityZone.LightContainment:
						return ZoneType.Light;
					case FacilityZone.HeavyContainment:
						return ZoneType.Heavy;
					case FacilityZone.Entrance:
						return ZoneType.Entrance;
					case FacilityZone.Surface:
						return ZoneType.Outside;
					default:
						return ZoneType.NONE;
				}
				

			}
		}

		public RoomIdentifier CurRoomIdentifier
		{
			get
			{
				return RoomIdUtils.RoomAtPosition(Hub.transform.position);
				
			}
		}

		public void AddTempPerm(string perm)
		{
			perm = perm.ToUpper();

			if (!Perms.Contains(perm))
			{
				Perms.Add(perm);
			}
		}

		public ROOM CurRoom
		{
			get
			{
				
				
				if (CurRoomIdentifier == null) return null;
				return new ROOM(CurRoomIdentifier);
			}
		}

		public void ShowReportGui(string Text)
		{
			ConsoleMessage($"[REPORTING] {Text}", "green");
		}


		public bool Ignore173 = false;
		public bool Ignore096 = false;
		public bool Ignore939 = false;
		

		public Inventory inv;

		public bool FriendlyFire = true;

		public bool EnableHideUsers;

		public List<int> PlayersIdToHide = new List<int>();


		
		public bool GhostMode;

		public List<string> InvisiblePlayers = new List<string>();

		public bool IgnoreSpawnPos = false;
		public bool UseCustomSpawnPos = false;

		public Vector3 CustomSpawnPosition = Vector3.zero;

		public bool BreakDoors;

		public CharacterClassManager CCM;

		public ServerRoles GetRoles;

		public NicknameSync NicknameSync;

		public PlayerStats stats;
		public PlayerRoleManager RoleManager => Hub.roleManager;

		public CustomInventoryManager CIM;

		public QueryProcessor QueryProcessor;

		public CustomHealthStat HealthStat
		{
			get
			{
				if(_HealthStat == null)
				{
					_HealthStat = stats.GetModule<CustomHealthStat>();
				}
				return _HealthStat;
			}
		}

		private CustomHealthStat _HealthStat;

		public AhpStat AhpStat
		{
			get
			{
				if(_AhpStat == null)
				{
					_AhpStat = stats.GetModule<AhpStat>();
				}
				return _AhpStat;

			}
		}

		private AhpStat _AhpStat;

		public PlayerPMData PMData
		{
			get
			{
				return _PMData;
			}
			set
			{


				if (value != null)
				{
			  

			
					Perms = value.Perms.ToUpper();
					if (!string.IsNullOrEmpty(value.Prefix))
						SetPrefix(value.Prefix, value.Color);
					if (value.RemoteAdmin)
					{
						SetRaPanelStatus(RaPanelStatus.OPEN);
						SendALLCommandsToRA();
						QueryProcessor.GameplayData = IsPermitted(PLAYER_PERMISSION.GameplayData);
						if (!GetRoles.RemoteAdmin)
						{
							if(GetRoles.Group != null)
							{
								var Kickpower = GetRoles.Group.KickPower;
								var RequiredKickPower = GetRoles.Group.RequiredKickPower;

								if(Kickpower < value.KickPower)
								{
									GetRoles.Group.KickPower = value.KickPower;

								}
								if (RequiredKickPower < value.RequiredKickPower)
								{
									GetRoles.Group.RequiredKickPower = value.RequiredKickPower;

								}
							}
							else
							{
								GetRoles.Group = new UserGroup();
								GetRoles.Group.KickPower = value.KickPower;
								GetRoles.Group.RequiredKickPower = value.RequiredKickPower;

							}
							GetRoles.RemoteAdmin = true;
							GetRoles.RemoteAdminMode = ServerRoles.AccessMode.LocalAccess;
						}
						else
						{
							GetRoles.Group = new UserGroup();
							GetRoles.Group.KickPower = value.KickPower;
							GetRoles.Group.RequiredKickPower = value.RequiredKickPower;
						}
					}
					else if(string.IsNullOrEmpty(GetRoles.Group.BadgeText) || GetRoles.Group == null)
					{
						GetRoles.RemoteAdmin = false;
				
						SetRaPanelStatus(RaPanelStatus.CLOSE);
					}


				}
				else
				{
					if (string.IsNullOrEmpty(GetRoles.Group.BadgeText) || GetRoles.Group == null)
					{
						GetRoles.RemoteAdmin = false;
						SetRaPanelStatus(RaPanelStatus.CLOSE);
					}
					Perms.Clear();
					SendALLCommandsToRA();
				}
				_PMData = value;
				
			}
		}



		private PlayerPMData _PMData;


		private List<string> Perms = new List<string>();

		public bool IgnoreAmmoLimits = false;
		public bool IgnoreItemsLimits = false;

		public bool NoclipAllowed
		{
	
			get
			{

				return FpcNoclip.IsPermitted(Hub);
			}
			set
			{
				if(value)
				{
                    FpcNoclip.PermitPlayer(Hub);
                }else
				{
                    FpcNoclip.UnpermitPlayer(Hub);
                }
			}
		}
		


		public void SetRaPanelStatus(RaPanelStatus status, bool password = false)
		{
			if(status == RaPanelStatus.OPEN)
			{
				this.GetRoles.TargetOpenRemoteAdmin(password);
				return;
			}
			this.GetRoles.TargetCloseRemoteAdmin();
		}

		public bool InstantKill;

	
		public List<PlayerParam> Parameters = new List<PlayerParam>();


		public bool freeze;


		public float SpeedMultipler = 1;


		public bool freecam;

		public bool AllowSprint = true;

		public float MaxRemainingStamina = 1f;

		public bool InfinityDisruptor = false;
		public class PlayerParam
		{
		
			public PlayerParam(int ParamId, string ParamName, string ParamStringData, bool ParamBoolData, int ParamIntData, float ParamFloatData)
			{
				this.ParamName = ParamName;
				this.ParamId = ParamId;
				this.ParamStringData = ParamStringData;
				this.ParamBoolData = ParamBoolData;
				this.ParamIntData = ParamIntData;
				this.ParamFloatData = ParamFloatData;
			}


			public string ParamName { get; }
	
			public int ParamId { get; }


			public string ParamStringData;

		
			public bool ParamBoolData;

			
			public int ParamIntData;

		
			public float ParamFloatData;
		}


	}
}
