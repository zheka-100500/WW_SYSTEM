using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using MEC;
using RoundRestarting;

namespace WW_SYSTEM.UpdateSystem
{
    public static class WW_UPDATE
    {
        private static string _Managed = Path.Combine(Application.dataPath, "Managed");
        private static string _PluginsDir = Path.Combine(FileManager.GetAppFolder(true, true), "Plugins");
        private static string _VersionFile = Path.Combine(_Managed, "buildid.txt");
        private static string _UpdateFile = Path.Combine(_Managed, "WW_SYSTEM_NEW.dll");
        private static string _MainFile = Path.Combine(_Managed, "WW_SYSTEM.dll");
        private static string _OLDFile = Path.Combine(_Managed, "WW_SYSTEM_OLD.dll");
        private static string Centralserver => "http://waer-world.ru/";
        private static string CurrentVersion
        {
            get
            {
                if (File.Exists(_VersionFile))
                {
                    return File.ReadAllText(_VersionFile);
                }
                else
                {
                    File.WriteAllText(_VersionFile, "0");
                    return "0";
                }
            }
            set
            {
                File.WriteAllText(_VersionFile, value);
            }
        }

        private static bool CheckInProcess = false;

        public static void CheckUpdate()
        {
            if (CheckInProcess) return;
            CheckInProcess = true;

            Timing.RunCoroutine(GetUpdateProcess());

        }


        private static IEnumerator<float> GetPluginsUpdateProcess()
        {
            Logger.Info("WW_SYSTEM", "CHECKING PLUGINS UPDATE...");

            bool RequiredRestart = false;
            foreach (var item in PluginManager.Manager.EnabledPlugins)
            {
                if (item.AutoUpdate != null)
                {
                    var Update = item.AutoUpdate;
                    UnityWebRequest www = new UnityWebRequest(Update.LinkToVersionFile);
                    www.downloadHandler = new DownloadHandlerBuffer();
                    yield return Timing.WaitUntilDone(www.SendWebRequest());
                    if (www.isHttpError || www.isNetworkError)
                    {
                        Logger.Error("WW_SYSTEM", $"FAILED TO CHECK UPDATE FOR PLUGIN {item.Details.id}: {www.error}");
                        continue;
                    }

                    var Current = item.Config.GetString("VERSION", "0");
                    string version;
                    try
                    {
                        version = www.downloadHandler.text;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("WW_SYSTEM", $"FAILED TO GET LATES VERSION FOR PLUGIN {item.Details.id}: {ex}");
                        continue;
                    }


                    Logger.Info("WW_SYSTEM", $"LATES PLUGIN {item.Details.id} VERSION: {version}");
                    if (version != Current)
                    {
                        IdleMode.PauseIdleMode = true;
                        Logger.Warn("WW_SYSTEM", $"PLUGIN {item.Details.id} is outdated your version ({Current}) new version ({version}) downloading and updating {item.Details.id}...");


                        www = new UnityWebRequest(Update.LinkToDllFile);

                        var DllName = string.IsNullOrEmpty(Update.DllFileName) ? Update.LinkToDllFile.Split(new char[] { '/' }).Last() : Update.DllFileName;

                        var UpdateFile = Path.Combine(_PluginsDir, DllName + ".new");
                        var OldFile = Path.Combine(_PluginsDir, DllName + ".old");
                        var MainFile = Path.Combine(_PluginsDir, DllName);
                        if (File.Exists(UpdateFile)) File.Delete(UpdateFile);

                        www.downloadHandler = new DownloadHandlerFile(UpdateFile);
                        yield return Timing.WaitUntilDone(www.SendWebRequest());
                        if (www.isHttpError || www.isNetworkError)
                        {
                            Logger.Error("WW_SYSTEM", $"FAILED TO DOWNLOAD UPDATE FOR PLUGIN {item.Details.id}: {www.error}");
                            continue;
                        }

                        item.Config.SetString("VERSION", version);
                        yield return Timing.WaitForOneFrame;
                        File.Replace(UpdateFile, MainFile, OldFile, true);

                        RequiredRestart = true;

                    }
                }
            }
            if (RequiredRestart)
            {
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                RoundRestart.ChangeLevel(true);

            }
            else
            {
                CheckInProcess = false;
            }

            
        }

        private static IEnumerator<float> GetUpdateProcess()
        {
            Logger.Info("WW_SYSTEM", "CHECKING WW_SYSTEM UPDATE...");
            UnityWebRequest www = new UnityWebRequest(Centralserver + "buildid.txt");
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return Timing.WaitUntilDone(www.SendWebRequest());
            if(www.isHttpError || www.isNetworkError)
            {
                Logger.Error("WW_SYSTEM", $"FAILED TO CHECK UPDATE: {www.error}");
                CheckInProcess = false;
                yield break;
            }
            var Current = CurrentVersion;
            string version;
            try
            {
                version = www.downloadHandler.text;
            }
            catch (Exception ex)
            {
                Logger.Error("WW_SYSTEM", $"FAILED TO GET LATES VERSION: {ex}");
                CheckInProcess = false;
                yield break;
            }
          
            Logger.Info("WW_SYSTEM", $"LATES VERSION: {version}");
            if (version != Current)
            {
                IdleMode.PauseIdleMode = true;
                Logger.Warn("WW_SYSTEM", $"WW_SYSTEM build is outdated your version ({Current}) new version ({version}) downloading and updating WW_SYSTEM...");


                www = new UnityWebRequest(Centralserver + "WW_SYSTEM.dll");

                if (File.Exists(_UpdateFile)) File.Delete(_UpdateFile);

                www.downloadHandler = new DownloadHandlerFile(_UpdateFile);
                yield return Timing.WaitUntilDone(www.SendWebRequest());
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Error("WW_SYSTEM", $"FAILED TO DOWNLOAD UPDATE: {www.error}");
                    CheckInProcess = false;
                    yield break;
                }

                CurrentVersion = version;
                yield return Timing.WaitForOneFrame;
                File.Replace(_UpdateFile, _MainFile, _OLDFile, true);

                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                RoundRestart.ChangeLevel(true);
            }
            Timing.RunCoroutine(GetPluginsUpdateProcess());
        } 


    }
}
