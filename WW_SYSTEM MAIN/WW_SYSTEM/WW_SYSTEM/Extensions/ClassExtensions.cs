using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class ClassExtensions
    {



        public static void RaMessage<T>(this T cl, string msg, bool success = false)
        {
            var type = typeof(T);

            foreach (var item in Round.GetPlayers())
            {
                if (item.GetRoles.RemoteAdmin)
                {
                    item.RaMessage(type.Name, msg, success);
                }
            }
        }

        public static void RaMessage<T>(this T cl, PLAYER_PERMISSION RequiredPerm, string msg, bool success = false)
        {
            var type = typeof(T);

            foreach (var item in Round.GetPlayers())
            {
                if (item.GetRoles.RemoteAdmin && item.IsPermitted(RequiredPerm))
                {
                    item.RaMessage(type.Name, msg, success);
                }
            }
        }


        public static void RaMessage<T>(this T cl, List<PLAYER_PERMISSION> RequiredPerms, string msg, bool success = false)
        {
            var type = typeof(T);

            foreach (var item in Round.GetPlayers())
            {
                if (item.GetRoles.RemoteAdmin && item.IsPermitted(RequiredPerms))
                {
                    item.RaMessage(type.Name, msg, success);
                }
            }
        }

    

        public static void Info<T>(this T cl, string msg)
        {
                var type = typeof(T);

                Logger.Info(type.Name, msg);
        }

        public static void Warn<T>(this T cl, string msg)
        {
            var type = typeof(T);

            Logger.Warn(type.Name, msg);
        }

        public static void Error<T>(this T cl, string msg)
        {
            var type = typeof(T);

            Logger.Error(type.Name, msg);
        }

        public static void Debug<T>(this T cl, string msg)
        {
            var type = typeof(T);

            Logger.Debug(type.Name, msg);
        }






        public static void RaMessage<T>(this T cl, string Prefix, string msg, bool success = false)
        {
            var type = typeof(T);

            foreach (var item in Round.GetPlayers())
            {
                if (item.GetRoles.RemoteAdmin)
                {
                    item.RaMessage(Prefix, msg, success);
                }
            }
        }

        public static void RaMessage<T>(this T cl, string Prefix, PLAYER_PERMISSION RequiredPerm, string msg, bool success = false)
        {
            var type = typeof(T);

            foreach (var item in Round.GetPlayers())
            {
                if (item.GetRoles.RemoteAdmin && item.IsPermitted(RequiredPerm))
                {
                    item.RaMessage(Prefix, msg, success);
                }
            }
        }


        public static void RaMessage<T>(this T cl, string Prefix, List<PLAYER_PERMISSION> RequiredPerms, string msg, bool success = false)
        {
            var type = typeof(T);

            foreach (var item in Round.GetPlayers())
            {
                if (item.GetRoles.RemoteAdmin && item.IsPermitted(RequiredPerms))
                {
                    item.RaMessage(Prefix, msg, success);
                }
            }
        }


        public static void Info<T>(this T cl, string Tag, string msg)
        {
            var type = typeof(T);

            Logger.Info(Tag, msg);
        }

        public static void Warn<T>(this T cl, string Tag, string msg)
        {
            var type = typeof(T);

            Logger.Warn(Tag, msg);
        }

        public static void Error<T>(this T cl, string Tag, string msg)
        {
            var type = typeof(T);

            Logger.Error(Tag, msg);
        }

        public static void Debug<T>(this T cl, string Tag, string msg)
        {
            var type = typeof(T);

            Logger.Debug(Tag, msg);
        }

    }
}
