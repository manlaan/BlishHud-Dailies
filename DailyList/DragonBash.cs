using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class DragonBash
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies(string startdate) {
            _dailies.Clear();

            /*
            {{#vardefine: day| {{#time: U| {{{3|{{#time: Y-m-d}}}}} + @@@ days }} }}
            {{#vardefine: daysSinceStart| {{#expr: floor( ( {{#var: day}}-{{#time: U| {{{1|requiered}}} }} )/(1*24*60*60) ) }} }}
            */
            /*  Holding until can test
            long today = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long firstday = ((DateTimeOffset)DateTime.Parse(startdate + " 00:00")).ToUnixTimeSeconds();
            long day = ((today - firstday) / 86400);

            long mod1 = day % 3;
            long mod2 = day % 4;

            _dailies.Add("dragon_elder");
            _dailies.Add("dragon_minion");
            switch (mod1) {
                case 1:
                    _dailies.Add("dragon_moa");
                    break;
                case 2:
                    _dailies.Add("dragon_mount");
                    break;
                case 0:
                    _dailies.Add("dragon_paper");
                    break;
            }
            switch (mod2) {
                case 3:
                    _dailies.Add("dragon_aim");
                    _dailies.Add("dragon_gladiator");
                    break;
                case 0:
                    _dailies.Add("dragon_aim");
                    _dailies.Add("dragon_holo");
                    break;
                case 1:
                    _dailies.Add("dragon_slapper");
                    _dailies.Add("dragon_gladiator");
                    break;
                case 2:
                    _dailies.Add("dragon_slapper");
                    _dailies.Add("dragon_holo");
                    break;
            }
            */
            return _dailies;
        }
        //https://wiki.guildwars2.com/wiki/Template:Daily_Dragon_Bash_rotation
        //https://wiki.guildwars2.com/wiki/Template:Daily_Dragon_Bash_rotation/achievement_names
    }
}
