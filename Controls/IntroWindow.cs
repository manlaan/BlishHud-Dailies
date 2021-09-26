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
using System.IO;
using Blish_HUD.Graphics.UI;

namespace Manlaan.Dailies.Controls
{
    public class IntroWindow : WindowBase2
    {
        #region Load Static

        private static Texture2D _blankBackground, _wndBackground, _pageIcon;

        static IntroWindow() {
            _blankBackground = Module.ModuleInstance.ContentsManager.GetTexture("blank.png");
            _wndBackground = Module.ModuleInstance.ContentsManager.GetTexture("1863949.png");
            _pageIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\42684.png");
        }
        #endregion


        public IntroWindow(Point size) : base() {
            this.Size = size;
            this.Title = "Dailies";
            this.Emblem = _pageIcon;
            BuildWindow();
        }

        private void BuildWindow() {
            ConstructWindow(_blankBackground, new Rectangle(0, 0, Size.X, Size.Y), new Rectangle(0, 0, Size.X, Size.Y));
            Panel parentPanel = new Panel() {
                CanScroll = false,
                Size = new Point(Size.X, Size.Y),
                Location = new Point(0, 0),
                Parent = this,
            };
            Image bgimage = new Image(_wndBackground) {
                Location = new Point(0, 0),
                Size = parentPanel.Size,
                Parent = parentPanel,
            };

            Label Desc = new Label() {
                Location = new Point(20, 20),
                Width = parentPanel.Width - 40,
                AutoSizeHeight = true,
                WrapText = true,
                Parent = parentPanel,
                Text = "Welcome to the Dailies Module\n\nTo get started, click on the Blish icon and then the Dailies tab on the left side.  \n\nChange the pull down box to 'All' and start adding in the Dailies you want to track by pressing the eye icon.\n\nOnce you have your Dailies added, you can use the corner icon to open a quick access small window.",
            };

            Checkbox DontShow = new Checkbox() {
                Location = new Point(20, parentPanel.Bottom - 70),
                Width = 100,
                Parent = parentPanel,
                Text = "Dont Show This Again",
                Checked = Module._settingDontShowIntro.Value
            };
            DontShow.CheckedChanged += delegate { Module._settingDontShowIntro.Value = DontShow.Checked; };

            StandardButton Close = new StandardButton() {
                Location = new Point(parentPanel.Width - 100 - 20, parentPanel.Bottom - 80),
                Width = 100,
                Parent = parentPanel,
                Text = "Close",
            };
            Close.Click += delegate { Hide(); };
        }
    }
}
