using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class FourWinds
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies(string startdate) {
            _dailies.Clear();

            /*  Not Needed.  Part of Daily API Group   */
            /*  API Really Slow at updating this, so adding it back in   */


            long today = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long firstday = ((DateTimeOffset)DateTime.Parse(startdate + " 00:00")).ToUnixTimeSeconds();
            long mod = ((today - firstday) / 86400) % 3;

            _dailies.Add("4284");  //Four Winds Zephyr
            _dailies.Add("4314");  //Watchknight Wrecker
            _dailies.Add("4317");  //Faster than Four Winds
            _dailies.Add("5432");  //Boss Blitz
            switch (mod) {
                case 0:
                    _dailies.Add("4283");  //Kite Chaser
                    _dailies.Add("4289");  //Banisher of Legends
                    break;
                case 1:
                    _dailies.Add("4280");  //Bundle Looter
                    _dailies.Add("4291");  //Gauntlet Gladiator
                    break;
                case 2:
                    _dailies.Add("4333");  //Day Trader
                    _dailies.Add("4319");  //Blitz Breaker
                    break;
            }
            return _dailies;
        }
        //https://wiki.guildwars2.com/wiki/Daily_Festival_of_the_Four_Winds

    }
}
