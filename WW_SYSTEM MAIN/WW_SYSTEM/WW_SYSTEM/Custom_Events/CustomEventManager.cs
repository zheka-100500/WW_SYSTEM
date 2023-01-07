using PlayerRoles;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
namespace WW_SYSTEM.Custom_Events
{
    public static class CustomEventManager
    {

        public static CustomEvent CurrentEvent = null;


        public static List<CustomEvent> EventList = new List<CustomEvent>();

        public static void StartEvent(int id, bool force = false)
        {
            CustomEvent customEvent = GetEvent(id);
            if(customEvent == null)
            {
                Error($"Event with {id} not found in the list!");
                throw new Exception($"Event with {id} not found in the list!");
            }
            if (CurrentEvent != null)
            {
                Error($"Event is already started!");
                throw new Exception($"Event is already started!");
            }
            if (RoundSummary.RoundInProgress())
            {
                Error($"It is not possible to launch an event during the round or the end of the round!");
                throw new Exception($"It is not possible to launch an event during the round or the end of the round!");
            }
            
            if (customEvent.RequedPlayers > Round.GetPlayers().Count && !force)
            {
                Error($"Not enough players to run the event!");
                throw new Exception($"Not enough players to run the event!");
            }
            CurrentEvent = customEvent;
            CurrentEvent.OnEventStarted();
            Info($"EVENT: {CurrentEvent.EventName} HAS BEEN STARTED!");
            CharacterClassManager.ForceRoundStart();
        }


        private static void Info(string data)
        {


            Logger.Info("CUSTOM EVENT MANAGER", data);
        }

        private static void Warn(string data)
        {


            Logger.Warn("CUSTOM EVENT MANAGER", data);
        }

        private static void Error(string data)
        {


            Logger.Error("CUSTOM EVENT MANAGER", data);
        }

        public static void StopEvent(bool force = false)
        {

            if(CurrentEvent == null)
            {
                Error("Event not started!");
                throw new Exception($"Event not started!");
            }
         

            CurrentEvent.OnEventStoped();
            var oldvalue = CustomNetworkManager.EnableFastRestart;
            if (RoundSummary.RoundInProgress())
            {
                foreach (var pl in Round.GetPlayers())
                {
                    if(pl.Role != RoleTypeId.Spectator)
                    {
                        pl.Role = RoleTypeId.Spectator;
                    }
                }

                Round.ForceEndRound = true;
                if (force)
                {
            
                    CustomNetworkManager.EnableFastRestart = false;
                    RoundRestart.InitiateRoundRestart();
                    CustomNetworkManager.EnableFastRestart = oldvalue;
                }
                Info($"EVENT: {CurrentEvent.EventName} HAS BEEN STOOPED!");
                CurrentEvent = null;
                return;
            }
            else
            {
                if (force)
                {
                    foreach (var pl in Round.GetPlayers())
                    {
                        if (pl.Role != RoleTypeId.Spectator)
                        {
                            pl.Role = RoleTypeId.Spectator;
                        }
                    }
                    CustomNetworkManager.EnableFastRestart = false;
                    RoundRestart.InitiateRoundRestart();
                    CustomNetworkManager.EnableFastRestart = oldvalue;
                    Info($"EVENT: {CurrentEvent.EventName} HAS BEEN STOOPED!");
                    CurrentEvent = null;
                    return;
                }
            }

            Error($"FAILED TO STOP EVENT!");
            throw new Exception($"FAILED TO STOP EVENT!");

        }


        public static void AddEvent(CustomEvent Event, int id, string EventName, int RequedPlayers)
        {

            for (int i = 0; i < EventList.Count; i++)
            {
                if(EventList[i].Id == id)
                {
                    
                    throw new Exception($"Event with {id} already in the list!");
                    
                }
            }
            Event.Id = id;
            Event.EventName = EventName;
            Event.RequedPlayers = RequedPlayers;
            EventList.Add(Event);


        }

        public static void RemoveEvent(int id)
        {

            for (int i = 0; i < EventList.Count; i++)
            {
                if (EventList[i].Id == id)
                {
                    EventList.Remove(EventList[i]);
                    return;

                }
            }
            throw new Exception($"Event with {id} not found in the list!");


        }

        public static bool EventInProgress
        {

            get { return CurrentEvent != null; }
        }

        public static bool EventIsActive(int id)
        {
            if(CurrentEvent != null && CurrentEvent.Id == id)
            {
                return true;
            }
            return false;
        }

        public static CustomEvent GetEvent(int id)
        {

            for (int i = 0; i < EventList.Count; i++)
            {
                if (EventList[i].Id == id)
                {
                    return EventList[i];
                   

                }
            }
            throw new Exception($"Event with {id} not found in the list!");
        }




    }
}
