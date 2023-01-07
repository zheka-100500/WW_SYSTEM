using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using WW_SYSTEM.API;

namespace WW_SYSTEM.VanillaFixes
{
    public static class VanillaCommands
    {
        private static List<VanillaCommand> _VanillaCommands = new List<VanillaCommand>();

        public static bool IsVanillaCmdPermitted(this Player player, string VanillaCmd)
        {
            if(TryGetVanillaPermissions(VanillaCmd, out var perms))
            {
                return player.IsPermitted(perms);
            }
            return true;
        }

        public static bool IsVanillaCmdPermitted(this Player player, string VanillaCmd, out List<PLAYER_PERMISSION> Perms)
        {
            if (TryGetVanillaPermissions(VanillaCmd, out var perms))
            {
                Perms = perms;
                return player.IsPermitted(perms);
            }
            Perms = new List<PLAYER_PERMISSION>();
            return true;
        }

        public static bool IsVanillaCmdPermitted(this Player player, string[] VanillaCmds, out List<PLAYER_PERMISSION> Perms)
        {
            var cmd = VanillaCmds[0].ToUpper();
            if (TryGetVanillaPermissions(cmd, out var perms))
            {
                Perms = perms;
                var result = player.IsPermitted(perms);
                if (!result && VanillaCmds.Length > 2 && cmd.ToLower() == "forceclass" || cmd.ToLower() == "fc")
                {

                    var cmds = VanillaCmds.Segment(1);
                    string[] newargs;
                    RAUtils.ProcessPlayerIdOrNamesList(cmds, 0, out newargs, false);

                    if(int.TryParse(newargs[0], out var roleId) && roleId == (int)RoleTypeId.Spectator)
                    {
                        result = player.IsPermitted(PLAYER_PERMISSION.ForceclassToSpectator);
                        Perms = new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.ForceclassToSpectator };
                    }


                }

                return result;
            }
            Perms = new List<PLAYER_PERMISSION>();
            return true;
        }


       

        public static bool TryGetVanillaPermissions(string Cmd, out List<PLAYER_PERMISSION> Perms)
        {
            foreach (var cmd in _VanillaCommands)
            {
                if (cmd.CheckForCmdName(Cmd))
                {
                    Perms = cmd.Permissions;
                    return true;
                }
            }
            Perms = null;
            return false;
        }

        public static void LoadCmds()
        {
            _VanillaCommands.Clear();

            RegisterCmd("clearcassie", PLAYER_PERMISSION.Announcer, "cassieclear");
            RegisterCmd("ban", PLAYER_PERMISSION.KickingAndShortTermBanning);
            RegisterCmd("bring", PLAYER_PERMISSION.PlayersManagement);
            RegisterCmd("pocketdimension", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "pocketdim", "shadowrealm", "pd" });
            RegisterCmd("roundrestart", PLAYER_PERMISSION.RoundEvents, new List<string>() { "rr", "restart" });
            RegisterCmd("broadcast", PLAYER_PERMISSION.Broadcasting, new List<string>() { "bc", "alert" });
            RegisterCmd("broadcastmono", PLAYER_PERMISSION.Broadcasting, new List<string>() { "bcmono", "alertmono" });
            RegisterCmd("changecolor", PLAYER_PERMISSION.FacilityManagement, new List<string>() { "changec", "ccolor" });
            RegisterCmd("setname", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "setnickname", "setnick" });
            RegisterCmd("name", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "nickname", "nick" });
            RegisterCmd("close", PLAYER_PERMISSION.FacilityManagement, new List<string>() { "closedoor", "c" });
            RegisterCmd("doorslist", PLAYER_PERMISSION.FacilityManagement, new List<string>() { "doors", "dl" });
            RegisterCmd("doortp", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "dtp", "doorteleport" });
            RegisterCmd("lock", PLAYER_PERMISSION.FacilityManagement, new List<string>() { "lockdoor", "l" });
            RegisterCmd("open", PLAYER_PERMISSION.FacilityManagement, new List<string>() { "opendoor", "o" });
            RegisterCmd("unlock", PLAYER_PERMISSION.FacilityManagement, new List<string>() { "unlockdoor", "ul" });
            RegisterCmd("lobbylock", PLAYER_PERMISSION.RoundEvents, new List<string>() { "ll", "llock" });
            RegisterCmd("noclip", PLAYER_PERMISSION.Noclip, new List<string>() { "n", "nc" });
            RegisterCmd("remoteconsole", PLAYER_PERMISSION.ServerConsoleCommands, new List<string>() { "rcon", "sudo" });
            RegisterCmd("roomtp", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "rtp", "ridtp" });
            RegisterCmd("roundlock", PLAYER_PERMISSION.RoundEvents, new List<string>() { "rl", "rlock" });
            RegisterCmd("setgroup", PLAYER_PERMISSION.SetGroup, new List<string>() { "sg", "setrole" });
            RegisterCmd("hp", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "sethealth", "sethp" });
            RegisterCmd("setlevel", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "level", "lvl" });
            RegisterCmd("timedeffect", PLAYER_PERMISSION.Effects, new List<string>() { "teffect", "tpfx" });
            RegisterCmd("softrestart", PLAYER_PERMISSION.ServerConsoleCommands, new List<string>() { "srestart", "sr" });
            RegisterCmd("stopnextround", PLAYER_PERMISSION.ServerConsoleCommands, "snr");
            RegisterCmd("forcestart", PLAYER_PERMISSION.RoundEvents, new List<string>() { "fs", "rs", "start", "roundstart" });
            RegisterCmd("playerinventory", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "playerinv", "pinventory", "pinv" });
            RegisterCmd("friendlyfiredetector", PLAYER_PERMISSION.PlayersManagement, new List<string>() { "tk", "tkd", "teamkilldetector", "ffd" });
            RegisterCmd("bypass", PLAYER_PERMISSION.FacilityManagement, "bm");
            RegisterCmd("lockdown", PLAYER_PERMISSION.FacilityManagement, "ld");
            RegisterCmd("decontamination", PLAYER_PERMISSION.RoundEvents, "decont");
            RegisterCmd("disarm", PLAYER_PERMISSION.PlayersManagement, "da");
            RegisterCmd("destroy", PLAYER_PERMISSION.FacilityManagement, "destroydoor");
            RegisterCmd("effect", PLAYER_PERMISSION.Effects, "pfx");
            RegisterCmd("pddebug", PLAYER_PERMISSION.RoundEvents, "106debug");
            RegisterCmd("setconfig", PLAYER_PERMISSION.ServerConfigs, "sc");
            RegisterCmd("intercomtext", PLAYER_PERMISSION.Broadcasting, "icomtxt");
            RegisterCmd("release", PLAYER_PERMISSION.PlayersManagement, "free");
            RegisterCmd("reloadconfig", PLAYER_PERMISSION.ServerConfigs, "rc");
            RegisterCmd("unban", PLAYER_PERMISSION.LongTermBanning, "pardon");
            RegisterCmd("forceattachments", PLAYER_PERMISSION.PlayersManagement, "forceatt");
            RegisterCmd("intercom-reset", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.RoundEvents, PLAYER_PERMISSION.FacilityManagement, PLAYER_PERMISSION.PlayersManagement }, new List<string>() { "icomreset", "ir" });
            RegisterCmd("intercom-timeout", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.KickingAndShortTermBanning, PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning, PLAYER_PERMISSION.RoundEvents, PLAYER_PERMISSION.FacilityManagement, PLAYER_PERMISSION.PlayersManagement }, new List<string>() { "icomstop", "icomtimeout", "it" });
            RegisterCmd("forceclass", PLAYER_PERMISSION.ForceclassSelf, "fc");
            RegisterCmd("icom", PLAYER_PERMISSION.Broadcasting, "speak");
            RegisterCmd("overcharge", PLAYER_PERMISSION.RoundEvents, "ocharge");
            RegisterCmd("overwatch", PLAYER_PERMISSION.Overwatch, "ovr");
            RegisterCmd("permissionsmanagement", PLAYER_PERMISSION.PermissionsManagement, "pm");
            RegisterCmd("playerbroadcast", PLAYER_PERMISSION.Broadcasting, "pbc");
            RegisterCmd("cassie", PLAYER_PERMISSION.Announcer);
            RegisterCmd("give", PLAYER_PERMISSION.GivingItems);
            RegisterCmd("god", PLAYER_PERMISSION.PlayersManagement);
            RegisterCmd("goto", PLAYER_PERMISSION.PlayersManagement);
            RegisterCmd("heal", PLAYER_PERMISSION.PlayersManagement);
            RegisterCmd("kill", PLAYER_PERMISSION.PlayersManagement);
            RegisterCmd("ping", PLAYER_PERMISSION.PlayersManagement);
            RegisterCmd("refreshcommands", PLAYER_PERMISSION.ServerConsoleCommands);
            RegisterCmd("strip", PLAYER_PERMISSION.PlayersManagement, "clear");
            RegisterCmd("tickets", PLAYER_PERMISSION.RespawnEvents, "tix");
            RegisterCmd("warhead", PLAYER_PERMISSION.WarheadEvents, "wh");
            RegisterCmd("config", PLAYER_PERMISSION.ServerConfigs, "cfg");
            RegisterCmd("restartnextround", PLAYER_PERMISSION.ServerConsoleCommands, "rnr");
            RegisterCmd("spawntarget", PLAYER_PERMISSION.FacilityManagement);
            RegisterCmd("stare", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.ForceclassSelf, PLAYER_PERMISSION.ForceclassWithoutRestrictions } );
            RegisterCmd("enrage", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.ForceclassSelf, PLAYER_PERMISSION.ForceclassWithoutRestrictions } );
            RegisterCmd("SERVER_EVENT");
            RegisterCmd("imute", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning, PLAYER_PERMISSION.PlayersManagement });
            RegisterCmd("iunmute", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning, PLAYER_PERMISSION.PlayersManagement });
            RegisterCmd("mute", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning, PLAYER_PERMISSION.PlayersManagement });
            RegisterCmd("unmute", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning, PLAYER_PERMISSION.PlayersManagement });
            RegisterCmd("offlineban", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.KickingAndShortTermBanning, PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning }, "oban");
            RegisterCmd("externallookup", new List<PLAYER_PERMISSION>() { PLAYER_PERMISSION.BanningUpToDay, PLAYER_PERMISSION.LongTermBanning, PLAYER_PERMISSION.SetGroup, PLAYER_PERMISSION.PlayersManagement, PLAYER_PERMISSION.PermissionsManagement, PLAYER_PERMISSION.ViewHiddenBadges, PLAYER_PERMISSION.PlayerSensitiveDataAccess, PLAYER_PERMISSION.ViewHiddenGlobalBadges });
            RegisterCmd("cassie_silent", PLAYER_PERMISSION.Announcer, new List<string>() { "cassie_silentnoise", "cassie_sn", "cassie_sl" });
            RegisterCmd("clearbroadcasts", PLAYER_PERMISSION.Broadcasting, new List<string>() { "cl", "clearbc", "bcclear", "clearalert", "alertclear" });
            RegisterCmd("cassiewords");
            RegisterCmd("contact");
            RegisterCmd("perm");
            RegisterCmd("showtag");
            RegisterCmd("version");
            RegisterCmd("wiki");
            RegisterCmd("buildinfo");
            RegisterCmd("hello");
            RegisterCmd("help");
            RegisterCmd("ridlist", "rids");
            RegisterCmd("uptime", "rounds");
            RegisterCmd("roundtime", new List<string>() { "rtime", "rt" });


        }

        private static void RegisterCmd(string cmd, string Alias)
        {
            RegisterCmd(cmd, new List<PLAYER_PERMISSION>(), new List<string> { Alias});
        }

        private static void RegisterCmd(string cmd, List<string> Aliases)
        {
            RegisterCmd(cmd, new List<PLAYER_PERMISSION>(), Aliases);
        }

        private static void RegisterCmd(string cmd)
        {
            RegisterCmd(cmd, new List<PLAYER_PERMISSION>(), new List<string>());
        }

        private static void RegisterCmd(string cmd, PLAYER_PERMISSION perm)
        {
            RegisterCmd(cmd, new List<PLAYER_PERMISSION>() { perm }, new List<string>());
        }

        private static void RegisterCmd(string cmd, PLAYER_PERMISSION perm, List<string> Aliases)
        {
            RegisterCmd(cmd, new List<PLAYER_PERMISSION>() { perm }, Aliases);
        }

        private static void RegisterCmd(string cmd, PLAYER_PERMISSION perm, string Alias)
        {
            RegisterCmd(cmd, new List<PLAYER_PERMISSION>() { perm}, new List<string>() { Alias });
        }


        private static void RegisterCmd(string cmd, List<PLAYER_PERMISSION> perms, List<string> Aliases)
        {
            cmd = cmd.ToUpper();
            foreach (var item in _VanillaCommands)
            {
                if (item.CheckForCmdName(cmd)) return;
            }

            _VanillaCommands.Add(new VanillaCommand(cmd, perms, Aliases));
        }
        private static void RegisterCmd(string cmd, List<PLAYER_PERMISSION> perms)
        {
            RegisterCmd(cmd, perms, new List<string>());
        }
        private static void RegisterCmd(string cmd, List<PLAYER_PERMISSION> perms, string Alias)
        {
            RegisterCmd(cmd, perms, new List<string>() { Alias });
        }



    }
}
