using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Attributes;
using WW_SYSTEM.Events;
using UnityEngine;
namespace WW_SYSTEM
{
	public class PluginManager
	{
		
		public static string GetWWSYSTEMVersion()
		{
			return string.Format("{0}.{1}.{2}", WW_SYSTEM_MAJOR, WW_SYSTEM_MINOR, WW_SYSTEM_REVISION);
		}
		public const int WW_SYSTEM_MAJOR = 12;


		public const int WW_SYSTEM_MINOR = 4;


		public const int WW_SYSTEM_REVISION = 7;

		public static string GetWWSYSTEMBuild()
		{
			return "R";
		}

		
		public List<Plugin> EnabledPlugins
		{
			get
			{
				List<Plugin> list = new List<Plugin>();
				foreach (KeyValuePair<string, Plugin> keyValuePair in this.enabledPlugins)
				{
					list.Add(keyValuePair.Value);
				}
				return list;
			}
		}

		
		public List<Plugin> DisabledPlugins
		{
			get
			{
				List<Plugin> list = new List<Plugin>();
				foreach (KeyValuePair<string, Plugin> keyValuePair in this.disabledPlugins)
				{
					list.Add(keyValuePair.Value);
				}
				return list;
			}
		}

		
		public List<Plugin> Plugins
		{
			get
			{
				List<Plugin> list = new List<Plugin>();
				foreach (KeyValuePair<string, Plugin> keyValuePair in this.disabledPlugins)
				{
					list.Add(keyValuePair.Value);
				}
				foreach (KeyValuePair<string, Plugin> keyValuePair2 in this.enabledPlugins)
				{
					list.Add(keyValuePair2.Value);
				}
				return list;
			}
		}

	



		



		
		public static PluginManager Manager
		{
			get
			{

				return MainLoader.manager;
			}
		}

		
		public PluginManager()
		{
			this.enabledPlugins = new Dictionary<string, Plugin>();
			this.disabledPlugins = new Dictionary<string, Plugin>();
			
		}

	
		public Plugin GetEnabledPlugin(string id)
		{
			Plugin result;
			this.enabledPlugins.TryGetValue(id, out result);
			return result;
		}

	
		public Plugin GetDisabledPlugin(string id)
		{
			Plugin result;
			this.disabledPlugins.TryGetValue(id, out result);
			return result;
		}


		public Plugin GetPlugin(string id)
		{
			return this.GetEnabledPlugin(id) ?? this.GetDisabledPlugin(id);
		}



		public List<Plugin> GetAllPlugins() {



			IEnumerable<Plugin> searchable = this.enabledPlugins.Values;
			List<Plugin> list = new List<Plugin>();
			foreach (Plugin pl in searchable)
			{
				list.Add(pl);
			}
			return list;


		}


		private List<Plugin> GetPluginsOfSearchable(IEnumerable<Plugin> searchable, string query, SearchFlags flags)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			Array values = Enum.GetValues(typeof(SearchFlags));
			List<Plugin> list = new List<Plugin>();
			foreach (Plugin plugin in searchable)
			{
				if (plugin.Details != null)
				{
					foreach (object obj in values)
					{
						SearchFlags searchFlags = (SearchFlags)obj;
						if (flags.HasFlag(searchFlags))
						{
							bool flag;
							switch (searchFlags)
							{
								case SearchFlags.NAME:
									flag = (plugin.Details.name == query);
									break;
								case SearchFlags.AUTHOR:
									flag = (plugin.Details.author == query);
									break;
								case SearchFlags.NAME | SearchFlags.AUTHOR:
									goto IL_158;
								case SearchFlags.ID:
									flag = (plugin.Details.id == query);
									break;
								default:
									if (searchFlags != SearchFlags.DESCRIPTION)
									{
										if (searchFlags != SearchFlags.VERSION)
										{
											goto IL_158;
										}
										flag = (plugin.Details.version == query);
									}
									else
									{
										string description = plugin.Details.description;
										int? num = (description != null) ? new int?(description.Length / 3) : null;
										int length = query.Length;
										flag = ((num.GetValueOrDefault() <= length & num != null) && plugin.Details.description.Contains(query));
									}
									break;
							}
						IL_15B:
							if (flag)
							{
								list.Add(plugin);
								break;
							}
							continue;
						IL_158:
							flag = false;
							goto IL_15B;
						}
					}
				}
			}
			return list;
		}


		public List<Plugin> GetEnabledPlugins(string query, SearchFlags flags)
		{
			return this.GetPluginsOfSearchable(this.enabledPlugins.Values, query, flags);
		}

	
		public List<Plugin> GetDisabledPlugins(string query, SearchFlags flags)
		{
			return this.GetPluginsOfSearchable(this.disabledPlugins.Values, query, flags);
		}

		
		public List<Plugin> GetMatchingPlugins(string query, SearchFlags flags)
		{
			List<Plugin> pluginsOfSearchable = this.GetPluginsOfSearchable(this.enabledPlugins.Values, query, flags);
			foreach (Plugin item in this.GetPluginsOfSearchable(this.disabledPlugins.Values, query, flags))
			{
				if (!pluginsOfSearchable.Contains(item))
				{
					pluginsOfSearchable.Add(item);
				}
			}
			return pluginsOfSearchable;
		}

	
		[Obsolete("Use GetEnabledPlugins instead.")]
		public List<Plugin> FindEnabledPlugins(string name)
		{
			List<Plugin> list = new List<Plugin>();
			foreach (Plugin plugin in this.enabledPlugins.Values)
			{
				if (plugin.Details.name.Contains(name) || plugin.Details.author.Contains(name))
				{
					list.Add(plugin);
				}
			}
			return list;
		}

		
		[Obsolete("Use GetDisabledPlugins instead.")]
		public List<Plugin> FindDisabledPlugins(string name)
		{
			List<Plugin> list = new List<Plugin>();
			foreach (Plugin plugin in this.disabledPlugins.Values)
			{
				if (plugin.Details.name.Contains(name) || plugin.Details.author.Contains(name))
				{
					list.Add(plugin);
				}
			}
			return list;
		}

		
		[Obsolete("Use GetMatchingPlugins instead.")]
		public List<Plugin> FindPlugins(string name)
		{
			List<Plugin> list = new List<Plugin>();
			foreach (Plugin plugin in this.enabledPlugins.Values)
			{
				if (plugin.Details.name.Contains(name) || plugin.Details.author.Contains(name))
				{
					list.Add(plugin);
				}
			}
			foreach (Plugin plugin2 in this.disabledPlugins.Values)
			{
				if (plugin2.Details.name.Contains(name) || plugin2.Details.author.Contains(name))
				{
					list.Add(plugin2);
				}
			}
			return list;
		}

	
		public void EnablePlugins(CommandShell commandShell)
		{
			Plugin[] array = new Plugin[this.disabledPlugins.Count];
			this.disabledPlugins.Values.CopyTo(array, 0);
			foreach (Plugin plugin in array)
			{
				this.EnablePlugin(plugin);
			}
			commandShell.RegisterCommands();
		}

		
		public void EnablePlugin(Plugin plugin)
		{
			if (this.enabledPlugins.ContainsValue(plugin))
			{
				return;
			}
			Logger.Info("PLUGIN_MANAGER", "Enabling plugin " + plugin.Details.name + " " + plugin.Details.version);

			
			Logger.Debug("PLUGIN_MANAGER", "Loading event snapshot");
			EventManager.Manager.AddSnapshotEventHandlers(plugin);
		
			Logger.Debug("PLUGIN_MANAGER", "Invoking OnEnable");
			plugin.OnEnable();
			Logger.Debug("PLUGIN_MANAGER", "Altering dictionaries");
			this.disabledPlugins.Remove(plugin.Details.id);
			this.enabledPlugins.Add(plugin.Details.id, plugin);
			Logger.Info("PLUGIN_MANAGER", "Enabled plugin  " + plugin.Details.name + " " + plugin.Details.version);
		}

		
		public void DisablePlugins()
		{
			Plugin[] array = new Plugin[this.disabledPlugins.Count];
			this.disabledPlugins.Values.CopyTo(array, 0);
			foreach (Plugin plugin in array)
			{
				this.DisablePlugin(plugin);
			}
		}

	
		public void DisablePlugin(string id)
		{
			Dictionary<string, Plugin> dictionary = new Dictionary<string, Plugin>();
			foreach (KeyValuePair<string, Plugin> keyValuePair in this.enabledPlugins)
			{
				if (keyValuePair.Value.Details.id == id)
				{
					this.DisablePlugin(keyValuePair.Value);
				}
				else
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			this.enabledPlugins = dictionary;
		}

	
		public void DisablePlugin(Plugin plugin)
		{
			if (this.disabledPlugins.ContainsValue(plugin))
			{
				return;
			}
			Logger.Info("PLUGIN_MANAGER", "Disabling plugin " + plugin.Details.name + " " + plugin.Details.version);
			Logger.Debug("PLUGIN_MANAGER", "Altering dictionaries");
			this.enabledPlugins.Remove(plugin.Details.id);
			this.disabledPlugins.Add(plugin.Details.id, plugin);
			Logger.Debug("PLUGIN_MANAGER", "Invoking OnDisable");
			plugin.OnDisable();
			
			Logger.Debug("PLUGIN_MANAGER", "Unloading event handlers");
			EventManager.Manager.RemoveEventHandlers(plugin);
		
		
			Logger.Info("PLUGIN_MANAGER", "Disabled plugin " + plugin.Details.name + " " + plugin.Details.version);
		}

	
		public void LoadPlugins(string dir)
		{
			
			this.LoadDirectoryPlugins(dir);
			this.LoadPluginAssemblies(dir);
		}

		
		public void LoadPluginAssemblies(string dir)
		{
			foreach (string text in Directory.GetFiles(dir))
			{
				if (text.EndsWith(".dll"))
				{
					Logger.Info("PLUGIN_LOADER", text);
					this.LoadPluginAssembly(text);
				}
			}
		}

		
		public void LoadDirectoryPlugins(string pluginDirectory)
		{
			string text = pluginDirectory + PluginManager.DEPENDENCY_FOLDER;
			if (Directory.Exists(text))
			{
				foreach (string text2 in Directory.GetFiles(text))
				{
					if (text2.Contains(".dll"))
					{
						Logger.Info("PLUGIN_LOADER", "Loading plugin dependency: " + text2);
						try
						{
							this.LoadAssembly(text2);
						}
						catch (Exception ex)
						{
							Logger.Warn("PLUGIN_LOADER", "Failed to load dependency: " + text2);
							Logger.Debug("PLUGIN_LOADER", ex.Message);
							Logger.Debug("PLUGIN_LOADER", ex.StackTrace);
						}
					}
				}
				return;
			}
			Logger.Debug("PLUGIN_LOADER", "No dependencies for directory: " + text);
		}


		public void LoadAssembly(string path)
		{
			Assembly.LoadFrom(path);
		}

		public void LoadPluginAssembly(string path)
		{
			Logger.Debug("PLUGIN_LOADER", path);
			Assembly assembly = Assembly.LoadFrom(path);
			try
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsSubclassOf(typeof(Plugin)) && type != typeof(Plugin))
					{
						try
						{
							Plugin plugin = (Plugin)Activator.CreateInstance(type);
							PluginDetails pluginDetails = (PluginDetails)Attribute.GetCustomAttribute(type, typeof(PluginDetails));
							if (pluginDetails.id != null)
							{
								if (pluginDetails.WWSYSTEMMajor != WW_SYSTEM_MAJOR && pluginDetails.WWSYSTEMMinor != WW_SYSTEM_MINOR)
								{
									Logger.Error("PLUGIN_LOADER", "Failed to load an outdated plugin " + pluginDetails.name + " " + pluginDetails.version + " plugin WW SYSTEM version: " + pluginDetails.WWSYSTEMMajor + "." + pluginDetails.WWSYSTEMMinor + "." + pluginDetails.WWSYSTEMRevision + " Current WW SYSTEM version: " + WW_SYSTEM_MAJOR + "." + WW_SYSTEM_MINOR + "." + WW_SYSTEM_REVISION);
									return;
								}
								else
								{
									plugin.Details = pluginDetails;
								
									plugin.Register();
									this.disabledPlugins.Add(pluginDetails.id, plugin);
									Logger.Info("PLUGIN_LOADER", "Plugin loaded: " + plugin.ToString());
								}
							}
							else
							{
								
								string tag = "PLUGIN_LOADER";
								string[] array = new string[5];
								array[0] = "Plugin loaded but missing an id: ";
								int num = 1;
								Type type2 = type;
								array[num] = ((type2 != null) ? type2.ToString() : null);
								array[2] = "[";
								array[3] = path;
								array[4] = "]";
								Logger.Warn(tag, string.Concat(array));
							}
						}
						catch (Exception ex)
						{
							
							string tag2 = "PLUGIN_LOADER";
							string[] array2 = new string[5];
							array2[0] = "Failed to create instance of plugin ";
							int num2 = 1;
							Type type3 = type;
							array2[num2] = ((type3 != null) ? type3.ToString() : null);
							array2[2] = "[";
							array2[3] = path;
							array2[4] = "]";
							Logger.Error(tag2, string.Concat(array2));
							Logger.Error("PLUGIN_LOADER", ex.GetType().Name + ": " + ex.Message);
							Logger.Error("PLUGIN_LOADER", ex.StackTrace);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("PLUGIN_LOADER", "Failed to load DLL [" + path + "], is it up to date?");
				Logger.Error("PLUGIN_LOADER", ex2.Message);
				Logger.Error("PLUGIN_LOADER", ex2.StackTrace);
			}
		}

		
		public static string ToUpperSnakeCase(string otherCase)
		{
			string text = "";
			for (int i = 0; i < otherCase.Length; i++)
			{
				if (text.Length != 0 || otherCase[i] != '_')
				{
					if (i > 0 && char.IsUpper(otherCase[i]) && otherCase[i - 1] != '_')
					{
						text = text + "_" + otherCase[i].ToString();
					}
					else
					{
						text += char.ToUpper(otherCase[i]).ToString();
					}
				}
			}
			return text;
		}

	
		public void RefreshPluginAttributes()
		{
			foreach (Plugin plugin in this.enabledPlugins.Values)
			{
				
				
			}
		}

		


	
		public const string WW_SYSTEM_BUILD = "A";

	
		public static readonly string DEPENDENCY_FOLDER = "dependencies";

	
		private Dictionary<string, Plugin> enabledPlugins;

	
		private Dictionary<string, Plugin> disabledPlugins;

	
	

	






	
	
	
}
}
