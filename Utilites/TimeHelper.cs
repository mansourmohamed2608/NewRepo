using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    public static class TimeHelper
    {
        public static string ConvertTimeCreateToString(DateTime timeCreated)
        {
            TimeSpan time = (DateTime.UtcNow) - (timeCreated);
            if (time.Days >= 365)
            {
                int years = time.Days / 365;
                return $"{years} Y";
            }
            if (time.Days >= 30)
            {
                int months = time.Days / 30;
                return $"{months} M";
            }
            if (time.Days > 0)
                return $"{time.Days} D";
            if (time.Hours > 0)
                return $"{time.Hours} H";
            if (time.Minutes > 0)
                return $"{time.Minutes} Minutes";
            if (time.Seconds > 0)
                return $"{time.Seconds} S";
            return "1 S";
        }
    }
}
