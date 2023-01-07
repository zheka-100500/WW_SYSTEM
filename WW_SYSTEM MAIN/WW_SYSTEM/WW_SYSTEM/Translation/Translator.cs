using PlayerRoles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Translation
{
    public static class Translator
    {
        private static Dictionary<string, TranslateData> _translations = new Dictionary<string, TranslateData>();

        private static string _filesDirectory = Path.Combine(FileManager.GetAppFolder(true, true), "Translations");

        public static TranslateData MainTranslation;

        public static string ConfigVersion => "1.3";

        private static string GetTranslationFile(string PluginName, bool IgnoreSave = false)
        {
                
                if (!Directory.Exists(_filesDirectory))
                {
                Directory.CreateDirectory(_filesDirectory);
                }
            string file = Path.Combine(_filesDirectory, $"{PluginName.ToUpper()}.txt");
            if (!File.Exists(file) && !IgnoreSave)
            {
                TranslateData data = new TranslateData(new List<Translate>());
                SaveTranslationToFile(data, file);
            }
            return file;
            
        }

        public static void ReloadAllTranslations()
        {
            Logger.Info("Translator", $"RELOADING TRANSLATIONS...");

            _translations.Clear();

            if (!Directory.Exists(_filesDirectory)) Directory.CreateDirectory(_filesDirectory);
            foreach (var item in Directory.GetFiles(_filesDirectory))
            {
                try
                {
                    var datas = File.ReadAllLines(item);
                    var translates = new List<Translate>();
                    foreach (var translate in datas)
                    {
                        var index = translate.IndexOf(":");
                        if(index != -1)
                        {
                            var key = translate.Substring(0, index);
                            var Value = translate.Substring(index + 2);
                            if(!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(Value))
                            translates.Add(new Translate(key, Value));
                        }
                   
                    }
                    if (translates.Count > 0)
                    {
                        var data = new TranslateData(translates);
                        var PluginName = Path.GetFileNameWithoutExtension(item);
                        if (!_translations.ContainsKey(PluginName.ToUpper()))
                        {
                            _translations.Add(PluginName.ToUpper(), data);
                            Logger.Info("Translator", $"LOADED: {PluginName}");
                        }
                    }


                }
                catch (Exception ex)
                {

                    Logger.Error("Translator", $"FAILED TO LOAD {item}: {ex}");
                }
            }

            if (!_translations.ContainsKey("WW_SYSTEM"))
            {
                LoadDefaults();
            }
            MainTranslation = GetTranslationForPlugin("WW_SYSTEM");
            Logger.Info("Translator", $"RELOAD COMPLETED!");
        }


        public static void LoadDefaults()
        {
            Logger.Info("Translator", "LOADING DEFAULTS..");
           

            var file = GetTranslationFile("WW_SYSTEM", true);
            var data = new TranslateData(GetDefaultTranslates());
            SaveTranslationToFile(data, file);
            _translations.Add("WW_SYSTEM", data);
            Logger.Info("Translator", data.Translates.Count.ToString());
            Logger.Info("Translator", "DEFAULTS LOADED!");
        }

        public static List<Translate> GetDefaultTranslates()
        {
            List<Translate> translates = new List<Translate>();


            foreach (var item in Enum.GetNames(typeof(RoleTypeId)))
            {
                translates.Add(new Translate($"ROLE_{item}", item));
            }
            foreach (var item in Enum.GetNames(typeof(TEAMTYPE)))
            {
                translates.Add(new Translate($"TEAM_{item}", item));
            }
            foreach (var item in Enum.GetNames(typeof(PLAYER_PERMISSION)))
            {
                translates.Add(new Translate($"PERM_{item}", item));
            }

            foreach (var item in Enum.GetNames(typeof(ExperienceType)))
            {
                translates.Add(new Translate($"XP_079_{item}", item));
            }
            foreach (var item in Enum.GetNames(typeof(DamageType)))
            {
                translates.Add(new Translate($"DAMAGE_{item}", item));
            }
            translates.Add(new Translate("CMD_NO_PERM", "REQUIRED PERMISSION: %perm%"));
            translates.Add(new Translate("CMD_NO_ROLE", "REQUIRED ROLE: %role%"));
            translates.Add(new Translate("CMD_NO_TEAM", "REQUIRED TEAM: %team%"));
            translates.Add(new Translate("EV_PICKUP_DENIED", "<color=red>PICKUP ITEM BLOCKED BY PLUGIN!</color>"));
            translates.Add(new Translate("CFG_VERSION", ConfigVersion));
            return translates;
        }

        public static void SaveTranslationToFile(TranslateData data, string file)
        {
            List<string> saveData = new List<string>();
            foreach (var item in data.Translates)
            {
                saveData.Add($"{item.TranslateKey.ToUpper()}: {item.TranslateValue}");
            }
            File.WriteAllLines(file, saveData);
        }

        public static void UpdateData(ref TranslateData data)
        {
            var defaults = GetDefaultTranslates();

            bool DetectedNoTranslation = false;
            foreach (var item in defaults)
            {
                if (!data.HasTranslation(item.TranslateKey))
                {
                    if (!DetectedNoTranslation)
                    {
                        DetectedNoTranslation = true;
                        if (data.HasTranslation("CFG_VERSION"))
                        {
                            data.Translates.Remove(data.GetTranslationData("CFG_VERSION"));
                        }
                        data.Translates.Add(new Translate("CFG_VERSION", ConfigVersion));
                    }
                    data.Translates.Add(item);

                }
            }
        }

        public static TranslateData LoadTranslate(string PluginName)
        {
            string file = GetTranslationFile(PluginName);
            try
            {
                var datas = File.ReadAllLines(file);
                var translates = new List<Translate>();
                foreach (var translate in datas)
                {
                    var index = translate.IndexOf(":");
                    if (index != -1)
                    {
                        var key = translate.Substring(0, index);
                        var Value = translate.Substring(index + 2);
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(Value))
                            translates.Add(new Translate(key.ToUpper(), Value));
                    }

                }
            
                var data = new TranslateData(translates);
                if(!_translations.ContainsKey(PluginName.ToUpper()))
                _translations.Add(PluginName.ToUpper(), data);

                if(data.GetTranslation("CFG_VERSION") != ConfigVersion)
                {
                    UpdateData(ref data);
                    SaveTranslationToFile(data, file);
                }

                return data;
            }
            catch (Exception ex)
            {
                Logger.Error("Translator", $"FAILED TO LOAD {file}: {ex}");
                return null;
            }
        }

        public static TranslateData GetTranslationForPlugin(string PluginName)
        {

            if (TryGetTranslate(PluginName, out var data))
            {
                return data;
            }
            else
            {
                return LoadTranslate(PluginName);
            }
        }


        public static bool TryGetTranslate(string PluginName, out TranslateData data)
        {
            foreach (var item in _translations)
            {
                if(item.Key.ToUpper() == PluginName.ToUpper())
                {
                    data = item.Value;
                    return true;
                }
            }

            data = null;
            return false;
        }

    }
}
