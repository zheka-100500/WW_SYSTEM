using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Attributes
{
	public class PluginDetails : Attribute
	{
		
		public int WWSYSTEMMajor;

		
		public int WWSYSTEMMinor;

	
		public int WWSYSTEMRevision;

		
		public string id;


		public string name;


		public string author;

	
		public string description;

	
		public string version;
	}
}
