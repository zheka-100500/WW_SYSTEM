using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;

namespace TestPlugin
{
    class EventHandler: IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandler079AddExp, IEventHandlerAdminQuery
    {
        private Plugin plugin;
        private TestPlugin testPlugin;

        public EventHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void On079AddExp(Player079AddExpEvent ev)
        {
            plugin.Info("ADD 079 XP: " + ev.ExpToAdd.ToString());
        }

        public void OnAdminQuery(AdminQueryEvent ev)
        {
           
               plugin.Info("ADMIN: " + ev.Admin.Nick + " WITCH ID: " + ev.Admin.UserId + " USED COMMAND: " + ev.Query);
            
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            plugin.Info("Player joined: " + ev.Player.Nick + " WITCH ID: " + ev.Player.UserId);
        }

      

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            plugin.Info("WAITING FOR PLAYERS");
        }
    }
}
