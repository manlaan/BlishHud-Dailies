using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace Manlaan.Dailies.Models
{
    public class Daily : IEquatable<Daily> , IComparable<Daily>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("clipboard")]
        public string Clipboard { get; set; }
        [JsonPropertyName("waypoint")]
        public string Waypoint { get; set; }
        [JsonPropertyName("wiki")]
        public string Wiki { get; set; }
        [JsonPropertyName("timer")]
        public string Timer { get; set; }
        [JsonPropertyName("achievement")]
        public string Achievement { get; set; }
        [JsonPropertyName("api")]
        public string API { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonIgnore]
        public bool IsTracked { get; set; }
        [JsonIgnore]
        public bool IsComplete { get; set; }
        [JsonIgnore]
        public bool IsDaily { get; set; }
        [JsonIgnore]
        public DailyDetailsButton Button { get; set; }
        [JsonIgnore]
        public DailyDetailsButton MiniButton { get; set; }

        public Daily() {
            Id = "";
            Name = "";
            Clipboard = "";
            Waypoint = "";
            Wiki = "";
            Timer = "";
            Achievement = "";
            API = "";
            Icon = "";
            Note = "";
            Category = "";
            IsTracked = false;
            IsComplete = false;
            IsDaily = false;
            Button = new DailyDetailsButton();
            MiniButton = new DailyDetailsButton();
    }

    public override string ToString() {
            return "ID: " + Id + "   Name: " + Name;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            Daily objAsDaily = obj as Daily;
            if (objAsDaily == null) return false;
            else return Equals(objAsDaily);
        }
        public int SortByNameAscending(string name1, string name2) {

            return name1.CompareTo(name2);
        }
        public int CompareTo(Daily compareDaily) {
            if (compareDaily == null)
                return 1;
            else
                return this.Id.CompareTo(compareDaily.Id);
        }
        public bool Equals(Daily other) {
            if (other == null) return false;
            return (this.Id.Equals(other.Id));
        }
        public override int GetHashCode() {
            return 0;
        }
    }
}
