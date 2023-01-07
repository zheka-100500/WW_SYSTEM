using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEC;
using WW_SYSTEM.Discord.BOT;
using UnityEngine;
using RemoteAdmin;
using WW_SYSTEM.Events;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Discord
{
	public static class DiscordBot
	{
		private static TcpClient socketConnection;

		private static Thread clientReceiveThread;

		public static int Port = 1234;
		public static string SecretKey = "SecretKey228";

		public static bool UseBot { get; private set; }

		public static List<Packet> Buffer = new List<Packet>();

		public static int BufferLimit = 100;

		public static float BufferSendTime = 0.3f;

		private static void AddLog(string data)
		{
			Logger.Info("BOT",data);
		}

		private static Dictionary<string, Packet> RecivedDatas = new Dictionary<string, Packet>();

		private static void SendDataRequest(string Hash, int DataType, List<string> Parameters)
		{
			Packet p = new Packet();
			p.Write(SecretKey);
			p.Write((int)DiscordBotMsgType.DATA);

			p.Write(Hash);
			p.Write(DataType);
			p.Write(Parameters);
			SendMessageToTcp(p);
		}

		

		struct RecideDataStruct
		{
			internal int DataType;
			internal List<string> Parameters;
			internal float WaitTime;
			internal Action<DiscordUser> OnRequestDone;
			internal Action<string> OnError;
	
		}

		static void StartReciveDataProcess(RecideDataStruct recideData)
		{
			var Thead = new Thread(ReciveDataProcess);
			Thead.Start(recideData);

		}



		static void ReciveDataProcess(object obj)
		{

			RecideDataStruct ds = (RecideDataStruct)obj;

			float TimeLeft = ds.WaitTime;
			
			string Hash = UnityEngine.Time.deltaTime.GetHashCode().ToString() + ds.DataType.GetHashCode() + Round.RoundTimeSeconds.GetHashCode();

			foreach (var item in ds.Parameters)
			{
				Hash += item.GetHashCode().ToString();

			}
			SendDataRequest(Hash, ds.DataType, ds.Parameters);

			while (true)
			{
				Thread.Sleep(10);
				TimeLeft -= 0.01f;

				if (RecivedDatas.ContainsKey(Hash))
				{
				  var user = new DiscordUser(RecivedDatas[Hash]);
					ds.OnRequestDone.Invoke(user);
					RecivedDatas.Remove(Hash);
					break;
				}

				if(TimeLeft <= 0)
				{
					ds.OnError.Invoke("NO RESPONSE RECEIVED!");
					break;
				}
			}
			

		}

		public static void RequestUserInfo(Action<DiscordUser> OnRequestDone, Action<string> OnError, ulong Id, float WaitTime = 5f)
		{
			var ds = new RecideDataStruct();
			ds.OnRequestDone = OnRequestDone;
			ds.OnError = OnError;
			ds.DataType = 0;
			ds.Parameters = new List<string>() { $"{Id}" };
			ds.WaitTime = WaitTime;
			StartReciveDataProcess(ds);

		}
		

		public static void SetStatus(string Status, DiscordStatus status = DiscordStatus.Online)
		{
		

			//BotStatus botStatus = new BotStatus(Status, (int)status);
			//string data = JsonUtility.ToJson(botStatus);

		//	DiscordBotMsg msg = new DiscordBotMsg(SecretKey, data, DiscordBotMsgType.STATUS);
			//var databytes = Utf8.GetBytes(JsonUtility.ToJson(msg));
			//data = Convert.ToBase64String(databytes);

			Packet p = new Packet();
			p.Write(SecretKey);
			p.Write((int)DiscordBotMsgType.STATUS);
			
			p.Write(Status);
			p.Write((int)status);
			Buffer.Add(p);

		}

		public static void SendCommandMessage(string data)
		{
			if (Buffer.Count >= BufferLimit) Buffer.Clear();
		//	DiscordBotMsg msg = new DiscordBotMsg(SecretKey, data, DiscordBotMsgType.COMMAND_OUTPUT);
			//var databytes = Utf8.GetBytes(JsonUtility.ToJson(msg));
			//data = Convert.ToBase64String(databytes);

			Packet p = new Packet();
			p.Write(SecretKey);
			p.Write((int)DiscordBotMsgType.COMMAND_OUTPUT);
			p.Write(data);
			SendMessageToTcp(p);

		}

		public static void SendMessage(string data)
		{
			if (Buffer.Count >= BufferLimit) Buffer.Clear();
			//DiscordBotMsg msg = new DiscordBotMsg(SecretKey, data, DiscordBotMsgType.MESSAGE);
			//var databytes = Utf8.GetBytes(JsonUtility.ToJson(msg));
			//data = Convert.ToBase64String(databytes);

			Packet p = new Packet();
			p.Write(SecretKey);
			p.Write((int)DiscordBotMsgType.MESSAGE);
			p.Write(data);

			Buffer.Add(p);

		}

		public static void SendMessage(string data, ulong ChannelID, bool UseBuffer = true)
		{
			if (Buffer.Count >= BufferLimit) Buffer.Clear();


			Packet p = new Packet();
			p.Write(SecretKey);
			p.Write((int)DiscordBotMsgType.CHANNEL_MESSAGE);
			p.Write(data);
			p.Write(ChannelID);

			if (UseBuffer)
			{
				Buffer.Add(p);
			}
			else
			{
				SendMessageToTcp(p);

			}
		

		}


		private static void SendMessageToTcp(Packet data)
		{
			try
			{
				NetworkStream stream = socketConnection.GetStream();
				if (stream.CanWrite)
				{
			
					stream.Write(data.ToArray(), 0, data.Length());
				}
				else
				{
					throw new Exception("Stream CanWrite is false!");
				}
			}
			catch (SocketException arg)
			{
				Logger.Error("TCP", "Socket exception: " + arg);
			}
		}

	//	private static void SendMessageToTcp(string data)
	  //  {
			//try
			//{
			//	NetworkStream stream = socketConnection.GetStream();
			//	if (stream.CanWrite)
			//	{
			//		byte[] bytes = Encoding.ASCII.GetBytes(data);
			//		stream.Write(bytes, 0, bytes.Length);
			//	}
		//	}
		//	catch (SocketException arg)
		//	{
		//	Logger.Error("TCP", "Socket exception: " + arg);
		//	}
		//}


		static IEnumerator<float> BufferProcess()
		{
			while (true)
			{
				yield return Timing.WaitForSeconds(BufferSendTime);
				if(Buffer.Count > 0)
				{
					try
					{
						SendMessageToTcp(Buffer[0]);
						Buffer.RemoveAt(0);
					}
					catch (Exception)
					{

					}
				}
			}
		}

		public static void ConnectToTcpServer()
		{
			try
			{
				clientReceiveThread = new Thread(new ThreadStart(ListenForData));
				clientReceiveThread.IsBackground = true;
				clientReceiveThread.Start();
				AddLog("CONNECTING TO THE SERVER DONE!");
				Timing.RunCoroutine(BufferProcess());
				UseBot = true;
				
			}
			catch (Exception arg)
			{
				Logger.Error("BOT","On client connect exception " + arg);
			}
			Timing.RunCoroutine(CheckConn());
		}

		private static void ListenForData()
		{
			try
			{
				socketConnection = new TcpClient("127.0.0.1", Port);
				byte[] array = new byte[999999];
				SendMessage("[SCP: SL] SERVER CONNECTED!");
				for (; ; )
				{
					using (NetworkStream stream = socketConnection.GetStream())
					{
						int num;
						while ((num = stream.Read(array, 0, array.Length)) != 0)
						{
							AddLog("MSG DETECTED!");

							byte[] array2 = new byte[num];
							Array.Copy(array, 0, array2, 0, num);		
							Packet p = new Packet(array2);
							ReadMessages(p);
				
						}
					}
				}
			}
			catch (SocketException arg)
			{
				Logger.Error("BOT","Socket exception: " + arg);
			}
		}
		private static void ReadMessages(Packet data)
		{
			
		//	AddLog($"MESSAGE FROM TPC: {data}");

			try
			{
				var Secretkey = data.ReadString();
				var Type = (DiscordBotMsgType)data.ReadInt();
				var msg = data.ReadString();
				AddLog($"MESSAGE FROM TPC: {msg} TYPE: {Type}");
				if (Secretkey != SecretKey) {
					AddLog($"WRONG SECRET KEY DETECTED!");
					return;

				} 
				switch (Type)
				{
					case DiscordBotMsgType.DATA:
						{
							if(RecivedDatas.Count >= 100)
							{
								RecivedDatas.Clear();

							}

							if (!RecivedDatas.ContainsKey(msg))
							{
								RecivedDatas.Add(msg, data);
							}
							break;
						}
					case DiscordBotMsgType.RA_COMMAND:
						{
						
							AddLog($"RECIVED RA COMMAND FROM DISCORD: {msg}");
							CommandProcessor.ProcessQuery(msg + $"BOT_RA_COMMAND_REQUEST{SecretKey}", new BotSender());
							//string Result =	ServerConsole.EnterCommand("/" + msg.Data, out color, new BotSender());
							//		Logger.Info("DATA:", Result);
							break;
						}
					case DiscordBotMsgType.MESSAGE:
						break;
					case DiscordBotMsgType.CONSOLE_COMMAND:
						{
							AddLog($"RECIVED CONSOLE COMMAND FROM DISCORD: {msg}");
							ServerConsole.EnterCommand(msg);
							break;
						}
					case DiscordBotMsgType.REQUEST:

						var rec = data.ReadString();
						var Channel = data.ReadULong();
						var User = new DiscordUser(data);
						ProcessRequest(new DiscordRequest(User, rec, Channel));

						break;
					case DiscordBotMsgType.GAME_CONSOLE:
						{
							AddLog($"RECIVED GAME CONSOLE COMMAND FROM DISCORD: {msg}");
							ServerConsole.EnterCommand($".{msg}BOT_GAME_CONSOLE_COMMAND_REQUEST{SecretKey}");
							break;
						}
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("TCP", ex.Message);
			 
			}
		
		}

		private static void ProcessRequest(DiscordRequest request)
		{
			try
			{



				BotRequestEvent ev = new BotRequestEvent(request.Request, request.User, string.Empty, true, request.Channel);
				EventManager.Manager.HandleEvent<IEventHandlerBotRequest>(ev);
				if (!string.IsNullOrEmpty(ev.Output))
				{
					if (ev.SendOutput)
					{
						if(ev.Channel != 0)
						{
							SendMessage($"[SCP: SL] {ev.Output}", ev.Channel, false);
						}
						//SendCommandMessage($"[SCP: SL] {ev.Output}");
					}
					else
					{
						return;
					}
				
				}
				else
				{
					if(ev.SendOutput)
						SendMessage($"[SCP: SL] Command not found!", ev.Channel, false);

				}

			}
			catch (Exception ex)
			{

				Logger.Error("TCP", $"COMMAND REQUEST FAILED {ex}");
			}
		}

			private static IEnumerator<float> CheckConn()
		{
			while (true)
			{
				yield return Timing.WaitForSeconds(1f);
				if(socketConnection != null)
				{
					if (!socketConnection.Connected || !socketConnection.Client.Connected)
					{
						Logger.Error("TCP", "NO TCP CONNECTION TRYDING TO RECONNECT...");
						RestartTcp();
						yield return Timing.WaitForSeconds(5f);
					}
				}
				else
				{
					Logger.Error("TCP", "NO TCP CONNECTION TRYDING TO RECONNECT...");
					RestartTcp();
					yield return Timing.WaitForSeconds(5f);
				}
			}
		}



		public static void RestartTcp()
		{
			try
			{
				clientReceiveThread.Abort();
				clientReceiveThread.IsBackground = true;
				AddLog("DISCONNECT FROM THE SERVER DONE!");
			}
			catch (Exception)
			{
			}
			try
			{
				clientReceiveThread = new Thread(new ThreadStart(ListenForData));
				clientReceiveThread.IsBackground = true;
				clientReceiveThread.Start();
				AddLog("CONNECTING TO THE SERVER DONE!");
	
			}
			catch (Exception arg2)
			{
				Logger.Error("TCP","On client connect exception " + arg2);
			}
		}
	}
}
