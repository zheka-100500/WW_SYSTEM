using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Config
{
	[Obsolete("Use ConfigSetting.Default.GetType() instead.", true)]
	public enum SettingType
	{
		
		NUMERIC,
	
		FLOAT,
	
		STRING,
	
		BOOL,
		
		LIST,

		NUMERIC_LIST,

		DICTIONARY,
	
		NUMERIC_DICTIONARY
	}
}
