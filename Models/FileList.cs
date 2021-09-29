using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace Manlaan.Dailies.Models
{
    public class FileList
    {
        [JsonPropertyName("file")]
        public string File { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
    }
}
