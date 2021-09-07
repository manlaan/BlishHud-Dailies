using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.DailyList
{
    class Wintersday
    {
        private static List<string> _dailies = new List<string>();
        public static List<string> Dailies(string startdate) {
            _dailies.Clear();

            /*
             * Reviewing the below data, there is a bit of a pattern in terms of which achievements can be coincident. Each day:
            Group 1 — One of: Saving Wintersday (infinirarium) OR Toypocalypse Today (toypocalpse) OR Snowball Mayhem (pvp)
            Group 2 — One of: Winter Wonderland Winner (JP) OR That Wintersday Ring (bells)
            Group 3A (only if JP) — One of: Making Some Wintersday Friends (build a snowman) OR Spreading Wintersday Joy (donation)
            Group 3B (only if bells) — One of: Wintersday Snowball Slinger (snowballs) OR Dashing Through the Snow (race)
            If you put them in the order I've described above, it seems that tomorrow (27th) will be Group 1 = Snowball Mayhem for sure, and Group 2 = Winter Wonderland Winner probably (less certain than group 1, but doesn't appear to be 3 in a row the same at any time).
            There doesn't seem to be a pattern yet for Group 3, but there does seem to be some correlation between a day with the JP and bells... so I've split group 3 into 3A and 3B. Since I've guessed JP is tomorrow, I'd then suspect one of group 3A, and Making Some Wintersday Friends has been a few days since it's appeared, so i guess this one. -Chieftain AlexUser Chieftain Alex sig.png 19:07, 26 December 2020 (UTC)

             
2020-12-16: Saving Wintersday, Spreading Wintersday Joy, That Wintersday Ring
2020-12-17: Making Some Wintersday Friends, Toypocalypse Today, Winter Wonderland Winner
2020-12-18: Making Some Wintersday Friends, Snowball Mayhem, Winter Wonderland Winner
2020-12-19: Dashing Through the Snow, Saving Wintersday, That Wintersday Ring
2020-12-20: Toypocalypse Today, Winter Wonderland Winner, Wintersday Snowball Slinger
2020-12-21: Snowball Mayhem, Spreading Wintersday Joy, That Wintersday Ring
2020-12-22: Making Some Wintersday Friends, Saving Wintersday, Winter Wonderland Winner
2020-12-23: Dashing Through the Snow, That Wintersday Ring, Toypocalypse Today
2020-12-24: Snowball Mayhem, Winter Wonderland Winner, Wintersday Snowball Slinger
2020-12-25: Saving Wintersday, Spreading Wintersday Joy, That Wintersday Ring
2020-12-26: Dashing Through the Snow, That Wintersday Ring, Toypocalypse Today
2020-12-27: Snowball Mayhem, Wintersday Snowball Slinger, Winter Wonderland Winner
            */

            /*  Holding until can test

            long today = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long firstday = ((DateTimeOffset)DateTime.Parse(startdate + " 00:00")).ToUnixTimeSeconds();
            long day = ((today - firstday) / 86400);

            long mod1 = day % 3;
            long mod2 = day % 4;

            */
            return _dailies;
        }
        //https://wiki.guildwars2.com/wiki/Wintersday_Daily
        //https://wiki.guildwars2.com/wiki/Talk:Wintersday_Daily

    }
}
