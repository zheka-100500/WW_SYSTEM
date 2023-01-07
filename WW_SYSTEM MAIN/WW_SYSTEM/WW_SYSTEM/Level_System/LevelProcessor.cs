using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Level_System
{
    public interface LevelProcessor
    {
        bool TryLoadPlayer(string UserId, out PlayerLevel level);

        bool CheckForPlayer(string UserId);

        bool SavePlayer(string UserId, PlayerLevel level);

        void ConvertToNewProcessor(LevelProcessor processor);
    }
}
