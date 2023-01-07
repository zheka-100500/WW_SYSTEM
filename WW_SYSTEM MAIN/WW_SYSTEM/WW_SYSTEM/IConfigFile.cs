using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Config;

namespace WW_SYSTEM
{
	public interface IConfigFile
	{
	
		[Obsolete("Use GetConfigPath(ConfigType type)")]
		string GetConfigPath();


		string GetConfigPath(ConfigType type);

		
		[Obsolete("Use GetIntValue(Smod2.Config.ConfigType type, string key, int def, bool randomValues = false)", false)]
		int GetIntValue(string key, int def, bool randomValues = false);

	
		int GetIntValue(ConfigType type, string key, int def, bool randomValues = false);


		[Obsolete("Use GetFloatValue(Smod2.Config.ConfigType type, string key, float def, bool randomValues = false)", false)]
		float GetFloatValue(string key, float def, bool randomValues = false);

	
		float GetFloatValue(ConfigType type, string key, float def, bool randomValues = false);

		
		[Obsolete("Use GetStringValue(Smod2.Config.ConfigType type, string key, string def, bool replaceNewlines = true, bool randomValues = false)", false)]
		string GetStringValue(string key, string def, bool randomValues = false);


		string GetStringValue(ConfigType type, string key, string def, bool replaceNewlines = true, bool randomValues = false);


		[Obsolete("Use GetBoolValue(Smod2.Config.ConfigType type, string key, bool def, bool randomValues = false)", false)]
		bool GetBoolValue(string key, bool def, bool randomValues = false);


		bool GetBoolValue(ConfigType type, string key, bool def, bool randomValues = false);


		[Obsolete("Use GetListValue(Smod2.Config.ConfigType type, string key, string def[], bool randomValues = false)", false)]
		string[] GetListValue(string key, string[] def, bool randomValues = false);


		string[] GetListValue(ConfigType type, string key, string[] def, bool randomValues = false);

	
		[Obsolete("Use GetListValue(Smod2.Config.ConfigType type, string key, bool randomValues = false)", false)]
		string[] GetListValue(string key, bool randomValues = false);


		string[] GetListValue(ConfigType type, string key, bool randomValues = false);


		[Obsolete("Use GetIntListValue(Smod2.Config.ConfigType type, string key, int[] def, bool randomValues = false)", false)]
		int[] GetIntListValue(string key, int[] def, bool randomValues = false);

		
		int[] GetIntListValue(ConfigType type, string key, int[] def, bool randomValues = false);

	
		[Obsolete("Use GetIntListValue(Smod2.Config.ConfigType type, string key, bool randomValues = false)", false)]
		int[] GetIntListValue(string key, bool randomValues = false);


		int[] GetIntListValue(ConfigType type, string key, bool randomValues = false);


		[Obsolete("Use GetDickValue(Smod2.Config.ConfigType type, string key, Dictionary<string, string> def, bool randomValues = false, char splitChar = ':')", false)]
		Dictionary<string, string> GetDictValue(string key, Dictionary<string, string> def, bool randomValues = false, char splitChar = ':');


		Dictionary<string, string> GetDickValue(ConfigType type, string key, Dictionary<string, string> def, bool randomValues = false, char splitChar = ':');

	
		[Obsolete("Use GetDictValue(Smod2.Config.ConfigType type, string key, bool randomValues = false, char splitChar = ':')", false)]
		Dictionary<string, string> GetDictValue(string key, bool randomValues = false, char splitChar = ':');

	
		Dictionary<string, string> GetDictValue(ConfigType type, string key, bool randomValues = false, char splitChar = ':');

		
		[Obsolete("Use GetIntDictValue(Smod2.Config.ConfigType type, string key, Dictionary<int, int> def, bool randomValues = false, char splitChar = ':')", false)]
		Dictionary<int, int> GetIntDictValue(string key, Dictionary<int, int> def, bool randomValues = false, char splitChar = ':');

	
		Dictionary<int, int> GetIntDictValue(ConfigType type, string key, Dictionary<int, int> def, bool randomValues = false, char splitChar = ':');


		[Obsolete("Use GetIntDictValue(Smod2.Config.ConfigType type, string key, bool randomValues = false, char splitChar = ':')", false)]
		Dictionary<int, int> GetIntDictValue(string key, bool randomValues = false, char splitChar = ':');

		
		Dictionary<int, int> GetIntDictValue(ConfigType type, string key, bool randomValues = false, char splitChar = ':');
	}
}
