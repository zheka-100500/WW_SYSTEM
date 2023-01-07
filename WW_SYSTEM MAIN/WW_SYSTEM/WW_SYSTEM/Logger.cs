using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore;

namespace WW_SYSTEM
{
	public static class Logger
	{
		public static void Debug(string tag, string message)
		{
			if (ConfigFile.ServerConfig.GetBool("WW_SYSTEM_DEBUG", false))
			{
				Write("DEBUG", tag, message);
			}
		}

		
		public static void Error(string tag, string message)
		{
			Write("ERROR", tag, message);
		}

		
		public static void Info(string tag, string message)
		{
			Write("INFO", tag, message);
		}

		
		public static void Warn(string tag, string message)
		{
			Write("WARN", tag, message);
		}

	
		private static void Write(string level, string tag, string message)
		{
			ServerConsole.AddLog(string.Format("[WW SYSTEM] [{0}] [{1}] {2}", level, tag, message));
		}
	}
}
