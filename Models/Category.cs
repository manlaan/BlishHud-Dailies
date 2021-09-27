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

namespace Manlaan.Dailies.Models
{
    public class Category : IEquatable<Category>, IComparable<Category>
    {
        public string Name { get; set; }
        public string Set { get; set; }
        public bool IsActive { get; set; }
        public MenuItem menuItem { get; set; }
        public Panel CategoryPanel { get; set; }

        public Category() {
            Name = "";
            Set = "";
            IsActive = true;
            menuItem = new MenuItem();
            CategoryPanel = new Panel();
        }

        public override string ToString() {
            return Name;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            Category objAsCat = obj as Category;
            if (objAsCat == null) return false;
            else return Equals(objAsCat);
        }
        public bool Equals(Category other) {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        public override int GetHashCode() {
            return 0;
        }
        public int SortByNameAscending(string name1, string name2) {
            return name1.CompareTo(name2);
        }
        public int CompareTo(Category compare) {
            if (compare == null)
                return 1;
            else
                return this.Name.CompareTo(compare.Name);
        }
    }
}
