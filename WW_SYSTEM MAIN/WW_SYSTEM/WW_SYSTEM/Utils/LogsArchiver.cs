using RoundRestarting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Utils
{
    public static class LogsArchiver
    {
        public static bool RemoveLogsFiles = true;
        public static bool Enabled = true;
        public static int MaxLogs = 60;

        private static string GetLogsDirectory()
        {
            return Path.Combine(FileManager.GetAppFolder(true, true), "logs");
        }

        private static int GetLogsCount()
        {

            

            return GetLogs().Count;
        }

        public static List<string> GetLogs()
        {
            var result = new List<string>();
            foreach (var item in Directory.GetFiles(GetLogsDirectory()))
            {
                if (item.EndsWith(".txt")) result.Add(item);
            }
            return result;

        }

        public static List<FileInfo> GetLogsInfos()
        {
            var Result = new List<FileInfo>();
            foreach (var item in GetLogs())
            {
                Result.Add(new FileInfo(item));
            }
            return Result;
        }

        public static void CheckLogs()
        {
            if (!Enabled) return;

            var LogsCount = GetLogsCount();
          if(LogsCount > MaxLogs)
            {
                Logger.Warn("LogsArchiver", "The limit of log files has been reached.");
                ArchiveLogs();
            }
        }

        public static void ArchiveLogs()
        {
            string hour = DateTime.Now.Hour.ToString();
            string Minute = DateTime.Now.Minute.ToString();
            string Seconds = DateTime.Now.Second.ToString();

            if (hour.Length == 1)
            {
                hour = $"0{hour}";
            }
            if (Minute.Length == 1)
            {
                Minute = $"0{Minute}";
            }
            if (Seconds.Length == 1)
            {
                Seconds = $"0{Seconds}";
            }

       

            var LogsP = Path.Combine(GetLogsDirectory(), $"LogArchive-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-_{hour}_{Minute}_{Seconds}.zip");

            using (var memoryStream = new MemoryStream())
            {
                var CurrentLog = 0;
                
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var alllogs = GetLogsInfos();
                    var LogsCount = alllogs.Count;


                   

                    foreach (var item in alllogs)
                    {
                        var zipfile = archive.CreateEntry(item.Name);
                        using (var entryStream = zipfile.Open())
                        {
                            using (var fileToCompressStream = new MemoryStream(File.ReadAllBytes(item.FullName)))
                            {
                                fileToCompressStream.CopyTo(entryStream);
                                fileToCompressStream.Close();
                            }
                        }
                        CurrentLog++;
                        
                     
                        if(RemoveLogsFiles) File.Delete(item.FullName);




                    }

                }

                using (var fileStream = new FileStream(LogsP, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }

                Logger.Info("LogsArchiver", $"LogsCompressed: {CurrentLog}");
            }

       
        }
    }
}
