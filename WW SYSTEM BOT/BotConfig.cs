using System;
using System.Collections.Generic;
using System.Text;
namespace WW_SYSTEM_BOT
{
   [Serializable]
    public class BotConfig
    {
        public string Token { get; set; }
        public List<ulong> AllowedRaUsers { get; set; }
        public List<ulong> AllowedConsoleUsers { get; set; }
        public List<ulong> CommandsChannels { get; set; }
        public List<ulong> UserCommandsChannels { get; set; }
        public List<ulong> LogsChannels { get; set; }
        public string SecretKey { get; set; }
        public int Port { get; set; }
        public string CommandsPrefix { get; set; }
        public int CheckBuffertimeout { get; set; }
        public int UsePacketOnCount { get; set; }
        public ulong Guild { get; set; }
        public BotConfig(string Token, List<ulong> AllowedRaUsers, List<ulong> AllowedConsoleUsers, List<ulong> CommandsChannels, List<ulong> LogsChannels, string SecretKey, int Port, string CommandsPrefix, List<ulong> UserCommandsChannels, int CheckBuffertimeout, int UsePacketOnCount, ulong Guild)
        {
            this.Token = Token;
            this.AllowedRaUsers = AllowedRaUsers;
            this.AllowedConsoleUsers = AllowedConsoleUsers;
            this.CommandsChannels = CommandsChannels;
            this.LogsChannels = LogsChannels;
            this.SecretKey = SecretKey;
            this.Port = Port;
            this.CommandsPrefix = CommandsPrefix;
            this.UserCommandsChannels = UserCommandsChannels;
            this.CheckBuffertimeout = CheckBuffertimeout;
            this.UsePacketOnCount = UsePacketOnCount;
            this.Guild = Guild;
        }
    }
}
