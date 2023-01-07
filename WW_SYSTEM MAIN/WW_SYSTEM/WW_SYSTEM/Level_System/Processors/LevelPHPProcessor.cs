using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using System.Net;
using System.Collections.Specialized;
using UnityEngine;

namespace WW_SYSTEM.Level_System.Processors
{
    public class LevelPHPProcessor : LevelProcessor
    {
        public string SecretKey { get; private set; }
        public string UrlToPHPFiles { get; private set; }
        public string Database { get; private set; }

        public LevelPHPProcessor(string UrlToPHPFiles, string SecretKey, string Database)
        {
            this.UrlToPHPFiles = UrlToPHPFiles;
            this.SecretKey = SecretKey;
            this.Database = Database;
        }


        public bool CheckForPlayer(string UserId)
        {
            if (string.IsNullOrEmpty(UrlToPHPFiles)) return false;
            var data = new NameValueCollection();
            data["UserID"] = UserId;
            if (SendPost("CheckForPlayer", data) == "OK")
            {
                return true;
            }

            return false;
        }

        public bool SavePlayer(string UserId, PlayerLevel level)
        {
            if (string.IsNullOrEmpty(UrlToPHPFiles)) return false;
            var data = new NameValueCollection();
            data["UserID"] = UserId;
            data["CurExp"] = level.CurExp.ToString();
            data["MaxExp"] = level.MaxExp.ToString();
            data["CurLvl"] = level.CurLvl.ToString();
            if (SendPost("SavePlayer", data) == "OK")
            {
                return true;
            }
            return false;
        }

        public bool TryLoadPlayer(string UserId, out PlayerLevel level)
        {
            level = null;
            if (string.IsNullOrEmpty(UrlToPHPFiles)) return false;

            var data = new NameValueCollection();
            data["UserID"] = UserId;
            var result = SendPost("LoadPlayer", data);
            if (result != "ERROR")
            {
                try
                {
               
                    level = JsonUtility.FromJson<PlayerLevel>(result);
                    level.OriginalPrefix = "";
                    level.OriginalColor = "";
                    return true;
                }
                catch (Exception ex)
                {
                    this.Error($"FAILED TO LOAD DATA: {ex}");
                    return false;
                }
            
            }

            return false;
        }

        public void ConvertToNewProcessor(LevelProcessor processor)
        {
            this.Error("NOT IMPLEMENTED!");
        }

        private string SendPost(string file, NameValueCollection values)
        {
            using (var wb = new WebClient())
            {
                values["SecretKey"] = SecretKey;
                values["Database"] = Database;
                var response = wb.UploadValues($"{UrlToPHPFiles}{file}.php", "POST", values);
                return Encoding.UTF8.GetString(response);
            }
        }
    }
}
