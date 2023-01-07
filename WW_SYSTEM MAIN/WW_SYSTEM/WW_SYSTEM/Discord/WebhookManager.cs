using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Utf8Json;

namespace WW_SYSTEM.Discord
{
	public static class WebhookManager
	{

		private static List<BufferData> buffer = new List<BufferData>();

		private static Dictionary<string, StacksData> Stacks = new Dictionary<string, StacksData>();

		public static int BufferLimit = 100;

		public static bool UseStacks = false;

		public static bool UseWWW = false;

		public static int StacksSize = 5;

		public static bool BufferIsEmpty => (buffer.Count <= 0);

		public static List<long> IgnoreErrorsList = new List<long>();

		public static void StartBufffer()
		{
			Timing.RunCoroutine(BufferProcess());
		}

		public static string GenerateWebhookUrl(ulong id, string token)
		{
			return $"https://discordapp.com/api/webhooks/{id}/{token}";

		}


		public static bool ClearBuffer()
		{
			try
			{
				
				buffer.Clear();
				Logger.Info("WebhookManager", $"BUFFER CLEAR DONE!");
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"BUFFER CLEAR ERROR: {ex}");
				return false;
			}
		
		}

	

		public static bool AddMsgToBHuffer(string Url, Webhook webhook)
		{
			try
			{
				if (UseStacks)
				{

					if (Stacks.ContainsKey(Url))
					{
						Webhook web = Stacks[Url].Webhook;
						string content = web.content + Environment.NewLine + webhook.content;
						string username = web.username;
						string avatar_url = web.avatar_url;
						bool tts = web.tts;
						List<Embed> embeds = web.embeds.ToList();
						for (int i = 0; i < webhook.embeds.Length; i++)
						{
							embeds.Add(webhook.embeds[i]);
						}
						Stacks[Url].Webhook = new Webhook(content, username, avatar_url, tts, embeds.ToArray());
						Stacks[Url].Count++;
						if(Stacks[Url].Count >= StacksSize)
						{
							if (buffer.Count >= BufferLimit)
							{
								Logger.Warn("WebhookManager", $"BUFFER IS FULL ({buffer.Count}/{BufferLimit}) CLEARING!");
								if (!ClearBuffer())
								{
									return false;
								}
							}
							buffer.Add(new BufferData(Url, Stacks[Url].Webhook));
							Stacks.Remove(Url);
						}
					}
					else
					{
						Stacks.Add(Url, new StacksData(1, webhook));

					}
				}
				else
				{
					if (buffer.Count >= BufferLimit)
					{
						Logger.Warn("WebhookManager", $"BUFFER IS FULL ({buffer.Count}/{BufferLimit}) CLEARING!");
						if (!ClearBuffer())
						{
							return false;
						}
					}
					buffer.Add(new BufferData(Url, webhook));
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"BUFFER ERROR: {ex}");
				return false;
			}
		}

		public static bool SendMessage(string WebhookUrl, string Name, string AvatarUrl, string data)
		{
			bool result;
			try
			{
				result = AddMsgToBHuffer(WebhookUrl, new Webhook(data, Name, AvatarUrl, false, new Embed[] { }));

				
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"WEBHOOK ERROR: {ex.Message}");
				result = false;
			}
			return result;
		}

		public static bool SendMessage(string WebhookUrl, string Name, string AvatarUrl, string title, string description, int color, EmbedField[] fields, string ImageUrl, string ThumbnailUrl)
		{
			bool result;
			try
			{
				
				result = AddMsgToBHuffer(WebhookUrl, new Webhook(string.Empty, Name, AvatarUrl, false, new Embed[] { new Embed(title, "rich", description, color, fields, new EmbedImage(ImageUrl), new EmbedImage(ThumbnailUrl)) }));
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"WEBHOOK ERROR: {ex.Message}");
				result = false;
			}
			return result;
		}

		public static bool SendMessage(string WebhookUrl, string Name, string AvatarUrl, string title, string description, int color, EmbedField[] fields)
		{
			bool result;
			try
			{

				result = AddMsgToBHuffer(WebhookUrl, new Webhook(string.Empty, Name, AvatarUrl, false, new Embed[] { new Embed(title, "rich", description, color, fields, new EmbedImage(string.Empty), new EmbedImage(string.Empty)) }));
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"WEBHOOK ERROR: {ex.Message}");
				result = false;
			}
			return result;
		}

		public static bool SendMessage(string WebhookUrl, string data)
		{
			bool result;
			try
			{

				result = AddMsgToBHuffer(WebhookUrl, new Webhook(data, string.Empty, string.Empty, false, new Embed[] { }));
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"WEBHOOK ERROR: {ex.Message}");
				result = false;
			}
			return result;
		}

		public static bool SendMessage(string WebhookUrl, string title, string description, int color, EmbedField[] fields, string ImageUrl, string ThumbnailUrl)
		{
			bool result;
			try
			{

				result = AddMsgToBHuffer(WebhookUrl, new Webhook(string.Empty, string.Empty, string.Empty, false, new Embed[] { new Embed(title, "rich", description, color, fields, new EmbedImage(ImageUrl), new EmbedImage(ThumbnailUrl)) }));
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"WEBHOOK ERROR: {ex.Message}");
				result = false;
			}
			
			return result;
		}
		public static bool SendMessage(string WebhookUrl, string title, string description, int color, EmbedField[] fields)
		{
			bool result;
			try
			{

				result = AddMsgToBHuffer(WebhookUrl, new Webhook(string.Empty, string.Empty, string.Empty, false, new Embed[] { new Embed(title, "rich", description, color, fields, new EmbedImage(string.Empty), new EmbedImage(string.Empty)) }));
			}
			catch (Exception ex)
			{
				Logger.Error("WebhookManager", $"WEBHOOK ERROR: {ex.Message}");
				result = false;
			}

			return result;
		}
		static IEnumerator<float> ClearStacksProcess()
		{
			
			Logger.Info("WebhookManager", "STACKS CLEAR SYSTEM ONLINE!");
			while (true)
			{
				yield return Timing.WaitForSeconds(15f);
				try
				{
					Dictionary<string, StacksData> TempStacks = new Dictionary<string, StacksData>();
					foreach (var item in Stacks)
					{
						TempStacks.Add(item.Key, item.Value);

					}
					foreach (var item in TempStacks)
					{
						if (buffer.Count >= BufferLimit)
						{
							Logger.Warn("WebhookManager", $"BUFFER IS FULL ({buffer.Count}/{BufferLimit}) CLEARING!");
							ClearBuffer();
						}
						buffer.Add(new BufferData(item.Key, item.Value.Webhook));
						Logger.Info("WebhookManager", $"CLEAR NOT FULL STACK DATA: {item.Key}!");
						Stacks.Remove(item.Key);
					}
				}
				catch (Exception ex)
				{

					Logger.Error("WebhookManager", $"FAILED CLEAN STACKS: {ex}");
				}
			}
		
		}

		static IEnumerator<float> BufferProcess()
		{
				Timing.RunCoroutine(ClearStacksProcess());
			
			while (true)
			{
				yield return Timing.WaitForSeconds(3f);
				if (!BufferIsEmpty)
				{

					if(buffer.Count < 5)
					{
							for (int i = 0; i < buffer.Count; i++)
							{
							yield return Timing.WaitForSeconds(0.2f);
							string url = buffer[0].Url;
								string data = "payload_json=" + JsonSerializer.ToJsonString<Webhook>(buffer[0].Webhook);

							if (UseWWW)
							{
#pragma warning disable CS0618 // Type or member is obsolete
								WWW www = new WWW(url, HttpQuery.ToUnityForm(data));
#pragma warning restore CS0618 // Type or member is obsolete
								yield return Timing.WaitUntilDone(www);
								if (!string.IsNullOrEmpty(www.error))
								{
									Logger.Error("WebhookManager", $"WEEBHOOK SEND FAILED WEEBHOOK: {buffer[0].Url} REASON: {www.error} RETRY..");
									break;
								}
							}
							else
							{
								using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, HttpQuery.ToUnityForm(data)))
								{
									UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
									while (!unityWebRequestAsyncOperation.isDone)
									{
										yield return Timing.WaitForSeconds(0.01f);
									}
									if (unityWebRequestAsyncOperation.webRequest.responseCode != 204)
									{
										
										if (IgnoreErrorsList.Contains(unityWebRequestAsyncOperation.webRequest.responseCode))
										{
											Logger.Error("WebhookManager", $"WEEBHOOK SEND FAILED WEEBHOOK: {buffer[0].Url} REASON: {unityWebRequestAsyncOperation.webRequest.error} CODE: {unityWebRequestAsyncOperation.webRequest.responseCode}");
										}
										else
										{
											Logger.Error("WebhookManager", $"WEEBHOOK SEND FAILED WEEBHOOK: {buffer[0].Url} REASON: {unityWebRequestAsyncOperation.webRequest.error} CODE: {unityWebRequestAsyncOperation.webRequest.responseCode} RETRY..");
											break;
										}

									}


								}


							}

								buffer.RemoveAt(0);
							}
						
					}
					else
					{
						for (int i = 0; i < 5; i++)
						{
							yield return Timing.WaitForSeconds(0.2f);
							string url = buffer[0].Url;
							string data = "payload_json=" + JsonSerializer.ToJsonString<Webhook>(buffer[0].Webhook);

							if (UseWWW)
							{
#pragma warning disable CS0618 // Type or member is obsolete
								WWW www = new WWW(url, HttpQuery.ToUnityForm(data));
#pragma warning restore CS0618 // Type or member is obsolete
								yield return Timing.WaitUntilDone(www);
								if (!string.IsNullOrEmpty(www.error))
								{
									Logger.Error("WebhookManager", $"WEEBHOOK SEND FAILED WEEBHOOK: {buffer[0].Url} REASON: {www.error} RETRY..");
									break;
								}
							}
							else
							{
								using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, HttpQuery.ToUnityForm(data)))
								{
									UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
									while (!unityWebRequestAsyncOperation.isDone)
									{
										yield return Timing.WaitForSeconds(0.01f);
									}
									if (unityWebRequestAsyncOperation.webRequest.responseCode != 204)
									{
										break;
									}

								}
							}


							buffer.RemoveAt(0);
						}
					}

					
					
				}
			}
		}
	}
}
