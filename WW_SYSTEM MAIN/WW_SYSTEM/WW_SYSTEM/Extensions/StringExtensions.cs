using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class StringExtensions
    {



        public static bool Contains(this string s, string[] strings)
        {

            return Contains(s, strings.ToList());
        }

        public static bool Contains(this string s, string[] strings, out string FoundString)
        {
            return Contains(s, strings.ToList(), out FoundString);

        }

        public static bool Contains(this string s, List<string> strings)
        {

            foreach (var item in strings.ToUpper())
            {
                if (s.ToUpper().Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains(this string s, List<string> strings, out string FoundString)
        {
            foreach (var item in strings)
            {
                var d = item.ToUpper();
                if (s.ToUpper().Contains(d))
                {
                    FoundString = item;
                    return true;
                }
            }
            FoundString = string.Empty;
            return false;

        }
    }
}
