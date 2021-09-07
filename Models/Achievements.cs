using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.Dailies.Models
{
    class Achievement : IEquatable<Achievement>
    {
        public string Id { get; set; }
        public bool Done { get; set; }
        public int Current { get; set; }
        public int Max { get; set; }
        public string API { get; set; }

        public Achievement() {
            Id = "";
            API = "";
            Done = false;
            Current = 0;
            Max = 0;
        }

        public override string ToString() {
            return Id;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            Achievement objAsAchieve = obj as Achievement;
            if (objAsAchieve == null) return false;
            else return Equals(objAsAchieve);
        }
        public bool Equals(Achievement other) {
            if (other == null) return false;
            return (this.Id.Equals(other.Id));
        }
        public override int GetHashCode() {
            return 0;
        }
    }
}
