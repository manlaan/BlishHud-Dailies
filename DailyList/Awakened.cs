using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class Awakened
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies() {
            _dailies.Clear();
            switch (DateTime.UtcNow.DayOfWeek) {
                case DayOfWeek.Monday:
                    _dailies.Add("awakened_mon_1");
                    _dailies.Add("awakened_mon_2");
                    _dailies.Add("awakened_mon_3");
                    _dailies.Add("awakened_mon_4");
                    _dailies.Add("awakened_mon_5");
                    _dailies.Add("awakened_mon_6");
                    _dailies.Add("awakened_mon_7");
                    break;
                case DayOfWeek.Tuesday:
                    _dailies.Add("awakened_tues_1");
                    _dailies.Add("awakened_tues_2");
                    _dailies.Add("awakened_tues_3");
                    _dailies.Add("awakened_tues_4");
                    _dailies.Add("awakened_tues_5");
                    _dailies.Add("awakened_tues_6");
                    _dailies.Add("awakened_tues_7");
                    break;
                case DayOfWeek.Wednesday:
                    _dailies.Add("awakened_wed_1");
                    _dailies.Add("awakened_wed_2");
                    _dailies.Add("awakened_wed_3");
                    _dailies.Add("awakened_wed_4");
                    _dailies.Add("awakened_wed_5");
                    _dailies.Add("awakened_wed_6");
                    _dailies.Add("awakened_wed_7");
                    break;
                case DayOfWeek.Thursday:
                    _dailies.Add("awakened_thurs_1");
                    _dailies.Add("awakened_thurs_2");
                    _dailies.Add("awakened_thurs_3");
                    _dailies.Add("awakened_thurs_4");
                    _dailies.Add("awakened_thurs_5");
                    _dailies.Add("awakened_thurs_6");
                    _dailies.Add("awakened_thurs_7");
                    break;
                case DayOfWeek.Friday:
                    _dailies.Add("awakened_fri_1");
                    _dailies.Add("awakened_fri_2");
                    _dailies.Add("awakened_fri_3");
                    _dailies.Add("awakened_fri_4");
                    _dailies.Add("awakened_fri_5");
                    _dailies.Add("awakened_fri_6");
                    _dailies.Add("awakened_fri_7");
                    break;
                case DayOfWeek.Saturday:
                    _dailies.Add("awakened_sat_1");
                    _dailies.Add("awakened_sat_2");
                    _dailies.Add("awakened_sat_3");
                    _dailies.Add("awakened_sat_4");
                    _dailies.Add("awakened_sat_5");
                    _dailies.Add("awakened_sat_6");
                    _dailies.Add("awakened_sat_7");
                    break;
                case DayOfWeek.Sunday:
                    _dailies.Add("awakened_sun_1");
                    _dailies.Add("awakened_sun_2");
                    _dailies.Add("awakened_sun_3");
                    _dailies.Add("awakened_sun_4");
                    _dailies.Add("awakened_sun_5");
                    _dailies.Add("awakened_sun_6");
                    _dailies.Add("awakened_sun_7");
                    break;
            }
            return _dailies;
        }
    }
}
