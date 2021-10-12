using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls;
using Blish_HUD.Content;
using Microsoft.Xna.Framework;

namespace Manlaan.Dailies.Models
{
    public class 
        Event : IEquatable<Event>, IComparable<Event>
    {
        public string Name { get; set; }
        public string DailyId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public string Group { get; set; }
        public Panel Button { get; set; }
        public Color Color { get; set; }
        public string Waypoint { get; set; }
        public Daily Daily { get; set; }

        public Event() {
            Name = "";
            DailyId = "";
            StartTime = new DateTime();
            Duration = 0;
            EndTime = new DateTime();
            Group = "";
            Button = new Panel();
            Waypoint = "";
            Daily = new Daily();
        }

        public override string ToString() {
            return Name;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            Event objAsCat = obj as Event;
            if (objAsCat == null) return false;
            else return Equals(objAsCat);
        }
        public bool Equals(Event other) {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        public override int GetHashCode() {
            return 0;
        }
        public int SortByNameAscending(string name1, string name2) {
            return name1.CompareTo(name2);
        }
        public int CompareTo(Event compare) {
            if (compare == null)
                return 1;
            else
                return this.Name.CompareTo(compare.Name);
        }
    }
}
