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
    public class Alert : IEquatable<Alert>, IComparable<Alert>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string DailyID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public DailyDetailsButton Button { get; set; }

        public Alert() {
            ID = "";
            Name = "";
            IsActive = true;
            DailyID = "";
            StartTime = new DateTime();
            Duration = 0;
            EndTime = new DateTime();
            Button = new DailyDetailsButton();
        }

        public override string ToString() {
            return Name;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            Alert objAsAlert = obj as Alert;
            if (objAsAlert == null) return false;
            else return Equals(objAsAlert);
        }
        public bool Equals(Alert other) {
            if (other == null) return false;
            return (this.ID.Equals(other.ID));
        }
        public override int GetHashCode() {
            return 0;
        }
        public int SortByNameAscending(string name1, string name2) {
            return name1.CompareTo(name2);
        }
        public int CompareTo(Alert compare) {
            if (compare == null)
                return 1;
            else
                return this.ID.CompareTo(compare.ID);
        }
    }
}
