using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.Dailies.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Manlaan.Dailies.Models
{
    public class DailyDetailsButton : Panel
    {
        public GlowButton CompleteButton { get; set; }
        public GlowButton TrackedButton { get; set; }
        public Label TimeButton { get; set; }

        public DailyDetailsButton() {
            CompleteButton = new GlowButton();
            TrackedButton = new GlowButton();
            TimeButton = new Label();
        }
    }
}
