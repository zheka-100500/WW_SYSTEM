using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.Events;
using WW_SYSTEM.EventHandlers;
using VoiceChat;
using PlayerRoles.Voice;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.IssueLocalMute))]
	public class PlayerMuteEventPatch
    {
		public static bool Prefix(string userId, bool intercom = false)
		{

			try
			{
				string text;
                if (!VoiceChatMutes.TryValidateId(userId, intercom, out text))
                {
                    return false;
                }
                PlayerMuteEvent ev;

				if (intercom)
                {
					ev = new PlayerMuteEvent(userId, MuteType.Intercom, false, true);

                }
                else
                {
					ev = new PlayerMuteEvent(userId, MuteType.Voice, false, true);
				}
				EventManager.Manager.HandleEvent<IEventHandlerPlayerMute>(ev);

				if (!ev.Allow) return false;

                if (!VoiceChatMutes.Mutes.Add(text))
                {
                    return false;
                }
                File.AppendAllText(VoiceChatMutes._path, "\r\n" + text);
                ReferenceHub hub;
                if (!VoiceChatMutes.TryGetHub(userId, out hub))
                {
                    return false;
                }
                VoiceChatMutes.SetFlags(hub, VoiceChatMutes.GetFlags(hub) | VoiceChatMutes.GetLocalFlag(intercom));

                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerMuteEventPatch error: {0}", arg));
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.RevokeLocalMute))]
	public class PlayerMuteEventPatch1
	{
		public static bool Prefix(string userId, bool intercom = false)
		{

			try
			{
                string item;
                if (!VoiceChatMutes.TryValidateId(userId, intercom, out item))
                {
                    return false;
                }
                PlayerMuteEvent ev;

				if (intercom)
				{
					ev = new PlayerMuteEvent(userId, MuteType.Intercom, true, true);

				}
				else
				{
					ev = new PlayerMuteEvent(userId, MuteType.Voice, true, true);
				}
				EventManager.Manager.HandleEvent<IEventHandlerPlayerMute>(ev);

				if (!ev.Allow) return false;

                if (!VoiceChatMutes.Mutes.Remove(item))
                {
                    return false;
                }
                FileManager.WriteToFile(VoiceChatMutes.Mutes, VoiceChatMutes._path, false);
                ReferenceHub hub;
                if (!VoiceChatMutes.TryGetHub(userId, out hub))
                {
                    return false;
                }
                VoiceChatMutes.SetFlags(hub, VoiceChatMutes.GetFlags(hub) & ~VoiceChatMutes.GetLocalFlag(intercom));
                return false;
			}
			catch (Exception arg)
			{


				Logger.Error("[EVENT MANAGER]", string.Format("PlayerMuteEventPatch error: {0}", arg));
			}
			return true;
		}
	}
}
