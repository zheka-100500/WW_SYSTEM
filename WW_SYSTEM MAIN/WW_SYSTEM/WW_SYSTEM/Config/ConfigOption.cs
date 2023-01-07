using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Config
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ConfigOption : Attribute
	{
		
		public string Key { get; }

	
		public string Description { get; }

		
		public bool PrimaryUser { get; }

	
		
		public bool Randomized { get; }

	
		public ConfigOption(bool primaryUser = true, bool randomized = false)
		{
			this.PrimaryUser = primaryUser;
			this.Randomized = randomized;
		}

	
		public ConfigOption(string key, bool primaryUser = true, bool randomized = false) : this(primaryUser, randomized)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentException("Config keys cannot be null, whitespace, or empty.", "key");
			}
			this.Key = key;
		}


		public ConfigOption(string key, string description, bool primaryUser = true, bool randomized = false) : this(key, primaryUser, randomized)
		{
			if (description == null)
			{
				throw new ArgumentNullException("description");
			}
			this.Description = description;
		}
	}
}
