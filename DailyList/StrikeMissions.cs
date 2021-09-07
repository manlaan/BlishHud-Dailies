using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class StrikeMissions
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies() {
            _dailies.Clear();

            /*
            From Wiki: 
            {{#vardefine: secondsSinceFirst|{{#expr: {{time|U}} - 1574119800 <!-- Unix time of 18. November 2019 23:30 UTC -->}} }}<!--
            -->{{#vardefine: fullWeeksSinceFirst|{{#expr: floor({{#var:secondsSinceFirst}} / 604800 <!-- Number of seconds in a week -->) }} }}<!--
            -->{{#vardefine: index|{{#expr: {{#var:fullWeeksSinceFirst}} mod 3}} }}<!--
            -->{{#vardefine: lastChanged|{{time | H:i j F Y | now - {{#expr: {{#var:secondsSinceFirst}} - {{#var:fullWeeksSinceFirst}}*604800}} seconds}} }}<!--
            */

            /*  For Weeklies, which dont work correctly due to resetting daily
            long today = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long firstday = 1574119800;
            long mod = ((today - firstday) / 604800) % 3;

            switch (mod) {
                case 2:
                    _dailies.Add("strike_bone");
                    break;
                case 0:
                    _dailies.Add("strike_voiceclaw");
                    break;
                case 1:
                    _dailies.Add("strike_fraenir");
                    break;
            }
            */
            return _dailies;
        }
        //https://wiki.guildwars2.com/wiki/Template:Bjora_Marches_strike_mission
        //https://wiki.guildwars2.com/wiki/Template:Daily_Strike_Mission
    }
}
