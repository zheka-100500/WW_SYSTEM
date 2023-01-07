using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WW_SYSTEM_BOT
{
    public static class Logger
    {

        private static string LogsDir = "";

        public static string LogName = "";

        public static string GetLogsDir
        {
            get
            {
                if (string.IsNullOrEmpty(LogsDir))
                {
                    var path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

                    var logs = Path.Combine(path, "Logs");
                    LogsDir = logs;
                    if (Directory.Exists(logs))
                    {
                
                        return LogsDir;
                    }
                    else
                    {
                        Directory.CreateDirectory(logs);
                        return LogsDir;
                    }
                }
                else
                {
                    return LogsDir;
                }
            }
        }

        public static void WriteLog(string log)
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

            if (string.IsNullOrEmpty(LogName))
            {

                LogName = $"{GetLogsDir}/{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-_{hour}_{Minute}_{Seconds}.log";
                File.WriteAllText(LogName, "LOG STARTED!");
            }

            string time = "[" + hour + ":" + Minute + ":" + Seconds + "]";

            string logToAdd = $"{time} {log}";

            File.AppendAllText(LogName, Environment.NewLine + logToAdd);
        }

        public static void Info(string tag, string Msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var log = $"[INFO] [{tag}] {Msg}";
            WriteLog(log);
            Console.WriteLine(Time + log);
            Console.ResetColor();
        }

        public static void Warning(string tag, string Msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            var log = $"[WARNING] [{tag}] {Msg}";
            WriteLog(log);
            Console.WriteLine(Time + log);
            Console.ResetColor();
        }

        public static void Error(string tag, string Msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var log = $"[ERROR] [{tag}] {Msg}";
            WriteLog(log);
            Console.WriteLine(Time + log);
            Console.ResetColor();
        }

        public static string Time => $"[{DateTime.Now.AddHours(3):HH:mm:ss}] ";
    }
}
