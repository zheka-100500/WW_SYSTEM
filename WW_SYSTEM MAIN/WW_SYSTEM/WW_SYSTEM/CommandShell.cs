using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WW_SYSTEM.API;
using RemoteAdmin;
using WW_SYSTEM.Translation;
using PlayerRoles;
using Mono.Cecil;

namespace WW_SYSTEM
{
    public struct CommandInfo
    {
        public Action<Player,CommandArg[]> proc;
        public int max_arg_count;
        public int min_arg_count;
        public string description;
        public string[] usage;
        public string[] aliases;
        public CommandType type;
        public PLAYER_PERMISSION permission;
        public string Custom_Permission;
        public RoleTypeId role;
        public TEAMTYPE team;
        public bool HideInHelp;
    }

    public struct CommandArg
    {
        public string String { get; set; }

        public int Int
        {
            get
            {
                int int_value;

                if (int.TryParse(String, out int_value))
                {
                    return int_value;
                }

                throw new Exception(TypeError("int"));
               
            }
        }

        public float Float
        {
            get
            {
                float float_value;

                if (float.TryParse(String, out float_value))
                {
                    return float_value;
                }

                throw new Exception(TypeError("float"));
          
            }
        }

        public bool Bool
        {
            get
            {
                if (string.Compare(String, "TRUE", ignoreCase: true) == 0)
                {
                    return true;
                }

                if (string.Compare(String, "FALSE", ignoreCase: true) == 0)
                {
                    return false;
                }

                throw new Exception(TypeError("bool"));
            }
        }

        public override string ToString()
        {
            return String;
        }

   

        public string TypeError(string expected_type)
        {
            return string.Format("Incorrect type for {0}, expected <{1}>",
                  String, expected_type);
        }
    }


    public class CommandShell
    {
        Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();
        List<CommandArg> arguments = new List<CommandArg>(); // Cache for performance

       

        public Dictionary<string, CommandInfo> Commands
        {
            get { return commands; }
        }
        public QueryProcessor.CommandData[] GetCommandsForRa(QueryProcessor processor)
        {
            List<QueryProcessor.CommandData> Result = new List<QueryProcessor.CommandData>();
            if (Round.TryGetPlayer(processor._hub.characterClassManager.UserId, out var pl))
            {
                foreach (var item in commands)
                {
                    if (item.Value.type == CommandType.RemoteAdmin && !item.Value.HideInHelp)
                    {
                        if (CheckPlayer(item.Value, pl, out var deniedres, true))
                        {
                           
                            Result.Add(new QueryProcessor.CommandData
                            {
                                Command = item.Key,
                                Usage = item.Value.usage,
                                Description = item.Value.description,
                                AliasOf = null,
                                Hidden = false
                            });
                            if (item.Value.aliases != null && item.Value.aliases.Length > 0)
                            {
                                foreach (var alias in item.Value.aliases)
                                {
                                    Result.Add(new QueryProcessor.CommandData
                                    {
                                        Command = alias,
                                        Usage = item.Value.usage,
                                        Description = item.Value.description,
                                        AliasOf = item.Key,
                                        Hidden = false
                                    });
                                }
                            }

                        }
                    }
                }

            }
            return Result.ToArray();
        }


        /// <summary>
        /// Uses reflection to find all RegisterCommand attributes
        /// and adds them to the commands dictionary.
        /// </summary>
        public void RegisterCommands()
        {
            int commands = 0;
            int RejectedCommands = 0;
            var rejected_commands = new Dictionary<string, CommandInfo>();
            var method_flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;


            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {

                Logger.Debug("COMMAND SYSTEM", $"CHECKING DLL: {item.GetName().Name} FOR COMMANDS...");

                try
                {
                    foreach (var type in item.GetTypes())
                    {
                        foreach (var method in type.GetMethods(method_flags))
                        {
                          

                            var attribute = Attribute.GetCustomAttribute(
                                method, typeof(CommandAttribute)) as CommandAttribute;

                            if (attribute == null)
                            {
                                if (method.Name.StartsWith("FRONTCOMMAND", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // Front-end Command methods don't implement RegisterCommand, use default attribute
                                    attribute = new CommandAttribute();
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            var methods_params = method.GetParameters();

                            string command_name = InferFrontCommandName(method.Name);
                            Action<Player, CommandArg[]> proc;

                            if (attribute.Name == null)
                            {
                                // Use the method's name as the command's name
                                command_name = InferCommandName(command_name == null ? method.Name : command_name);
                            }
                            else
                            {
                                command_name = attribute.Name;
                            }

                            if (methods_params.Length != 2 || methods_params[0].ParameterType != typeof(Player) || methods_params[1].ParameterType != typeof(CommandArg[]))
                            {
                                // Method does not match expected Action signature,
                                // this could be a command that has a FrontCommand method to handle its arguments.
                                rejected_commands.Add(command_name.ToUpper(), CommandFromParamInfo(methods_params, attribute.Description));
                                RejectedCommands++;
                                continue;
                            }

                            // Convert MethodInfo to Action.
                            // This is essentially allows us to store a reference to the method,
                            // which makes calling the method significantly more performant than using MethodInfo.Invoke().
                            proc = (Action<Player, CommandArg[]>)Delegate.CreateDelegate(typeof(Action<Player, CommandArg[]>), method);
                            Logger.Info("WW SYSTEM", $"REGISTER COMMAND: {command_name.ToUpper()}!");
                            AddCommand(command_name.ToUpper(), proc, attribute.MinArgCount, attribute.MaxArgCount, attribute.Description, attribute.Usage, attribute.Aliases, attribute.type, attribute.Role, attribute.Team, attribute.Permissions, attribute.Custom_Permissions, attribute.HideInHelp);
                            commands++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("COMMAND SYSTEM", $"CHECKING DLL: {item.GetName().Name} FAILED: {ex}");
                }

               
            }
            Logger.Info("WW SYSTEM", $"DONE! REGISTERED: {commands} COMMANDS! REJECTED: {RejectedCommands} COMMANDS!");
            HandleRejectedCommands(rejected_commands);
        }

        /// <summary>
        /// Parses an input line into a command and runs that command.
        /// </summary>
        /// 

        private bool CheckPlayer(CommandInfo info, Player player, out string DeniedReason, bool OnlyPerm = false)
        {
            DeniedReason = "NO ERROR";
            if (player == null && info.type != CommandType.ServerConsole) { DeniedReason = "PLAYER IS NULL"; return false; }

            if (info.type == CommandType.ServerConsole) return true;

          

            if (info.permission != PLAYER_PERMISSION.NONE)
            {
                if (!player.IsPermitted(info.permission))
                {
                    DeniedReason =Translator.MainTranslation.GetTranslation("CMD_NO_PERM").Replace("%perm%", Translator.MainTranslation.GetTranslation(info.permission));
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(info.Custom_Permission))
            {
                if (!player.IsPermitted(info.Custom_Permission))
                {
                    DeniedReason = Translator.MainTranslation.GetTranslation("CMD_NO_PERM").Replace("%perm%", Translator.MainTranslation.GetTranslation($"PERM_" + info.Custom_Permission));
                    return false;
                }
            }

            if (OnlyPerm) return true;

            if (info.role != RoleTypeId.None)
            { 
                if (player.Role != info.role)
                {
                    DeniedReason = Translator.MainTranslation.GetTranslation("CMD_NO_ROLE").Replace("%role%", Translator.MainTranslation.GetTranslation(info.role));
                    return false;
                }
            }
            if (info.team != TEAMTYPE.NONE)
            {
                if (!player.CheckForTeam(info.team))
                {

                    DeniedReason = Translator.MainTranslation.GetTranslation("CMD_NO_TEAM").Replace("%team%", Translator.MainTranslation.GetTranslation(info.team));
                    return false;
                }
                
            }
            return true;

        }

        public string RunCommand(string line, Player pl, CommandType type, out string Result)
        {
            string remaining = line;
            arguments.Clear();

            while (remaining != "")
            {
                var argument = EatArgument(ref remaining);

                if (argument.String != "")
                {
                    arguments.Add(argument);
                }
            }

            if (arguments.Count == 0)
            {
                // Nothing to run
                Result = "ERROR";
                return "ERROR";
            }

            string command_name = arguments[0].String.ToUpper();
            arguments.RemoveAt(0); // Remove command name from arguments


            if (command_name == "HELP")
            {
                string result = "COMMANDS";
                foreach (var item in commands)
                {

                   
                    if (item.Value.type != type) continue;
                    if (item.Value.HideInHelp) continue;
                    string Description = string.IsNullOrEmpty(item.Value.description) ? "NO DESCRIPTION" : item.Value.description;
           
                    if(CheckPlayer(item.Value, pl, out var Denied))
                    {
                        result += $"\n{item.Key} - {Description}";
                    }
                   
                }
                switch (type)
                {
                    case CommandType.GameConsole:
                        pl.ConsoleMessage(result, "green");
                        break;
                    case CommandType.RemoteAdmin:
                        pl.RaMessage("HELP", result, true);
                        break;
                    case CommandType.ServerConsole:
                        Logger.Info("HELP", result);
                        break;
                    default:
                        Logger.Info("HELP", result);
                        break;
                }
                Result = "DONE!";
                return "DONE!";
            }
            CommandInfo command;
            foreach (var item in commands)
            {
                if(item.Key == command_name)
                {
                    command = item.Value;
                    goto process;
                }
                else if(IsAlias(item.Value, command_name))
                {
                    command_name = item.Key;
                    command = item.Value;
                    goto process;
                }
            }
            Result = "ERROR";
            return "ERROR";
        process:


            if (command.type != type)
            {
                Result = "ERROR";
                return "ERROR";
            }

            string DeniedRes;
            if (CheckPlayer(command, pl, out DeniedRes))
            {
                Result = RunCommand(command_name, arguments.ToArray(), pl);

                if(Result == "DONE!")
                {
                    return "DONE!";
                }
                else
                {
                    return "OUTPUT";
                }
              
            }
            else
            {
                Result = DeniedRes;
                return "DENIED";
            }

        }

        public string RunCommand(string command_name, CommandArg[] arguments, Player pl)
        {
            var command = commands[command_name];
            int arg_count = arguments.Length;
            string error_message = null;
            int required_arg = 0;

            if (arg_count < command.min_arg_count)
            {
                if (command.min_arg_count == command.max_arg_count)
                {
                    error_message = "exactly";
                }
                else
                {
                    error_message = "at least";
                }
                required_arg = command.min_arg_count;
            }
            else if (command.max_arg_count > -1 && arg_count > command.max_arg_count)
            {
                // Do not check max allowed number of arguments if it is -1
                if (command.min_arg_count == command.max_arg_count)
                {
                    error_message = "exactly";
                }
                else
                {
                    error_message = "at most";
                }
                required_arg = command.max_arg_count;
            }

            if (error_message != null)
            {
                string plural_fix = required_arg == 1 ? "" : "s";
                return IssueErrorMessage(
                    "{0} requires {1} {2} argument{3}",
                    command_name,
                    error_message,
                    required_arg,
                    plural_fix
                );
               
            }
          
           command.proc(pl, arguments);
            return "DONE!";
        }

         private bool IsAlias(CommandInfo info, string alias)
        {
            if (info.aliases == null || info.aliases.Length <= 0) return false;

            foreach (var item in info.aliases)
            {
                if(item.ToUpper() == alias.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        public void AddCommand(string name, CommandInfo info)
        {
            name = name.ToUpper();

            if (commands.ContainsKey(name))
            {
                IssueErrorMessage("Command {0} is already defined.", name);
                return;
            }

            commands.Add(name, info);
        }

        public void AddCommand(string name,
                               Action<Player, CommandArg[]> proc,
                               int min_arg_count = 0,
                               int max_arg_count = -1,
                               string description = "",
                               string[] usage = null,
                               string[] aliases = null,
                               CommandType type = CommandType.GameConsole,
                               RoleTypeId role = RoleTypeId.None,
                               TEAMTYPE team = TEAMTYPE.NONE,
                               PLAYER_PERMISSION permissions = PLAYER_PERMISSION.NONE,
                               string Custom_Permission = "",
                               bool HideInHelp = false)
                                  
        {
            var info = new CommandInfo()
            {
                proc = proc,
                min_arg_count = min_arg_count,
                max_arg_count = max_arg_count,
                description = description,
                usage = usage,
                aliases = aliases,
                type = type,
                role = role,
                permission = permissions,
                Custom_Permission = Custom_Permission,
                HideInHelp = HideInHelp
            };

            AddCommand(name, info);
        }

        public string IssueErrorMessage(string format, params object[] message)
        {
            return string.Format(format, message);
        }

        string InferCommandName(string method_name)
        {
            string command_name;
            int index = method_name.IndexOf("COMMAND", StringComparison.CurrentCultureIgnoreCase);

            if (index >= 0)
            {
                // Method is prefixed, suffixed with, or contains "COMMAND".
                command_name = method_name.Remove(index, 7);
            }
            else
            {
                command_name = method_name;
            }

            return command_name;
        }

        string InferFrontCommandName(string method_name)
        {
            int index = method_name.IndexOf("FRONT", StringComparison.CurrentCultureIgnoreCase);
            return index >= 0 ? method_name.Remove(index, 5) : null;
        }

        void HandleRejectedCommands(Dictionary<string, CommandInfo> rejected_commands)
        {
            foreach (var command in rejected_commands)
            {
                if (commands.ContainsKey(command.Key))
                {
                    commands[command.Key] = new CommandInfo()
                    {
                        proc = commands[command.Key].proc,
                        min_arg_count = command.Value.min_arg_count,
                        max_arg_count = command.Value.max_arg_count,
                        description = command.Value.description
                    };
                }
                else
                {
                    IssueErrorMessage("{0} is missing a front command.", command);
                }
            }
        }

        CommandInfo CommandFromParamInfo(ParameterInfo[] parameters, string Description)
        {
            int optional_args = 0;

            foreach (var param in parameters)
            {
                if (param.IsOptional)
                {
                    optional_args += 1;
                }
            }

            return new CommandInfo()
            {
                proc = null,
                min_arg_count = parameters.Length - optional_args,
                max_arg_count = parameters.Length,
                description = Description
            };
        }

        CommandArg EatArgument(ref string s)
        {
            var arg = new CommandArg();
            int space_index = s.IndexOf(' ');

            if (space_index >= 0)
            {
                arg.String = s.Substring(0, space_index);
                s = s.Substring(space_index + 1); // Remaining
            }
            else
            {
                arg.String = s;
                s = "";
            }

            return arg;
        }
    }
}
