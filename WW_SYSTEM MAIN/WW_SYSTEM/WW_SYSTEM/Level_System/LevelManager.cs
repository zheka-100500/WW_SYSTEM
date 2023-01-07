using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WW_SYSTEM.API;
using UnityEngine;
using WW_SYSTEM.Events;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Level_System.Processors;

namespace WW_SYSTEM.Level_System
{
    public static class LevelManager
    {
        public static Dictionary<string, PlayerLevel> Levels = new Dictionary<string, PlayerLevel>();
       
        private static void AddLog(string data) => Logger.Info("LEVELSYSTEM", data);
        private static void Error(string data) => Logger.Error("LEVELSYSTEM", data);

        public static PlayerLevel StartLevel = new PlayerLevel(0, 1000, 0);

        public static LevelProcessor Processor = new LevelFileProcessor();

        public static bool RemoveInProgress { get; private set; }

        public static bool IgnoreDNT = false;



        private static bool CheckPlayerForDnt(string UserID)
        {
            if (!IgnoreDNT) return false;

            if(Round.TryGetPlayer(UserID, out var pl))
            {
                return pl.GetRoles.DoNotTrack;
            }
            return false;
            
        }

        public static void RemoveData(int LevelsToRemove, int XpToRemove, List<string> IgnoreUserIds)
        {
            //if (IgnoreUserIds == null) IgnoreUserIds = new List<string>();
            //if (RoundSummary.RoundInProgress() || Radio.roundEnded)
            //{
            //    throw new Exception("FAILED TO START REMOVE DATA: REMOVE IS ONLY POSSIBLE WHILE WAITING FOR PLAYERS!");
            //}
            //if (RemoveInProgress)
            //{
            //    throw new Exception("FAILED TO START REMOVE DATA: REMOVE IN PROGRESS!");

            //}
            //RemoveInProgress = true;
            //Levels.Clear();
            //int Count = 0;
            //foreach (var item in Directory.GetFiles(XPPath))
            //{
            //    if (item.Contains(".lvl"))
            //    {
            //        try
            //        {
            //            PlayerLevel level = JsonUtility.FromJson<PlayerLevel>(File.ReadAllText(item));

            //            string UserId = item.Replace(XPPath, "").Replace("/", "").Replace(".lvl", "");
            //            if (IgnoreUserIds.Contains(UserId))
            //            {
            //                AddLog($"FILE NOT REMOVED: {item} REASON: USER IS PROTECTED!");
            //                continue;
            //            }
            //            if(level.CurLvl <= LevelsToRemove && level.CurExp <= XpToRemove)
            //            {
                     
            //                AddLog($"REMOVING FILE: {item}");
            //                File.Delete(item);
            //                Count++;
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //            Error($"ERROR OF CHECK DATA: {item} REASON: {ex}");
            //        }
                  
            //    }
            //}
            //AddLog($"DONE REMOVED: {Count} FILES!");
            //RemoveInProgress = false;
        }

        public static bool LoadPlayer(string UserId)
        {
            if (CheckPlayerForDnt(UserId)) return false;
            if(Levels.ContainsKey(UserId)) return true;


            if(Processor.TryLoadPlayer(UserId, out var level))
            {
                Levels.Add(UserId, level);
                return true;
            }

            return false;




               
        }


        public static bool CheckForPlayer(string UserId)
        {
         
            if (CheckPlayerForDnt(UserId))
            {
                
                return false;
            } 
            if (Levels.ContainsKey(UserId)) return true;

            

            if (!Processor.CheckForPlayer(UserId))
            {
               AddLog("PLAYER: " + UserId + " NOT FOUND CREATING..");
                return SavePlayer(UserId);
            }
            else
            {
                return LoadPlayer(UserId);
            }
        }


        public static PlayerLevel GetPlayerLevel(string UserId)
        {
           

            if (CheckPlayerForDnt(UserId)) return null;

            if (Levels.ContainsKey(UserId))
            {
                return Levels[UserId];
            }
            else if(LoadPlayer(UserId))
            {
                return Levels[UserId];
            }

            return null;
            

        }

        public static bool SavePlayer(string UserId)
        {
            if (CheckPlayerForDnt(UserId)) return false;
            if (Levels.ContainsKey(UserId))
            {
         
                    PlayerLevel level = Levels[UserId];
                return Processor.SavePlayer(UserId, level);
            }
            else
            {
                if(Processor.SavePlayer(UserId, StartLevel))
                {
                    Levels.Add(UserId, StartLevel);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        static void RemoveXp(string UserId, int xp)
        {
            if (CheckPlayerForDnt(UserId)) return;
            if (!CheckForPlayer(UserId)) return;
            if (GetPlayerLevel(UserId) == null) return;

            PlayerLevel level = Levels[UserId];
            Player player = Round.GetPlayer(UserId);

            PlayerChangeXPEvent ev = new PlayerChangeXPEvent(false, true, xp, player);

            EventManager.Manager.HandleEvent<IEventHandlerPlayerChangeXP>(ev);

            if (!ev.Allow)
                return;
            xp = ev.XpCount;

            if(level.CurExp - xp < 0)
            {
            setNewLevel:
                int NewXpdata = xp - level.CurExp;
                int NewLevel = Levels[UserId].CurLvl - 1;
                if (NewLevel < StartLevel.CurLvl) return;
                int MaxXp = NewLevel * 250 + 750;
                
                PlayerLevelUpEvent ev1 = new PlayerLevelUpEvent(true, NewLevel, MaxXp, player);

                EventManager.Manager.HandleEvent<IEventHandlerPlayerLevelUp>(ev1);
                if (!ev1.Allow)
                    return;
                MaxXp = ev1.MaxXp;
                NewLevel = ev1.Level;

                Levels[UserId].CurExp = MaxXp - 1;
                Levels[UserId].CurLvl = NewLevel;
                Levels[UserId].MaxExp = MaxXp;
                SavePlayer(UserId);
                if (NewXpdata > 0)
                {
                    if (Levels[UserId].MaxExp <= NewXpdata)
                    {
                        xp = NewXpdata;
                        goto setNewLevel;
                    }
                    else if(Levels[UserId].CurExp - NewXpdata >= 0)
                    {
                      Levels[UserId].CurExp -= NewXpdata;
                        SavePlayer(UserId);
                        return;
                    }
                }
                else
                {
                    Levels[UserId].CurExp = 0;
                    SavePlayer(UserId);
                    return;
                }
                return;
            }
            else
            {
                level.CurExp -= xp;
                SavePlayer(UserId);
                return;
            }

        }
        static void AddXp(string UserId, int xp)
        {
            if (CheckPlayerForDnt(UserId)) return;
            if (!CheckForPlayer(UserId)) return;
            if (GetPlayerLevel(UserId) == null) return;
            PlayerLevel level = Levels[UserId];
            Player player = Round.GetPlayer(UserId);

            PlayerChangeXPEvent ev = new PlayerChangeXPEvent(true, true, xp, player);

            EventManager.Manager.HandleEvent<IEventHandlerPlayerChangeXP>(ev);

            if (!ev.Allow)
                return;
            xp = ev.XpCount;

            if (level.CurExp + xp >= level.MaxExp)
            {
               
                if(level.CurExp + xp == level.MaxExp)
                {
                    int NewLevel = Levels[UserId].CurLvl + 1;
                    int MaxXp = NewLevel * 250 + 750;
                    PlayerLevelUpEvent ev1 = new PlayerLevelUpEvent(true, NewLevel, MaxXp, player);

                    EventManager.Manager.HandleEvent<IEventHandlerPlayerLevelUp>(ev1);
                    if (!ev1.Allow)
                        return;
                    MaxXp = ev1.MaxXp;
                    NewLevel = ev1.Level;

                    Levels[UserId].CurExp = 0;
                    Levels[UserId].CurLvl = NewLevel;
                    Levels[UserId].MaxExp = MaxXp;
                    SavePlayer(UserId);
                    return;
                }
                else
                {
                    setNewLevel:
                    int NewXpdata = xp - Levels[UserId].MaxExp;
                    int NewLevel = Levels[UserId].CurLvl + 1;
                    int MaxXp = NewLevel * 250 + 750;
                    PlayerLevelUpEvent ev1 = new PlayerLevelUpEvent(true, NewLevel, MaxXp, player);

                    EventManager.Manager.HandleEvent<IEventHandlerPlayerLevelUp>(ev1);
                    if (!ev1.Allow)
                        return;
                    MaxXp = ev1.MaxXp;
                    NewLevel = ev1.Level;
                    
                    Levels[UserId].CurExp = NewXpdata;
                    Levels[UserId].CurLvl = NewLevel;
                    Levels[UserId].MaxExp = MaxXp;
                    SavePlayer(UserId);
                    if(NewXpdata > 0)
                    {
                        if (NewXpdata >= Levels[UserId].MaxExp)
                        {
                            Levels[UserId].CurExp = 0;
                            xp = NewXpdata;
                            goto setNewLevel;
                        }
                        else
                        {
                            Levels[UserId].CurExp = NewXpdata;
                            SavePlayer(UserId);
                            return;
                        }
                    }
                    else
                    {
                        Levels[UserId].CurExp = 0;
                        SavePlayer(UserId);
                        return;
                    }

                }



            }
            else
            {
                Levels[UserId].CurExp += xp;
                SavePlayer(UserId);
                return;
            }

        }

        public static void SetExp(LVLSETTYPE type, LVLDATATYPE Dtype, string UserId, int data)
        {
            if (CheckPlayerForDnt(UserId)) return;
            if (!CheckForPlayer(UserId)) return;
            if (GetPlayerLevel(UserId) == null) return;

            switch (Dtype)
            {
                case LVLDATATYPE.LEVEL:
                    switch (type)
                    {
                        case LVLSETTYPE.Add:
                            int NewLevel = Levels[UserId].CurLvl + data;
                            int MaxXp = NewLevel * 250 + 750;
                            Player player = Round.GetPlayer(UserId);
                            PlayerLevelUpEvent ev1 = new PlayerLevelUpEvent(true, NewLevel, MaxXp, player);

                            EventManager.Manager.HandleEvent<IEventHandlerPlayerLevelUp>(ev1);
                            if (!ev1.Allow)
                                return;
                            MaxXp = ev1.MaxXp;
                            NewLevel = ev1.Level;

                            Levels[UserId].CurExp = 0;
                            Levels[UserId].CurLvl = NewLevel;
                            Levels[UserId].MaxExp = MaxXp;
                            SavePlayer(UserId);
                            return;
                        case LVLSETTYPE.Set:
                            if (data < 0)
                            {
                                Levels[UserId].CurLvl = 0;
                            }
                            else
                            {
                                Levels[UserId].CurLvl = data;
                            }
                            SavePlayer(UserId);
                            return;
                        case LVLSETTYPE.Remove:
                            if(Levels[UserId].CurLvl - data <= 0)
                            {
                                Levels[UserId].CurLvl = 0;
                            }
                            else
                            {
                                Levels[UserId].CurLvl -= data;
                            }
                            SavePlayer(UserId);
                            return;
                        default:
                            return;
                    }
                case LVLDATATYPE.XP:
                    switch (type)
                    {
                        case LVLSETTYPE.Add:
                            AddXp(UserId, data);
                            return;
                        case LVLSETTYPE.Set:
                            if (data < 0)
                            {
                                Levels[UserId].CurExp = 0;
                            }
                            else
                            {
                                Levels[UserId].CurExp = data;
                            }
                            SavePlayer(UserId);
                            return;
                        case LVLSETTYPE.Remove:
                            RemoveXp(UserId, data);
                            return;
                        default:
                            return;
                    }
                case LVLDATATYPE.XPTOLVLUP:
                    switch (type)
                    {
                        case LVLSETTYPE.Add:
                            Levels[UserId].MaxExp += data;
                            SavePlayer(UserId);
                            return;
                        case LVLSETTYPE.Set:
                            if(data <= 0)
                            {
                                Levels[UserId].MaxExp = 1;
                            }
                            else
                            {
                                Levels[UserId].MaxExp = data;
                            }
                            SavePlayer(UserId);
                            return;
                        case LVLSETTYPE.Remove:
                            if (Levels[UserId].MaxExp - data <= 0)
                            {
                                Levels[UserId].MaxExp = 1;
                            }
                            else
                            {
                                Levels[UserId].MaxExp -= data;
                            }
                            SavePlayer(UserId);
                            return;
                        default:
                            return;
                    }
                default:
                    break;
            }


        }
    }
}
