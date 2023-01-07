using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class ArrayExtensions
    {


        public static string[] ToStringArray(this CommandArg[] args)
        {
            var result = new List<string>();
            foreach (var item in args)
            {
                try
                {
                    result.Add(item.String);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return result.ToArray();
        }

        public static int[] ToIntArray(this CommandArg[] args)
        {
            var result = new List<int>();
            foreach (var item in args)
            {
                try
                {
                    result.Add(item.Int);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return result.ToArray();
        }
        public static float[] ToFloatArray(this CommandArg[] args)
        {
            var result = new List<float>();
            foreach (var item in args)
            {
                try
                {
                    result.Add(item.Float);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return result.ToArray();
        }

        public static bool[] ToBoolArray(this CommandArg[] args)
        {
            var result = new List<bool>();
            foreach (var item in args)
            {
                try
                {
                    result.Add(item.Bool);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return result.ToArray();
        }

        public static List<int> GetPlayersIds(this string arg)
        {
            var result = new List<int>();
            string[] array = arg.Split(new char[]
                         {
                            '.'
                         });
            for (int i = 0; i < array.Length; i++)
            {
                int playerId;
                if (int.TryParse(array[i], out playerId) && !result.Contains(playerId))
                {
                    result.Add(playerId);
                }
            }

            return result;
        }

        public static string[] ToUpper(this string[] Strings)
        {
            return Strings.ToList().ToUpper().ToArray();
        }

        public static List<string> ToUpper(this List<string> Strings)
        {
            var Result = new List<string>();

            foreach (var item in Strings)
            {
                Result.Add(item.ToUpper());
            }
            return Result;
        }

        public static string[] ToLower(this string[] Strings)
        {
            return Strings.ToList().ToLower().ToArray();
        }


        public static List<string> ToLower(this List<string> Strings)
        {
            var Result = new List<string>();

            foreach (var item in Strings)
            {
                Result.Add(item.ToLower());
            }
            return Result;
        }

        public static List<PLAYER_PERMISSION> ToMod(this List<PlayerPermissions> Perms)
        {
            var Result = new List<PLAYER_PERMISSION>();

            foreach (var item in Perms)
            {
                Result.Add((PLAYER_PERMISSION)(int)item);
            }
            return Result;
        }
        public static List<PlayerPermissions> ToVanilla(this List<PLAYER_PERMISSION> Perms)
        {
            var Result = new List<PlayerPermissions>();

            foreach (var item in Perms)
            {
                if(item != PLAYER_PERMISSION.NONE)
                Result.Add((PlayerPermissions)(int)item);
            }
            return Result;
        }
    }
}
