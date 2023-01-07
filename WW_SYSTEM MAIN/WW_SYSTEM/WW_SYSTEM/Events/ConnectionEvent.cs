using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Events
{
	public abstract class ConnectionEvent : Event
	{

		public Connection Connection
		{
			get
			{
				return this.connection;
			}
		}


		public ConnectionEvent(Connection connection)
		{
			this.connection = connection;
		}


		private Connection connection;
	}
}
