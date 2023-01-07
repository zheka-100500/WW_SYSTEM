using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WW_SYSTEM_BOT.Commands;

namespace WW_SYSTEM_BOT
{
    public class Bot
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextModule Commands { get; private set; }

        public static BotConfig Config { get; private set; }

        public static List<DiscordChannel> CommandsChannels = new List<DiscordChannel>();
        public static List<DiscordChannel> LogsChannels = new List<DiscordChannel>();
        public static List<DiscordChannel> UserCommandsChannels = new List<DiscordChannel>();

        public static TcpClient TcpClient { get; private set; }

        public static List<string> Logs = new List<string>();
        public Thread LogsThread;

        public async Task RunAsync()
        {
            var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            if (!File.Exists($"{path}/config.cfg"))
            {
                BotConfig cfg = new BotConfig("TOKEN", new List<ulong>() { 111111, 2222222 }, new List<ulong>() { 111111, 2222222 }, new List<ulong>() { 111111, 2222222 }, new List<ulong>() { 111111, 2222222 }, "SECRET_KEY", 1234, "/", new List<ulong>() { 111111, 2222222 }, 3000, 10, 000000000000);

                string data = JsonConvert.SerializeObject(cfg, Formatting.Indented);
               await File.WriteAllTextAsync($"{path}/config.cfg", data);
                Logger.Error("SYSTEM", $"PLEASE SETUP CFG: {path}/config.cfg");
                return;
            }
            else
            {
             
                string Text = await File.ReadAllTextAsync($"{path}/config.cfg");
                
                Config = JsonConvert.DeserializeObject<BotConfig>(Text);
               
                if (string.IsNullOrEmpty(Config.Token) || Config.Token.ToUpper() == "TOKEN")
                {
                    Logger.Error("SYSTEM", $"TOKEN NOT SET. PLEASE SETUP CFG: {path}/config.cfg");
                    return;
                }

                if (string.IsNullOrEmpty(Config.SecretKey) || Config.SecretKey.ToUpper() == "SECRET_KEY")
                {
                    Logger.Error("SYSTEM", $"SECRET KEY NOT SET. PLEASE SETUP CFG: {path}/config.cfg");
                    return;
                }
            }


            var DsConfig = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(DsConfig);
            Client.Ready += OnClientReady;

            var CommandsConfig = new CommandsNextConfiguration
            {
                StringPrefix = Config.CommandsPrefix,
                EnableDms = false,
                EnableMentionPrefix = true
            };

            Commands = Client.UseCommandsNext(CommandsConfig);

            Commands.RegisterCommands<RaCommands>();


            Client.MessageCreated += OnMessageRecived;

            await Client.ConnectAsync();
            ThreadStart ts = new ThreadStart(LogsProccess);
            LogsThread = new Thread(ts);
            LogsThread.Start();
            await TpcServer();

        }

        private static void LogsProccess()
        {
            while (true)
            {
                if (Config.CheckBuffertimeout <= 0) Config.CheckBuffertimeout = 3000;
                if (Config.UsePacketOnCount <= 0) Config.UsePacketOnCount = 10;

                Thread.Sleep(Config.CheckBuffertimeout);
                try
                {
                    if (Logs.Count > Config.UsePacketOnCount)
                    {
                        var log = "";
                        var count = Logs.Count;
                        var RemoveCount = 0;
                        for (int i = 0; i < count; i++)
                        {
                            if ((log + Environment.NewLine + Logs[i]).Length < 1900)
                            {
                                log += Environment.NewLine + Logs[i];

                                if (i == count - 1)
                                {
                                    SendLog(log).GetAwaiter().GetResult();
                                    Logs.RemoveRange(0, count);
                                    break;
                                }
                                RemoveCount++;
                            }
                            else
                            {
                                SendLog(log).GetAwaiter().GetResult();
                                Logs.RemoveRange(0, RemoveCount);
                                break;
                            }
                        }

                    }
                    else if (Logs.Count > 0)
                    {
                        var log = Logs[0];
                        SendLog(log).GetAwaiter().GetResult();
                        Logs.RemoveAt(0);
                    }
                }
                catch (Exception ex)
                {

                    Logger.Error("BUFFER", ex.ToString());
                }
               
            }
        }

        private static async Task SendLog(string log)
        {
            foreach (var item in LogsChannels)
            {
                await item.SendMessageAsync(log);
            }
        }

        private async Task TpcServer()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), Config.Port);
            server.Start();  // запускаем сервер
            Logger.Info("TCP", "STARTED!");
        start:
            Logger.Info("TCP", "SYSTEM STARTED!");
            while (true)   // бесконечный цикл обслуживания клиентов
            {
                try
                {
                    TcpClient = server.AcceptTcpClient();  // ожидаем подключение клиента
                    NetworkStream ns = TcpClient.GetStream(); // для получения и отправки сообщений     // отправляем сообщение
                    while (TcpClient.Connected)  // пока клиент подключен, ждем приходящие сообщения
                    {
                        byte[] msg = new byte[1024];     // готовим место для принятия сообщения
                        int count = ns.Read(msg, 0, msg.Length);   // читаем сообщение от клиента
                             
                 
                        //string request = Encoding.Default.GetString(msg, 0, count);
                        byte[] array2 = new byte[count];
                        Array.Copy(msg, 0, array2, 0, count);

                        Packet p = new Packet(array2);

                        if (count != 0)
                        {
                            try
                            {
                                // Logger.Info("TCP", $"RECIVED DATA: {request}");
                                //byte[] DecryptedMsg = Convert.FromBase64String(request);
                                // string DecryptedData = Encoding.UTF8.GetString(DecryptedMsg);

                                //  DiscordBotMsg BotMsg = JsonConvert.DeserializeObject<DiscordBotMsg>(DecryptedData);
                     

                                if(p.ReadString() == Config.SecretKey)
                                {
                   
                                    await ReadMsg(p);
                                }
                                else
                                {
                                    Logger.Warning("TCP", $"CLIENT: {TcpClient.Client.RemoteEndPoint} TRYDING TO SEND REQUEST WITH NO AUTH! DISCONNECTING CLIENT!");
                                    TcpClient.Client.Close();
                                }
                                
                               
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("TCP", $"REQUEST PROCESS FAILED: {ex.Message}");
                            }
                        }
                        else
                        {
                            Logger.Warning("TCP", "CLIENT DISCONNECTED RESTART SYSTEM IN 5 SECONDS..");
                            await Task.Delay(5000);


                            goto start;
                        }
                        }
                }
                catch (Exception ex)
                {

                    Logger.Info("TCP", $"DETECTED ERROR: {ex}");
                }
              
            }
        }

        public static async Task SendMsg(Packet msg)
        {
            try
            {
            
                NetworkStream ns = TcpClient.GetStream();
                Logger.Info("TCP",$"SEND PACKET Length: {msg.Length()}");
                await ns.WriteAsync(msg.ToArray(), 0, msg.Length());
            }
            catch (Exception)
            {

            }

        }



        private async Task ReadMsg(Packet msg)
        {

            var type = (DiscordBotMsgType)msg.ReadInt();
            var data = msg.ReadString();
            Logger.Info("TCP", $"DATA RECIVED TYPE: {type} DATA: {data}");
            switch (type)
            {
                case DiscordBotMsgType.RA_COMMAND:
                    break;
                case DiscordBotMsgType.MESSAGE:
                    {
                        Logs.Add(data);
                        break;
                    }
                case DiscordBotMsgType.CONSOLE_COMMAND:
                    break;
                case DiscordBotMsgType.REQUEST:
                    break;
                case DiscordBotMsgType.DATA:
                    {
                        await ProcessData(data, msg);
                        break;
                    }
                case DiscordBotMsgType.STATUS:
                    {
                        await SetBotStatus(new BotStatus(data, msg.ReadInt()));
                        break;
                    }
                case DiscordBotMsgType.COMMAND_OUTPUT:
                    {
                        foreach (var item in CommandsChannels)
                        {
                            await item.SendMessageAsync(data);
                        }
                        break;
                    }
                case DiscordBotMsgType.CHANNEL_MESSAGE:
                    {
                        try
                        {
                            DiscordChannel channel = await Client.GetChannelAsync(msg.ReadULong());
                            await channel.SendMessageAsync(data);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("MSG PROCESSOR", $"FAILED TO SEND MESSAGE TO CHANNEL: {ex}");
                        }
                  
                        break;
                    }
                default:
                    Logger.Error("MSG PROCESSOR", $"RECEIVED MSG WITH WRONG TYPE");
                    break;
            }


        }

        private async Task<Datas.DiscordUser> GetDiscordUser(ulong Id)
        {
            var g = await Client.GetGuildAsync(Config.Guild);
            if (g != null)
            {
               
                var member = await g.GetMemberAsync(Id);
                if (member != null)
                {
  
                    var User = new Datas.DiscordUser(member.Id, member.Username, member.Discriminator, member.AvatarUrl);
                    foreach (var item in member.Roles)
                    {
                 
                              User.Roles.Add(new Datas.DiscordRole(item.Id, item.Name, item.Color));
                    }


                    return User;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private async Task ProcessData(string Hash,Packet packet)
        {

            try
            {
                switch (packet.ReadInt())
                {
                    case 0:
                        {
                            var g = await Client.GetGuildAsync(Config.Guild);
                            if(g != null)
                            {
                                var Params = packet.ReadStrings();
                                if(Params.Count >= 1)
                                {
                                    if(ulong.TryParse(Params[0], out var id))
                                    {
                                        var member = await GetDiscordUser(id);
                                        if(member != null)
                                        {
                                            Packet p = new Packet();
                                            p.Write(Config.SecretKey);
                                            p.Write((int)DiscordBotMsgType.DATA);
                                            p.Write(Hash);


                                            await SendMsg(member.GetPacket(p));
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {

                Logger.Error("ProcessData", $"DETECTED ERROR: {ex}");
            }
         

        }


            private async Task SetBotStatus(BotStatus status)
        {


            try
            {
          

                DiscordGame game = new DiscordGame { Name = status.Status};
                await Client.UpdateStatusAsync(game, (UserStatus)status.Type);
             
            }
            catch (Exception ex)
            {
                Logger.Error("TCP", $"FAILED TO DECRYPT STATUS: {ex}");
            }



        }




        private async Task OnClientReady(ReadyEventArgs ev)
        {
          

            foreach (var item in Config.CommandsChannels)
            {
                try
                {
                    DiscordChannel channel = await Client.GetChannelAsync(item);
                    if (!CommandsChannels.Contains(channel))
                    {
                        CommandsChannels.Add(channel);
                        Logger.Info("CLIENT", $"LOADED COMMANDS CHANNEL: {channel.Name}");
                    }
              
                }
                catch (Exception ex)
                {
                    Logger.Error("CLIENT", $"FAILED TO GET COMMANDS CHANNEL: {item} REASON: {ex.Message}");
                }
            }
            foreach (var item in Config.LogsChannels)
            {
                try
                {
                    DiscordChannel channel = await Client.GetChannelAsync(item);
                    if (!LogsChannels.Contains(channel))
                    {
                        LogsChannels.Add(channel);
                        Logger.Info("CLIENT", $"LOADED LOGS CHANNEL: {channel.Name}");
                     
                    }
                  
                }
                catch (Exception ex)
                {
                    Logger.Error("CLIENT", $"FAILED TO GET LOGS CHANNEL: {item} REASON: {ex.Message}");
                }
            }

            foreach (var item in Config.UserCommandsChannels)
            {
                try
                {
                    DiscordChannel channel = await Client.GetChannelAsync(item);
                    if (!UserCommandsChannels.Contains(channel))
                    {
                        UserCommandsChannels.Add(channel);
                        Logger.Info("CLIENT", $"LOADED USER COMMANDS CHANNEL: {channel.Name}");
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("CLIENT", $"FAILED TO GET USER COMMANDS CHANNEL: {item} REASON: {ex.Message}");
                }
            }
            Logger.Info("CLIENT", "READY!");
        }

        private bool IsAllowedCmd(string Cmds)
        {
            if (Cmds.StartsWith("RA") || Cmds.StartsWith("SUDO") || Cmds.StartsWith("LIST") || Cmds.StartsWith("GAME")) return false;
            return true;
        }

        private async Task OnMessageRecived(MessageCreateEventArgs ev)
        {
            if ((CommandsChannels.Contains(ev.Message.Channel) || UserCommandsChannels.Contains(ev.Message.Channel)) && ev.Message.Content.StartsWith(Config.CommandsPrefix))
            {
                string Cmds = ev.Message.Content;
                if (IsAllowedCmd(Cmds.ToUpper().Remove(0, 1)))
                {
                  //  string request = JsonConvert.SerializeObject(new DiscordRequest(ev.Message.Author.Username, ev.Message.Author.Id, Cmds.Remove(0, 1)));

                    Packet p = new Packet();
                    p.Write(Bot.Config.SecretKey);
                    p.Write((int)DiscordBotMsgType.REQUEST);
             
                    p.Write(ev.Message.Author.Username);
     
                    var User = await GetDiscordUser(ev.Message.Author.Id);
                    if(User != null)
                    {
                        Cmds = Cmds.Remove(0, 1);
                        Logger.Info("COMMAND", $"TRY TO SEND COMMAND: {Cmds} FROM USER: {ev.Message.Author.Username}");
                        p.Write(Cmds);
                        p.Write(ev.Message.ChannelId);
                        await Bot.SendMsg(User.GetPacket(p));
                      
                    }
                 
                }
        
            }
       
        }
    }
}
