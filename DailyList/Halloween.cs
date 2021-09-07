using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class Halloween
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies(string startdate) {
            _dailies.Clear();

            /*
            {{#vardefine:festival|2020-10-13}}
            {{#vardefine:offset|7}}<!-- festival started with day N -->
            {{#vardefine:today offset|{{#expr: (floor(({{#time:U}} - {{#time:U|{{#var:festival}}}}) / 86400) + {{#var: offset}} - 1) mod 9 }}}}
            */
            /*  Holding until can test

            long today = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long firstday = ((DateTimeOffset)DateTime.Parse(startdate + " 00:00")).ToUnixTimeSeconds();
            long mod = ((today - firstday) / 86400) % 9;

            switch (mod) {
                case 7:
                    _dailies.Add("halloween_againstking");
                    _dailies.Add("halloween_cravingquaggan");
                    _dailies.Add("halloween_festivefeud");
                    _dailies.Add("halloween_costumebrawl");
                    _dailies.Add("halloween_racer");
                    break;
                case 8:
                    _dailies.Add("halloween_reapersreturn");
                    _dailies.Add("halloween_shutlocked");
                    _dailies.Add("halloween_terrorlabyrinth");
                    _dailies.Add("halloween_trickortreat");
                    _dailies.Add("halloween_tourlabyrinth");
                    break;
                case 0:
                    _dailies.Add("halloween_againstking");
                    _dailies.Add("halloween_candycraver");
                    _dailies.Add("halloween_festivefeud");
                    _dailies.Add("halloween_costumebrawl");
                    _dailies.Add("halloween_pumpkinhacker");
                    break;
                case 1:
                    _dailies.Add("halloween_cravingquaggan");
                    _dailies.Add("halloween_jumpingmad");
                    _dailies.Add("halloween_pumpkinhacker");
                    _dailies.Add("halloween_shutlocked");
                    _dailies.Add("halloween_madinquist");
                    break;
                case 2:
                    _dailies.Add("halloween_grimreaper");
                    _dailies.Add("halloween_reapersreturn");
                    _dailies.Add("halloween_racer");
                    _dailies.Add("halloween_terrorlabyrinth");
                    _dailies.Add("halloween_trickortreat");
                    break;
                case 3:
                    _dailies.Add("halloween_candycraver");
                    _dailies.Add("halloween_jumpingmad");
                    _dailies.Add("halloween_racer");
                    _dailies.Add("halloween_shutlocked");
                    _dailies.Add("halloween_madinquist");
                    break;
                case 4:
                    _dailies.Add("halloween_againstking");
                    _dailies.Add("halloween_cravingquaggan");
                    _dailies.Add("halloween_grimreaper");
                    _dailies.Add("halloween_costumebrawl");
                    _dailies.Add("halloween_tourlabyrinth");
                    break;
                case 5:
                    _dailies.Add("halloween_festivefeud");
                    _dailies.Add("halloween_reapersreturn");
                    _dailies.Add("halloween_terrorlabyrinth");
                    _dailies.Add("halloween_tourlabyrinth");
                    _dailies.Add("halloween_trickortreat");
                    break;
                case 6:
                    _dailies.Add("halloween_candycraver");
                    _dailies.Add("halloween_grimreaper");
                    _dailies.Add("halloween_jumpingmad");
                    _dailies.Add("halloween_pumpkinhacker");
                    _dailies.Add("halloween_madinquist");
                    break;
            }
            */

            return _dailies;

        }
        //https://wiki.guildwars2.com/wiki/Halloween_Daily
        //https://wiki.guildwars2.com/wiki/Template:Halloween_Daily_rotation
        //https://wiki.guildwars2.com/wiki/Template:Halloween_Daily_rotation/element

    }
}
