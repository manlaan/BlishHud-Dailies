using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class Merchants
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies() {
            _dailies.Clear();
            _dailies.Add("psna_" + ((int)DateTime.UtcNow.AddHours(-8).DayOfWeek).ToString());
            return _dailies;
        }
    }
}
