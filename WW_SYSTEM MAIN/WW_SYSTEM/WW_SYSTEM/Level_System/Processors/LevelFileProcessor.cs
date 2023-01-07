using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Level_System.Processors
{
    public class LevelFileProcessor : LevelProcessor
    {
        public string dirSeperator = Path.DirectorySeparatorChar.ToString();
   
        public string XPPath
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomXpPath)) return CustomXpPath;

                return FileManager.GetAppFolder(true, true, "") + "LEVELSYSTEM";
            }
        }
        public string CustomXpPath = string.Empty;

        public LevelFileProcessor()
        {
            CustomXpPath = string.Empty;
        }
        public LevelFileProcessor(string XPPath)
        {
            CustomXpPath = XPPath;
        }

        public string GetLvlFile(string UserId) => XPPath + dirSeperator + UserId + ".lvl";

        public bool CheckForPlayer(string UserId)
        {
            if (!Directory.Exists(XPPath))
            {
                Directory.CreateDirectory(XPPath);
            }
            return File.Exists(GetLvlFile(UserId));
        }

        public bool SavePlayer(string UserId, PlayerLevel level)
        {
            try
            {
                string json = JsonUtility.ToJson(level, true);
                File.WriteAllText(GetLvlFile(UserId), json);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
      
        }

        public bool TryLoadPlayer(string UserId, out PlayerLevel level)
        {
            level = null;
            if (!File.Exists(GetLvlFile(UserId))) return false;

            try
            {
                level = JsonUtility.FromJson<PlayerLevel>(File.ReadAllText(GetLvlFile(UserId)));
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

        public void ConvertToNewProcessor(LevelProcessor processor)
        {
            foreach (var file in Directory.GetFiles(XPPath))
            {
                if (file.Contains(".lvl"))
                {
                    try
                    {
                        PlayerLevel playerLevel = JsonUtility.FromJson<PlayerLevel>(File.ReadAllText(file));
                        string UserID = file.Replace(XPPath, "").Replace("/", "").Replace(".lvl", "");
                        processor.SavePlayer(UserID, playerLevel);
                        continue;

                    }
                    catch (Exception arg)
                    {

                        this.Error(string.Format("ERROR OF CONVERT DATA: {0} REASON: {1}", file, arg));
                        continue;
                    }
                }
            }
        }
    }
}
