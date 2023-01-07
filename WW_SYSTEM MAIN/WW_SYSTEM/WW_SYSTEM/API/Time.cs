using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.API
{
    public static class Time
    {
        public static string ToTimeString(this int Seconds, FormatTime format)
        {
            return ConvertTime(Seconds, format);
        }


        public static string ConvertTime(int SecondsToConvert ,FormatTime type)
        {
            string data = "";
            TimeSpan t = TimeSpan.FromSeconds(SecondsToConvert);



            string Hours = t.Hours.ToString();
            string Minutes = t.Minutes.ToString();
            string Seconds = t.Seconds.ToString();
            string Milliseconds = t.ToString();
            if (Hours.Length == 1)
                Hours = "0" + Hours;

            if (Minutes.Length == 1)
                Minutes = "0" + Minutes; 

            if (Seconds.Length == 1)
                Seconds = "0" + Seconds;






            switch (type) {
                case FormatTime.Hours: {

                        data = Hours;
                        break;
                    }

                case FormatTime.Minutes: {

                            data = Minutes;
                     
                        break;
                    }
                case FormatTime.Seconds: {

                        data = Seconds;

                        break;
                    
                    
                }
                case FormatTime.HoursMinutes: {


                        data = Hours + ":" + Minutes;
                        break;
                    

                }
                case FormatTime.HoursMinutesSeconds: { 
                    
                    
                    data = Hours + ":" + Minutes + ":" + Seconds;
                        break;
                    }
                case FormatTime.HoursMinutesSecondsMilliseconds:
                    {


                        data = Hours + ":" + Minutes + ":" + Seconds + ":" + Milliseconds;
                        break;
                    }

                case FormatTime.MinutesSeconds: {
                        

                        data = Minutes + ":" + Seconds;
                        break;
                    
                    }

                case FormatTime.MinutesSecondsMilliseconds:
                    {


                        data = Minutes + ":" + Seconds + ":" + Milliseconds;
                        break;

                    }

                 


            }
            return data;


        }


        public static float realtimeSinceStartup { get { return UnityEngine.Time.realtimeSinceStartup; } }

    }
    public enum FormatTime { 
    
    Hours,
    Minutes,
    Seconds,
    Milliseconds,
    HoursMinutes,
    HoursMinutesSeconds,
    HoursMinutesSecondsMilliseconds,
    MinutesSeconds,
    MinutesSecondsMilliseconds,
    
    
    
    }
}
