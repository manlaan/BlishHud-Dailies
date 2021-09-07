using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace Manlaan.Dailies.Models
{
    public class DailySettingEntry : IEquatable<DailySettingEntry>
    {
        [JsonPropertyName("category")]
        public string Id { get; set; }
        [JsonPropertyName("IsTracked")]
        public bool IsTracked { get; set; }
        [JsonPropertyName("IsCopmlete")]
        public bool IsComplete { get; set; }

        public DailySettingEntry() {
            Id = "";
            IsTracked = false;
            IsComplete = false;
        }

        public override string ToString() {
            return Id;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            DailySettingEntry objAsEntry = obj as DailySettingEntry;
            if (objAsEntry == null) return false;
            else return Equals(objAsEntry);
        }
        public bool Equals(DailySettingEntry other) {
            if (other == null) return false;
            return (this.Id.Equals(other.Id));
        }
        public override int GetHashCode() {
            return 0;
        }
    }
}
