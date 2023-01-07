using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.ConfigHandler;
using System.Globalization;
using System.IO;

using Scp914;
using UnityEngine;
namespace WW_SYSTEM.Config
{
    public class PluginConfig : ConfigRegister
	{


		public string[] RawData
		{
			get
			{
				if (!this._virtual)
				{
					return this._rawData;
				}
				return this._rawDataUnfiltered;
			}
			set
			{
				if (this._virtual)
				{
					this._rawDataUnfiltered = value;
					return;
				}
				this._rawData = value;
			}
		}

		
		public bool IsVirtual
		{
			get
			{
				return this._virtual;
			}
			set
			{
				if (!value || this._virtual)
				{
					return;
				}
				this._virtual = true;
				this._rawDataUnfiltered = this.RawData;
			}
		}

	
		public PluginConfig()
		{
			this.RawData = new string[0];
		}

	
		public PluginConfig(string path)
		{
			if (!Directory.Exists(FileManager.GetAppFolder(true, true, "") + "Plugins_Configs"))
				Directory.CreateDirectory(FileManager.GetAppFolder(true, true, "") + "Plugins_Configs");

			if (!File.Exists(path))
				File.WriteAllText(path, "#CONFIG FILE VERSION: " + PluginManager.GetWWSYSTEMVersion());

			this.Path = path;
			this.LoadConfigFile(path);
		}


		public override void UpdateConfigValue(ConfigEntry configEntry)
		{
			if (configEntry == null)
			{
				throw new NullReferenceException("Config type unsupported (Config: Null).");
			}
			if (configEntry != null)
			{
				ConfigEntry<bool> configEntry2;
				if ((configEntry2 = (configEntry as ConfigEntry<bool>)) != null)
				{
					ConfigEntry<bool> configEntry3 = configEntry2;
					configEntry3.Value = this.GetBool(configEntry3.Key, configEntry3.Default);
					return;
				}
				ConfigEntry<byte> configEntry4;
				if ((configEntry4 = (configEntry as ConfigEntry<byte>)) != null)
				{
					ConfigEntry<byte> configEntry5 = configEntry4;
					configEntry5.Value = this.GetByte(configEntry5.Key, configEntry5.Default);
					return;
				}
				ConfigEntry<char> configEntry6;
				if ((configEntry6 = (configEntry as ConfigEntry<char>)) != null)
				{
					ConfigEntry<char> configEntry7 = configEntry6;
					configEntry7.Value = this.GetChar(configEntry7.Key, configEntry7.Default);
					return;
				}
				ConfigEntry<decimal> configEntry8;
				if ((configEntry8 = (configEntry as ConfigEntry<decimal>)) != null)
				{
					ConfigEntry<decimal> configEntry9 = configEntry8;
					configEntry9.Value = this.GetDecimal(configEntry9.Key, configEntry9.Default);
					return;
				}
				ConfigEntry<double> configEntry10;
				if ((configEntry10 = (configEntry as ConfigEntry<double>)) != null)
				{
					ConfigEntry<double> configEntry11 = configEntry10;
					configEntry11.Value = this.GetDouble(configEntry11.Key, configEntry11.Default);
					return;
				}
				ConfigEntry<float> configEntry12;
				if ((configEntry12 = (configEntry as ConfigEntry<float>)) != null)
				{
					ConfigEntry<float> configEntry13 = configEntry12;
					configEntry13.Value = this.GetFloat(configEntry13.Key, configEntry13.Default);
					return;
				}
				ConfigEntry<int> configEntry14;
				if ((configEntry14 = (configEntry as ConfigEntry<int>)) != null)
				{
					ConfigEntry<int> configEntry15 = configEntry14;
					configEntry15.Value = this.GetInt(configEntry15.Key, configEntry15.Default);
					return;
				}
				ConfigEntry<long> configEntry16;
				if ((configEntry16 = (configEntry as ConfigEntry<long>)) != null)
				{
					ConfigEntry<long> configEntry17 = configEntry16;
					configEntry17.Value = this.GetLong(configEntry17.Key, configEntry17.Default);
					return;
				}
				ConfigEntry<sbyte> configEntry18;
				if ((configEntry18 = (configEntry as ConfigEntry<sbyte>)) != null)
				{
					ConfigEntry<sbyte> configEntry19 = configEntry18;
					configEntry19.Value = this.GetSByte(configEntry19.Key, configEntry19.Default);
					return;
				}
				ConfigEntry<short> configEntry20;
				if ((configEntry20 = (configEntry as ConfigEntry<short>)) != null)
				{
					ConfigEntry<short> configEntry21 = configEntry20;
					configEntry21.Value = this.GetShort(configEntry21.Key, configEntry21.Default);
					return;
				}
				ConfigEntry<string> configEntry22;
				if ((configEntry22 = (configEntry as ConfigEntry<string>)) != null)
				{
					ConfigEntry<string> configEntry23 = configEntry22;
					configEntry23.Value = this.GetString(configEntry23.Key, configEntry23.Default);
					return;
				}
				ConfigEntry<uint> configEntry24;
				if ((configEntry24 = (configEntry as ConfigEntry<uint>)) != null)
				{
					ConfigEntry<uint> configEntry25 = configEntry24;
					configEntry25.Value = this.GetUInt(configEntry25.Key, configEntry25.Default);
					return;
				}
				ConfigEntry<ulong> configEntry26;
				if ((configEntry26 = (configEntry as ConfigEntry<ulong>)) != null)
				{
					ConfigEntry<ulong> configEntry27 = configEntry26;
					configEntry27.Value = this.GetULong(configEntry27.Key, configEntry27.Default);
					return;
				}
				ConfigEntry<ushort> configEntry28;
				if ((configEntry28 = (configEntry as ConfigEntry<ushort>)) == null)
				{
					ConfigEntry<List<bool>> configEntry29;
					if ((configEntry29 = (configEntry as ConfigEntry<List<bool>>)) == null)
					{
						ConfigEntry<List<byte>> configEntry30;
						if ((configEntry30 = (configEntry as ConfigEntry<List<byte>>)) == null)
						{
							ConfigEntry<List<char>> configEntry31;
							if ((configEntry31 = (configEntry as ConfigEntry<List<char>>)) == null)
							{
								ConfigEntry<List<decimal>> configEntry32;
								if ((configEntry32 = (configEntry as ConfigEntry<List<decimal>>)) == null)
								{
									ConfigEntry<List<double>> configEntry33;
									if ((configEntry33 = (configEntry as ConfigEntry<List<double>>)) == null)
									{
										ConfigEntry<List<float>> configEntry34;
										if ((configEntry34 = (configEntry as ConfigEntry<List<float>>)) == null)
										{
											ConfigEntry<List<int>> configEntry35;
											if ((configEntry35 = (configEntry as ConfigEntry<List<int>>)) == null)
											{
												ConfigEntry<List<long>> configEntry36;
												if ((configEntry36 = (configEntry as ConfigEntry<List<long>>)) == null)
												{
													ConfigEntry<List<sbyte>> configEntry37;
													if ((configEntry37 = (configEntry as ConfigEntry<List<sbyte>>)) == null)
													{
														ConfigEntry<List<short>> configEntry38;
														if ((configEntry38 = (configEntry as ConfigEntry<List<short>>)) == null)
														{
															ConfigEntry<List<string>> configEntry39;
															if ((configEntry39 = (configEntry as ConfigEntry<List<string>>)) == null)
															{
																ConfigEntry<List<uint>> configEntry40;
																if ((configEntry40 = (configEntry as ConfigEntry<List<uint>>)) == null)
																{
																	ConfigEntry<List<ulong>> configEntry41;
																	if ((configEntry41 = (configEntry as ConfigEntry<List<ulong>>)) == null)
																	{
																		ConfigEntry<List<ushort>> configEntry42;
																		if ((configEntry42 = (configEntry as ConfigEntry<List<ushort>>)) == null)
																		{
																			ConfigEntry<Dictionary<string, string>> configEntry43;
																			if ((configEntry43 = (configEntry as ConfigEntry<Dictionary<string, string>>)) == null)
																			{
																				ConfigEntry<Scp914Mode> configEntry44;
																				if ((configEntry44 = (configEntry as ConfigEntry<Scp914Mode>)) == null)
																				{
																					goto IL_8CD;
																				}
																				ConfigEntry<Scp914Mode> configEntry45 = configEntry44;
																				string @string = this.GetString(configEntry45.Key, "");
																				Scp914Mode value;
																				if (@string == "default" || !Enum.TryParse<Scp914Mode>(@string, out value))
																				{
																					configEntry45.Value = configEntry45.Default;
																					return;
																				}
																				configEntry45.Value = value;
																				return;
																			}
																			else
																			{
																				ConfigEntry<Dictionary<string, string>> configEntry46 = configEntry43;
																				configEntry46.Value = this.GetStringDictionary(configEntry46.Key);
																				if (configEntry46.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry46.Key), "default", StringComparison.OrdinalIgnoreCase))
																				{
																					configEntry46.Value = configEntry46.Default;
																					return;
																				}
																			}
																		}
																		else
																		{
																			ConfigEntry<List<ushort>> configEntry47 = configEntry42;
																			configEntry47.Value = this.GetUShortList(configEntry47.Key);
																			if (configEntry47.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry47.Key), "default", StringComparison.OrdinalIgnoreCase))
																			{
																				configEntry47.Value = configEntry47.Default;
																				return;
																			}
																		}
																	}
																	else
																	{
																		ConfigEntry<List<ulong>> configEntry48 = configEntry41;
																		configEntry48.Value = this.GetULongList(configEntry48.Key);
																		if (configEntry48.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry48.Key), "default", StringComparison.OrdinalIgnoreCase))
																		{
																			configEntry48.Value = configEntry48.Default;
																			return;
																		}
																	}
																}
																else
																{
																	ConfigEntry<List<uint>> configEntry49 = configEntry40;
																	configEntry49.Value = this.GetUIntList(configEntry49.Key);
																	if (configEntry49.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry49.Key), "default", StringComparison.OrdinalIgnoreCase))
																	{
																		configEntry49.Value = configEntry49.Default;
																		return;
																	}
																}
															}
															else
															{
																ConfigEntry<List<string>> configEntry50 = configEntry39;
																configEntry50.Value = this.GetStringList(configEntry50.Key);
																if (configEntry50.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry50.Key), "default", StringComparison.OrdinalIgnoreCase))
																{
																	configEntry50.Value = configEntry50.Default;
																	return;
																}
															}
														}
														else
														{
															ConfigEntry<List<short>> configEntry51 = configEntry38;
															configEntry51.Value = this.GetShortList(configEntry51.Key);
															if (configEntry51.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry51.Key), "default", StringComparison.OrdinalIgnoreCase))
															{
																configEntry51.Value = configEntry51.Default;
																return;
															}
														}
													}
													else
													{
														ConfigEntry<List<sbyte>> configEntry52 = configEntry37;
														configEntry52.Value = this.GetSByteList(configEntry52.Key);
														if (configEntry52.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry52.Key), "default", StringComparison.OrdinalIgnoreCase))
														{
															configEntry52.Value = configEntry52.Default;
															return;
														}
													}
												}
												else
												{
													ConfigEntry<List<long>> configEntry53 = configEntry36;
													configEntry53.Value = this.GetLongList(configEntry53.Key);
													if (configEntry53.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry53.Key), "default", StringComparison.OrdinalIgnoreCase))
													{
														configEntry53.Value = configEntry53.Default;
														return;
													}
												}
											}
											else
											{
												ConfigEntry<List<int>> configEntry54 = configEntry35;
												configEntry54.Value = this.GetIntList(configEntry54.Key);
												if (configEntry54.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry54.Key), "default", StringComparison.OrdinalIgnoreCase))
												{
													configEntry54.Value = configEntry54.Default;
													return;
												}
											}
										}
										else
										{
											ConfigEntry<List<float>> configEntry55 = configEntry34;
											configEntry55.Value = this.GetFloatList(configEntry55.Key);
											if (configEntry55.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry55.Key), "default", StringComparison.OrdinalIgnoreCase))
											{
												configEntry55.Value = configEntry55.Default;
												return;
											}
										}
									}
									else
									{
										ConfigEntry<List<double>> configEntry56 = configEntry33;
										configEntry56.Value = this.GetDoubleList(configEntry56.Key);
										if (configEntry56.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry56.Key), "default", StringComparison.OrdinalIgnoreCase))
										{
											configEntry56.Value = configEntry56.Default;
											return;
										}
									}
								}
								else
								{
									ConfigEntry<List<decimal>> configEntry57 = configEntry32;
									configEntry57.Value = this.GetDecimalList(configEntry57.Key);
									if (configEntry57.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry57.Key), "default", StringComparison.OrdinalIgnoreCase))
									{
										configEntry57.Value = configEntry57.Default;
										return;
									}
								}
							}
							else
							{
								ConfigEntry<List<char>> configEntry58 = configEntry31;
								configEntry58.Value = this.GetCharList(configEntry58.Key);
								if (configEntry58.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry58.Key), "default", StringComparison.OrdinalIgnoreCase))
								{
									configEntry58.Value = configEntry58.Default;
									return;
								}
							}
						}
						else
						{
							ConfigEntry<List<byte>> configEntry59 = configEntry30;
							configEntry59.Value = this.GetByteList(configEntry59.Key);
							if (configEntry59.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry59.Key), "default", StringComparison.OrdinalIgnoreCase))
							{
								configEntry59.Value = configEntry59.Default;
								return;
							}
						}
					}
					else
					{
						ConfigEntry<List<bool>> configEntry60 = configEntry29;
						configEntry60.Value = this.GetBoolList(configEntry60.Key);
						if (configEntry60.Value.Count <= 0 && string.Equals(this.GetRawString(configEntry60.Key), "default", StringComparison.OrdinalIgnoreCase))
						{
							configEntry60.Value = configEntry60.Default;
							return;
						}
					}
					return;
				}
				ConfigEntry<ushort> configEntry61 = configEntry28;
				configEntry61.Value = this.GetUShort(configEntry61.Key, configEntry61.Default);
				return;
			}
		IL_8CD:
			throw new Exception(string.Concat(new string[]
			{
			"Config type unsupported (Config: Key = \"",
			configEntry.Key ?? "Null",
			"\" Type = \"",
			configEntry.ValueType.FullName ?? "Null",
			"\" Name = \"",
			configEntry.Name ?? "Null",
			"\" Description = \"",
			configEntry.Description ?? "Null",
			"\")."
			}));
		}

		
		private static string[] Filter(IEnumerable<string> lines)
		{
			return (from line in lines
					where !string.IsNullOrEmpty(line) && !line.StartsWith("#") && (line.StartsWith(" - ") || line.Contains(':'))
					select line).ToArray<string>();
		}


		public void LoadConfigFile(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			this.Path = path;
	
			this._rawDataUnfiltered = FileManager.ReadAllLines(path);
			this.RawData = PluginConfig.Filter(this._rawDataUnfiltered);
			if (ServerStatic.IsDedicated && this.Path.EndsWith("config_remoteadmin.txt"))
			{
				Application.targetFrameRate = this.GetInt("server_tickrate", 60);
			}
			base.UpdateRegisteredConfigValues();
		}

		
		private static void RemoveDeprecated(string path)
		{
			List<string> list = FileManager.ReadAllLinesList(path);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				for (int j = 0; j < PluginConfig._deprecatedconfigs.Length; j++)
				{
					if (list[i].StartsWith(PluginConfig._deprecatedconfigs[j] + ":") && (i == 0 || list[i - 1] != "#REMOVED FROM GAME - REDUNDANT"))
					{
						list.Insert(i, "#REMOVED FROM GAME - REDUNDANT");
					}
				}
			}
			FileManager.WriteToFile(list, path, false);
		}

		

		private static void AddMissingTemplateKeys(string templatepath, string path, ref bool _afteradding)
		{
			string text = TimeBehaviour.FormatTime("yyyy/MM/dd HH:mm:ss");
			string text2 = FileManager.ReadAllText(path);
			string[] array = FileManager.ReadAllLines(templatepath);
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && array[i].Contains(":") && ((i + 1 < array.Length && array[i + 1].StartsWith(" -")) || array[i].EndsWith("[]")))
				{
					list.Add(array[i]);
				}
				else if (!array[i].StartsWith("#") && array[i].Contains(":") && !array[i].StartsWith(" -"))
				{
					list2.Add(array[i].Substring(0, array[i].IndexOf(':') + 1));
				}
			}
			foreach (string text3 in list2)
			{
				if (!text2.Contains(text3))
				{
					list3.Add(text3 + " default");
				}
			}
			PluginConfig.Write(list3, path, ref text);
			foreach (string text4 in list)
			{
				if (!text2.Contains(text4))
				{
					bool flag = false;
					List<string> list4 = new List<string>
				{
					"#LIST",
					text4
				};
					foreach (string text5 in array)
					{
						if (text5.StartsWith(text4) && text5.EndsWith("[]"))
						{
							list4.Clear();
							list4.AddRange(new string[]
							{
							"#LIST - [] equals to empty",
							text5
							});
							break;
						}
						if (text5.StartsWith(text4))
						{
							flag = true;
						}
						else if (flag)
						{
							if (text5.StartsWith(" - "))
							{
								list4.Add(text5);
							}
							else if (!text5.StartsWith("#"))
							{
								break;
							}
						}
					}
					PluginConfig.Write(list4, path, ref text);
				}
			}
			_afteradding = true;
		}

		
		private static void Write(IEnumerable<string> text, string path, ref string time)
		{
			string[] array = text.ToArray<string>();
			if (array.Length != 0)
			{
				PluginConfig.Write(string.Join("\r\n", array), path, ref time);
			}
		}

	
		private static void Write(string text, string path, ref string time)
		{
			using (StreamWriter streamWriter = File.AppendText(path))
			{
				streamWriter.Write(string.Concat(new string[]
				{
				"\r\n\r\n#ADDED BY CONFIG VALIDATOR - ",
				time,
				" Game version: ",
				CustomNetworkManager._expectedGameFilesVersion.ToString(),
				"\r\n",
				text
				}));
			}
		}

		private static void RemoveInvalid(string path)
		{
			string[] array = FileManager.ReadAllLines(path);
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && !array[i].Contains(":") && !string.IsNullOrEmpty(array[i].Replace(" ", "")))
				{
					flag = true;
					array[i] = "#INVALID - " + array[i];
				}
			}
			if (flag)
			{
				FileManager.WriteToFile(array, path, false);
			}
		}


		private void CommentInvalid(string key, string type)
		{
			if (this.IsVirtual)
			{
				return;
			}
			for (int i = 0; i < this._rawDataUnfiltered.Length; i++)
			{
				if (this._rawDataUnfiltered[i].StartsWith(key + ": "))
				{
					this._rawDataUnfiltered[i] = "#INVALID " + type + " - " + this._rawDataUnfiltered[i];
				}
			}
		
				FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
			
		}

	
		public bool Reload()
		{
			if (this.IsVirtual)
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.Path))
			{
				return false;
			}
			this.LoadConfigFile(this.Path);
			return true;
		}

		
		private string GetRawString(string key)
		{
			foreach (string text in this.RawData)
			{
				if (text.StartsWith(key + ": "))
				{
					return text.Substring(key.Length + 2);
				}
			}
			return "default";
		}


		public void SetString(string key, string value = null)
		{
			this.Reload();
			int num = 0;
			List<string> list = null;
			int i = 0;
			while (i < this._rawDataUnfiltered.Length)
			{
				if (this._rawDataUnfiltered[i].StartsWith(key + ": "))
				{
					if (value == null)
					{
						list = this._rawDataUnfiltered.ToList<string>();
						list.RemoveAt(i);
						num = 2;
						break;
					}
					this._rawDataUnfiltered[i] = key + ": " + value;
					num = 1;
					break;
				}
				else
				{
					i++;
				}
			}
			if (this.IsVirtual)
			{
				return;
			}
			if (num != 0)
			{
				if (num != 1)
				{
					if (num == 2 && list != null)
					{
						FileManager.WriteToFile(list, this.Path, false);
					}
				}
				else
				{
					FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
				}
			}
			else
			{
				list = this._rawDataUnfiltered.ToList<string>();
				list.Insert(list.Count, key + ": " + value);
				FileManager.WriteToFile(list, this.Path, false);
			}
			this.Reload();
		}


		private static List<string> GetStringList(string key, string path)
		{
			bool flag = false;
			List<string> list = new List<string>();
			foreach (string text in FileManager.ReadAllLines(path))
			{
				if (text.StartsWith(key) && text.EndsWith("[]"))
				{
					break;
				}
				if (text.StartsWith(key + ":"))
				{
					string text2 = text.Substring(key.Length + 1);
					if (text2.Contains("[") && text2.Contains("]"))
					{
						return YamlConfig.ParseCommaSeparatedString(text2).ToList<string>();
					}
					flag = true;
				}
				else if (flag)
				{
					if (text.StartsWith(" - "))
					{
						list.Add(text.Substring(3));
					}
					else if (!text.StartsWith("#"))
					{
						break;
					}
				}
			}
			return list;
		}


		public void SetStringListItem(string key, string value, string newValue)
		{
			this.Reload();
			bool flag = false;
			int num = 0;
			List<string> list = null;
			for (int i = 0; i < this._rawDataUnfiltered.Length; i++)
			{
				string text = this._rawDataUnfiltered[i];
				if (text.StartsWith(key + ":"))
				{
					flag = true;
				}
				else if (flag)
				{
					if (value != null && text == " - " + value)
					{
						if (newValue == null)
						{
							list = this._rawDataUnfiltered.ToList<string>();
							list.RemoveAt(i);
							num = 2;
							break;
						}
						this._rawDataUnfiltered[i] = " - " + newValue;
						num = 1;
						break;
					}
					else if (!text.StartsWith(" - ") && !text.StartsWith("#"))
					{
						if (value != null)
						{
							list = this._rawDataUnfiltered.ToList<string>();
							list.Insert(i, " - " + newValue);
							num = 2;
							break;
						}
						break;
					}
				}
			}
			if (this.IsVirtual)
			{
				return;
			}
			if (num == 1)
			{
				FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
			}
			else if (num == 2 && list != null)
			{
				FileManager.WriteToFile(list, this.Path, false);
			}
			this.Reload();
		}

	
		public IEnumerable<string> StringListToText(string key, List<string> list)
		{
			yield return key + ":";
			foreach (string str in list)
			{
				yield return " - " + str;
			}
	
			yield break;
		
		}


		public Dictionary<string, string> GetStringDictionary(string key)
		{
			List<string> stringList = this.GetStringList(key);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in stringList)
			{
				int num = text.IndexOf(": ", StringComparison.Ordinal);
				string text2 = text.Substring(0, num);
				if (!dictionary.ContainsKey(text2))
				{
					dictionary.Add(text2, text.Substring(num + 2));
				}
				else
				{
					ServerConsole.AddLog(string.Concat(new string[]
					{
					"Ignoring duplicated subkey ",
					text2,
					" in dictionary ",
					key,
					" in the config file."
					}));
				}
			}
			return dictionary;
		}


		public void SetStringDictionaryItem(string key, string subkey, string value)
		{
			this.Reload();
			bool flag = false;
			int num = 0;
			List<string> list = null;
			for (int i = 0; i < this._rawDataUnfiltered.Length; i++)
			{
				string text = this._rawDataUnfiltered[i];
				if (text.StartsWith(key + ":"))
				{
					flag = true;
				}
				else if (flag)
				{
					if (text.StartsWith(" - " + subkey + ": "))
					{
						if (value == null)
						{
							list = this._rawDataUnfiltered.ToList<string>();
							list.RemoveAt(i);
							num = 2;
							break;
						}
						this._rawDataUnfiltered[i] = " - " + subkey + ": " + value;
						num = 1;
						break;
					}
					else if (!text.StartsWith(" - ") && !text.StartsWith("#"))
					{
						if (value != null)
						{
							list = this._rawDataUnfiltered.ToList<string>();
							list.Insert(i, " - " + subkey + ": " + value);
							num = 2;
							break;
						}
						break;
					}
				}
			}
			if (this.IsVirtual)
			{
				return;
			}
			if (num != 0)
			{
				if (num != 1)
				{
					if (num == 2 && list != null)
					{
						FileManager.WriteToFile(list, this.Path, false);
					}
				}
				else
				{
					FileManager.WriteToFile(this._rawDataUnfiltered, this.Path, false);
				}
			}
			else
			{
				list = this._rawDataUnfiltered.ToList<string>();
				list.Insert(list.Count, " - " + subkey + ": " + value);
				FileManager.WriteToFile(list, this.Path, false);
			}
			this.Reload();
		}

	
		public static string[] ParseCommaSeparatedString(string data)
		{
			data = data.Trim();
			if (!data.StartsWith("[") || !data.EndsWith("]"))
			{
				return null;
			}
			data = data.Substring(1, data.Length - 2).Replace(", ", ",");
			return data.Split(new char[]
			{
			','
			});
		}

	
		public IEnumerable<string> GetKeys()
		{
			return from line in this.RawData
				   where line.Contains(":")
				   select line.Split(new char[]
				   {
			':'
				   })[0];
		}

		
		public bool IsList(string key)
		{
			bool flag = false;
			foreach (string text in this.RawData)
			{
				if (text.StartsWith(key + ":"))
				{
					flag = true;
				}
				else if (flag)
				{
					if (text.StartsWith(" - "))
					{
						return true;
					}
					if (!text.StartsWith("#"))
					{
						break;
					}
				}
			}
			return false;
		}

		
		public void Merge(ref PluginConfig toMerge)
		{
			string[] array = this.GetKeys().ToArray<string>();
			this.IsVirtual = true;
			foreach (string text in toMerge.GetKeys())
			{
				if (!array.Contains(text))
				{
					if (toMerge.IsList(text))
					{
						using (IEnumerator<string> enumerator2 = toMerge.StringListToText(text, toMerge.GetStringList(text)).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string element = enumerator2.Current;
								this.RawData.Append(element);
							}
							continue;
						}
					}
					this.SetString(text, toMerge.GetRawString(text));
				}
			}
		}

	
		public bool GetBool(string key, bool def = false)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			bool result;
			if (bool.TryParse(text, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid bool!");
			this.CommentInvalid(key, "BOOL");
			return def;
		}


		public byte GetByte(string key, byte def = 0)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			byte result;
			if (byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid byte!");
			this.CommentInvalid(key, "BYTE");
			return def;
		}


		public sbyte GetSByte(string key, sbyte def = 0)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			sbyte result;
			if (sbyte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid signed byte!");
			this.CommentInvalid(key, "SBYTE");
			return def;
		}

		
		public char GetChar(string key, char def = ' ')
		{
			string rawString = this.GetRawString(key);
			if (rawString == "default")
			{
				return def;
			}
			char result;
			if (char.TryParse(rawString, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + rawString + " is not a valid char!");
			this.CommentInvalid(key, "CHAR");
			return def;
		}


		public decimal GetDecimal(string key, decimal def = 0m)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			decimal result;
			if (decimal.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid decimal!");
			this.CommentInvalid(key, "DECIMAL");
			return def;
		}

	
		public double GetDouble(string key, double def = 0.0)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			double result;
			if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid double!");
			this.CommentInvalid(key, "DOUBLE");
			return def;
		}


		public float GetFloat(string key, float def = 0f)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			float result;
			if (float.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has invalid value, " + text + " is not a valid float!");
			this.CommentInvalid(key, "FLOAT");
			return def;
		}

		public int GetInt(string key, int def = 0)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			int result;
			if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid integer!");
			this.CommentInvalid(key, "INT");
			return def;
		}


		public uint GetUInt(string key, uint def = 0u)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			uint result;
			if (uint.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned integer!");
			this.CommentInvalid(key, "UINT");
			return def;
		}


		public long GetLong(string key, long def = 0L)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			long result;
			if (long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid long!");
			this.CommentInvalid(key, "LONG");
			return def;
		}

		public ulong GetULong(string key, ulong def = 0UL)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			ulong result;
			if (ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned long!");
			this.CommentInvalid(key, "ULONG");
			return def;
		}


		public short GetShort(string key, short def = 0)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			short result;
			if (short.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid short!");
			this.CommentInvalid(key, "SHORT");
			return def;
		}

		public ushort GetUShort(string key, ushort def = 0)
		{
			string text = this.GetRawString(key).ToLower();
			if (text == "default")
			{
				return def;
			}
			ushort result;
			if (ushort.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			ServerConsole.AddLog(key + " has an invalid value, " + text + " is not a valid unsigned short!");
			this.CommentInvalid(key, "USHORT");
			return def;
		}

	
		public string GetString(string key, string def = "")
		{
			string rawString = this.GetRawString(key);
			if (!(rawString == "default"))
			{
				return rawString;
			}
			return def;
		}


		public List<bool> GetBoolList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, bool>(bool.Parse)).ToList<bool>();
		}

		public List<byte> GetByteList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, byte>(byte.Parse)).ToList<byte>();
		}


		public List<sbyte> GetSByteList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, sbyte>(sbyte.Parse)).ToList<sbyte>();
		}


		public List<char> GetCharList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, char>(char.Parse)).ToList<char>();
		}

	
		public List<decimal> GetDecimalList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, decimal>(decimal.Parse)).ToList<decimal>();
		}

		public List<double> GetDoubleList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, double>(double.Parse)).ToList<double>();
		}

	
		public List<float> GetFloatList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, float>(float.Parse)).ToList<float>();
		}


		public List<int> GetIntList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, int>(int.Parse)).ToList<int>();
		}

	
		public List<uint> GetUIntList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, uint>(uint.Parse)).ToList<uint>();
		}


		public List<long> GetLongList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, long>(long.Parse)).ToList<long>();
		}


		public List<ulong> GetULongList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, ulong>(ulong.Parse)).ToList<ulong>();
		}


		public List<short> GetShortList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, short>(short.Parse)).ToList<short>();
		}


		public List<ushort> GetUShortList(string key)
		{
			return this.GetStringList(key).Select(new Func<string, ushort>(ushort.Parse)).ToList<ushort>();
		}


		public List<string> GetStringList(string key)
		{
			bool flag = false;
			List<string> list = new List<string>();
			foreach (string text in this.RawData)
			{
				if (text.StartsWith(key) && text.TrimEnd(Array.Empty<char>()).EndsWith("[]"))
				{
					break;
				}
				if (text.StartsWith(key + ":"))
				{
					string text2 = text.Substring(key.Length + 1);
					if (text2.Contains("[") && text2.Contains("]"))
					{
						return YamlConfig.ParseCommaSeparatedString(text2).ToList<string>();
					}
					flag = true;
				}
				else if (flag)
				{
					if (text.StartsWith(" - "))
					{
						list.Add(text.Substring(3).TrimEnd(Array.Empty<char>()));
					}
					else if (!text.StartsWith("#"))
					{
						break;
					}
				}
			}
			return list;
		}


		private static readonly string[] _deprecatedconfigs = new string[]
	{
		"TESTING"
	};
	

		
		private bool _virtual;

	
		private string[] _rawDataUnfiltered;

		private string[] _rawData;

	
		public string Path;
	}

}

