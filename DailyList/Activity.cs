using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class Activity
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies() {
            _dailies.Clear();
            _dailies.Add("activity_" + ((int)DateTime.UtcNow.DayOfWeek).ToString());
            return _dailies;
        }
    }
}
