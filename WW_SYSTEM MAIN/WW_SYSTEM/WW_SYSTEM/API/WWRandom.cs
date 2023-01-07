using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class WWRandom
    {
        
        public static float RandomMin(float Min)
        {
           
            return RandomFloat(Min, float.MaxValue);
        }

        public static float RandomMax(float Max)
        {
            return RandomFloat(float.MinValue, Max);
        }

        public static float RandomFloat(float Min, float Max)
        {
            var rnd = new Random(GetRandomSeed);
            double min = Min;
            double max = Max;
            double range = max - min;
            double sample = rnd.NextDouble();
            double scaled = (sample * range) + min;
            float f = (float)scaled;
            return f;
        }


        public static int RandomMin(int Min)
        {

            return RandomInt(Min, int.MaxValue);
        }

        public static int RandomMax(int Max)
        {
            return RandomInt(int.MinValue, Max);
        }

        public static int RandomInt(int Min, int Max)
        {
            var rnd = new Random(GetRandomSeed);
          
            if(Max < int.MaxValue)
            {
                Max += 1;
            }
            return rnd.Next(Min, Max);
        }

        public static bool RandomBool()
        {
            var rnd = new Random(GetRandomSeed);


            int prob = rnd.Next(100);
            return prob <= 50;
        }
        public static T Random<T>(this T[] items)
        {
            var rnd = new Random(GetRandomSeed);

            return items[rnd.Next(0, items.Length)];
        }

      
        public static T Random<T>(this List<T> items)
        {
            var rnd = new Random(GetRandomSeed);
            return items[rnd.Next(0, items.Count)];
        }


        public static ItemType RandomItem()
        {
            var random = new Random(GetRandomSeed);
            var values = Enum.GetValues(typeof(ItemType));
            return (ItemType)values.GetValue(random.Next(values.Length));
        }

        public static RoleTypeId RandomRole()
        {
            var random = new Random(GetRandomSeed);
            var values = Enum.GetValues(typeof(RoleTypeId));
            return (RoleTypeId)values.GetValue(random.Next(values.Length));
        }
        public static AmmoType RandomAmmo()
        {
            var random = new Random(GetRandomSeed);
            var values = Enum.GetValues(typeof(AmmoType));
            return (AmmoType)values.GetValue(random.Next(values.Length));
        }

        public static int LatesSeed = 0;

        public static int GetRandomSeed
        {
            get
            {
                var rnd = new Random(new Random(LatesSeed).Next(new Random(LatesSeed).Next(int.MinValue, -1111111111), new Random(LatesSeed).Next(1111111111, int.MaxValue))).Next(int.MinValue, int.MaxValue);
               
                if(LatesSeed != 0)
                {
                    rnd = UnityEngine.Random.Range(LatesSeed / 2, LatesSeed * UnityEngine.Random.Range(1111111111, int.MaxValue));
                    
                }
               
                    LatesSeed = rnd;
                    return rnd;
                
                
         
            }
        }

    }
}
