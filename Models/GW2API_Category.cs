using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Manlaan.Dailies.Models
{
    class GW2API_Category
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("order")]
        public int Order { get; set; }
        [JsonPropertyName("categories")]
        public int[] Categories { get; set; }

        public GW2API_Category() {
            Id = "";
            Name = "";
            Description = "";
            Order = 0;
            Categories = new int[] { };
        }
    }
}
