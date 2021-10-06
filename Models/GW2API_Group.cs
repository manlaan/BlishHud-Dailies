using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Manlaan.Dailies.Models
{
    class GW2API_Group
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("order")]
        public int Order { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("achievements")]
        public int[] Achievements { get; set; }

        public GW2API_Group() {
            Id = 0;
            Name = "";
            Description = "";
            Icon = "";
            Order = 0;
            Achievements = new int[] { };
        }
    }
}
