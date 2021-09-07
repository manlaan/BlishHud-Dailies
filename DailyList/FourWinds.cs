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

            /*  Not Needed.  Part of Daily API Group 
               
             
            long today = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long firstday = ((DateTimeOffset)DateTime.Parse(startdate + " 00:00")).ToUnixTimeSeconds();
            long mod = ((today - firstday) / 86400) % 3;

            _dailies.Add("fourwind_zephyr");
            _dailies.Add("fourwind_watchknight");
            _dailies.Add("fourwind_faster");
            switch (mod) {
                case 0:
                    _dailies.Add("fourwind_kite");
                    _dailies.Add("fourwind_banisher");
                    break;
                case 1:
                    _dailies.Add("fourwind_bundle");
                    _dailies.Add("fourwind_gladiator");
                    break;
                case 2:
                    _dailies.Add("fourwind_trader");
                    _dailies.Add("fourwind_blitz");
                    break;
            }
            */
            return _dailies;
        }
        //https://wiki.guildwars2.com/wiki/Daily_Festival_of_the_Four_Winds

    }
}
