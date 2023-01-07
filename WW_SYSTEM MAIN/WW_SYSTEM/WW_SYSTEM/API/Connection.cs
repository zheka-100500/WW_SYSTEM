using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
	public abstract class Connection
	{
		public string IpAddress
		{
			get
			{
				return this.conn.address;
			}
		}

		public Connection(NetworkConnection conn)
		{
			this.conn = conn;
			
		}

	
		public void Disconnect()
		{
			this.conn.Disconnect();
		
		}

		public NetworkConnection Conn {

			get { return this.conn; }
		
		
		}


		public bool IsBanned
		{
			get
			{
				return BanHandler.QueryBan(null, this.IpAddress).Value != null;
			}
		}

	
		private NetworkConnection conn;
	}
}

