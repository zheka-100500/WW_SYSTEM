using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public enum PLAYER_PERMISSION
    {
        NONE = -1,
        KickingAndShortTermBanning = 1,
        BanningUpToDay = 2,
        LongTermBanning = 4,
        ForceclassSelf = 8,
        ForceclassToSpectator = 16,
        ForceclassWithoutRestrictions = 32,
        GivingItems = 64,
        WarheadEvents = 128,
        RespawnEvents = 256,
        RoundEvents = 512,
        SetGroup = 1024,
        GameplayData = 2048,
        Overwatch = 4096,
        FacilityManagement = 8192,
        PlayersManagement = 16384,
        PermissionsManagement = 32768,
        ServerConsoleCommands = 65536,
        ViewHiddenBadges = 131072,
        ServerConfigs = 262144,
        Broadcasting = 524288,
        PlayerSensitiveDataAccess = 1048576,
        Noclip = 2097152,
        AFKImmunity = 4194304,
        AdminChat = 8388608,
        ViewHiddenGlobalBadges = 16777216,
        Announcer = 33554432,
        Effects = 67108864,
        FriendlyFireDetectorImmunity = 134217728,
        FriendlyFireDetectorTempDisable = 268435456
    }
}
