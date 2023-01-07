using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using System.IO;
using UnityEngine;
using Utf8Json;

namespace WW_SYSTEM.Permissions
{
    public static class PermissionsManager
    {

        private static PMData Data;

       
        private static string ToJson()
        {
          return JsonSerializer.PrettyPrint(JsonSerializer.Serialize(Data));
        }



        private static PMData FromJson(string json)
        {
            return JsonSerializer.Deserialize<PMData>(json);
        }


        public static void SaveFile()
        {
       


            File.WriteAllText(GetFilePath, ToJson());
        }

        public static string GetFilePath
        {
            get
            {
                var path = Path.Combine(FileManager.GetAppFolder(true, true), "WW_PERMISSIONS.json");

                if (!File.Exists(path))
                {
                    LoadDefaults();
                    try
                    {

                        File.WriteAllText(path, "CREATING SAVE..");
                      SaveFile();
                    }
                    catch (Exception ex)
                    {

                        Logger.Error("WW_PermissionsManager", ex.ToString());
                    }

                }
                return path;
            }
        }

        public static void LoadDefaults()
        {
            var Admins = new List<PlayerPMData>();
            var Groups = new List<GroupPMData>();
            Admins.Add(new PlayerPMData("000000000000000@steam", "", "", 255, 255, true, new List<string>() { "TESTGROUP" }, new List<string>() { "TestPerm", "Overwatch" }));
            Groups.Add(new GroupPMData("TESTGROUP","TestPrefix", "red", 255, 255, new List<string>() { "PlayersManagement", "TestPerm2" }));

            Data = new PMData(Admins, Groups);
        }

        public static bool TryGetAdminData(string UserID, out PlayerPMData admin)
        {

            foreach (var item in Data.Admins)
            {
                if(item.UserID.ToUpper() == UserID.ToUpper())
                {
                    admin = item;
                    return true;
                }
            }

              admin = null;
            return false;
        }

        public static bool TryGetGroupData(string GroupName, out GroupPMData group)
        {

            foreach (var item in Data.Groups)
            {
                if (item.GroupName.ToUpper() == GroupName.ToUpper())
                {
                    group = item;
                    return true;
                }
            }

            group = null;
            return false;
        }

        public static void Reload()
        {
            Logger.Info("WW_PermissionsManager", "RELOADING..");
            try
            {
                var d = File.ReadAllText(GetFilePath);

                Data = FromJson(d);

                foreach (var g in Data.Groups)
                {
                    g.GroupName = g.GroupName.ToUpper();
                    g.GroupPerms = g.GroupPerms.ToUpper();
                }
                foreach (var a in Data.Admins)
                {
                    a.Groups = a.Groups.ToUpper();
                    a.Perms = a.Perms.ToUpper();
                    a.UserID = a.UserID.ToUpper();
                }

                foreach (var item in Server.Players)
                {
                    try
                    {
                        LoadPlayerGroup(item);
                    }
                    catch (Exception)
                    {

                       
                    }
                }


                Logger.Info("WW_PermissionsManager", "Permissions reloaded!");
            }
            catch (Exception ex)
            {
                Logger.Error("WW_PermissionsManager", ex.ToString());

            }
        }

        public static void LoadPlayerGroup(Player pl)
        {
            if (Data == null) return;

            foreach (var item in Data.Admins)
            {
                if(item.UserID.ToUpper() == pl.UserId.ToUpper())
                {
                    var Perms = item.Perms.ToUpper();
                    foreach (var group in item.Groups)
                    {

                        if (TryGetGroupData(group, out var g))
                        {
                            foreach (var perm in g.GroupPerms)
                            {
                                if (!Perms.Contains(perm.ToUpper()))
                                {
                                    Perms.Add(perm.ToUpper());
                                }
                            }

                            if (string.IsNullOrEmpty(item.Prefix))
                            {
                                item.Prefix = g.GroupPrefix;
                                item.Color = g.GroupColor;
                            }
                            if(item.KickPower < g.KickPower)
                            {
                                item.KickPower = g.KickPower;
                            }
                            if (item.RequiredKickPower < g.RequiredKickPower)
                            {
                                item.RequiredKickPower = g.RequiredKickPower;
                            }
                        }
                    }

                    item.Perms = Perms;
                    pl.PMData = item;
                 
                    return;
                }
            }
        }



        public static void RemovePlayer(string UserID)
        {
            if(TryGetAdminData(UserID, out var d))
            {
                Data.Admins.Remove(d);

                if(Round.TryGetPlayer(UserID, out var pl))
                {
                    pl.PMData = null;
                }

                SaveFile();
            }
        }

        public static void RemoveGroup(string GroupName)
        {
            if (TryGetGroupData(GroupName, out var g))
            {
                Data.Groups.Remove(g);
                SaveFile();
            }
        }

        public static void AddPlayer(string UserId, List<string> Perms, List<string> Groups, string Prefix = "", string Color = "", bool RemoteAdmin = false, byte KickPower = 0, byte RequiredKickPower = 0)
        {

       
            if(TryGetAdminData(UserId, out var d))
            {
                return;
            }

            var admin = new PlayerPMData(UserId.ToUpper(), Prefix, Color, KickPower, RequiredKickPower, RemoteAdmin, Groups, Perms);

           if(Round.TryGetPlayer(UserId, out var pl))
            {
                pl.PMData = admin;
            }

            Data.Admins.Add(admin);
            SaveFile();
        }

        public static void AddGroup(string GroupName, List<string> Perms, string Prefix = "", string Color = "", byte KickPower = 0, byte RequiredKickPower = 0)
        {

            if(TryGetGroupData(GroupName, out var g))
            {
                return;
            }
            
            var group = new GroupPMData(GroupName, Prefix, Color, KickPower, RequiredKickPower, Perms);

         

            Data.Groups.Add(group);
            SaveFile();
        }

        public static void AddPlayerPerm(string UserId, string Perm)
        {
            Perm = Perm.ToUpper();
            if (TryGetAdminData(UserId, out var d))
            {

                if (!d.Perms.Contains(Perm))
                {
                    d.Perms.Add(Perm);

                    if (Round.TryGetPlayer(UserId, out var pl))
                    {
                        pl.PMData = d;
                    }
                    SaveFile();
                }
            
            }
        }


        public static void RemovePlayerPerm(string UserId, string Perm)
        {
            Perm = Perm.ToUpper();
            if (TryGetAdminData(UserId, out var d))
            {

                if (d.Perms.Contains(Perm))
                {
                    d.Perms.Remove(Perm);

                    if (Round.TryGetPlayer(UserId, out var pl))
                    {
                        pl.PMData = d;
                    }
                    SaveFile();
                }

            }
        }

        public static void AddPlayerGroup(string UserId, string GroupName)
        {
            GroupName = GroupName.ToUpper();
            if (TryGetAdminData(UserId, out var d))
            {

                if (!d.Groups.Contains(GroupName))
                {
                    d.Groups.Add(GroupName);

                    if (Round.TryGetPlayer(UserId, out var pl))
                    {
                        LoadPlayerGroup(pl);
                    }
                    SaveFile();
                }

            }
        }

        public static void RemovePlayerGroup(string UserId, string GroupName)
        {
            GroupName = GroupName.ToUpper();
            if (TryGetAdminData(UserId, out var d))
            {

                if (d.Groups.Contains(GroupName))
                {
                    d.Groups.Remove(GroupName);

                    if (Round.TryGetPlayer(UserId, out var pl))
                    {
                        LoadPlayerGroup(pl);
                    }
                    SaveFile();
                }

            }
        }

        public static void SetPlayerPrefix(string UserID, string Prefix, string Color)
        {
            if (TryGetAdminData(UserID, out var d))
            {

                d.Prefix = Prefix;
                d.Color = Color;
                if (Round.TryGetPlayer(UserID, out var pl))
                {
                    LoadPlayerGroup(pl);
                }
                SaveFile();
                

            }
        }

        public static void SetPlayerRemoteAdmin(string UserID, bool RemoteAdmin)
        {
            if (TryGetAdminData(UserID, out var d))
            {

                d.RemoteAdmin = RemoteAdmin;
                if (Round.TryGetPlayer(UserID, out var pl))
                {
                    LoadPlayerGroup(pl);
                }
                SaveFile();


            }
        }

        public static void SetGroupPrefix(string GroupName, string Prefix, string Color)
        {
            if (TryGetGroupData(GroupName, out var d))
            {

                d.GroupPrefix = Prefix;
                d.GroupColor = Color;
              
                SaveFile();


            }
        }




        public static void AddGroupPerm(string GroupName, string Perm)
        {
            Perm = Perm.ToUpper();
            if (TryGetGroupData(GroupName, out var g))
            {

                if (!g.GroupPerms.Contains(Perm))
                {
                    g.GroupPerms.Add(Perm);

                   
                    SaveFile();
                }

            }
        }

        public static void RemoveGroupPerm(string GroupName, string Perm)
        {
            Perm = Perm.ToUpper();
            if (TryGetGroupData(GroupName, out var g))
            {

                if (g.GroupPerms.Contains(Perm))
                {
                    g.GroupPerms.Remove(Perm);


                    SaveFile();
                }

            }
        }


    }
}
