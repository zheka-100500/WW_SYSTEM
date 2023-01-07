using LiteNetLib.Utils;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Security
{
	public static class ConnectionSecurity
	{
		public static bool UseSystem = false;
		public static bool InitDone { get; private set; } = false;
		public static void Init()
		{
			if(!UseSystem)
			{
				return;
			}

			if (!InitDone)
			{
				InitDone = true;
			}
			else
			{
				return;
			}


		}

		public static bool AllowConnect(IPAddress address,NetDataReader p)
		{
			if (!UseSystem) return true;

			byte b;
			if (!p.TryGetByte(out b) || b >= 2)
			{

				return true;
			}

			byte cMajor;
			byte cMinor;
			byte cRevision;
			bool flag;
			byte cBackwardRevision = 0;

			if (!p.TryGetByte(out cMajor) || !p.TryGetByte(out cMinor) || !p.TryGetByte(out cRevision) || !p.TryGetBool(out flag) || (flag && !p.TryGetByte(out cBackwardRevision)))
			{

				return true;
			}

			int num;
			p.TryGetInt(out num);
			string testing;
			p.TryGetString(out testing);
			string key2 = address + "-" + num;
			Logger.Info("TESTING",$"IP: {address} STEAM: {testing}");
			if (!CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key2))
			{
				if (num == 0 && testing.Contains("@steam")) return false;
			}
			return false;


		}


	}
}
