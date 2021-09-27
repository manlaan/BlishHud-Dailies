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

namespace Manlaan.Dailies.Controls
{
    public class EventWindow : WindowBase2
    {
        #region Load Static

        private static Texture2D _blankBackground, _wndBackground, _btnBackground, _pageIcon;

        static EventWindow() {
            _blankBackground = Module.ModuleInstance.ContentsManager.GetTexture("blank.png");
            _wndBackground = Module.ModuleInstance.ContentsManager.GetTexture("1863949.png");
            _btnBackground = Module.ModuleInstance.ContentsManager.GetTexture("button.png");
            _pageIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\42684.png");
        }
        #endregion


        private Dropdown _selectCategory , _selectTracked;
        private Point WinSize = new Point();
        private Panel _dailyPanel;
        private string _dailyCategory = "";
        private Panel _parentPanel;

        public EventWindow(Point size) : base() {
            WinSize = size;
            this.CanClose = false;
            this.Title = "Events";
            this.Emblem = _pageIcon;
            BuildWindow();
        }

        private void BuildWindow() {
            ConstructWindow(_blankBackground, new Rectangle(0, 0, WinSize.X, WinSize.Y), new Rectangle(0, 0, WinSize.X, WinSize.Y));
            _parentPanel = new Panel() {
                CanScroll = false,
                Size = new Point(WinSize.X, WinSize.Y),
                Location = new Point(0, 0),
                Parent = this,
            };
            Image bgimage = new Image(_wndBackground) {
                Location = new Point(0, 0),
                Size = _parentPanel.Size,
                Parent = _parentPanel,
            };

            _selectCategory = new Dropdown() {
                Location = new Point(15, 15),
                Width = _parentPanel.Width - 30,
                Parent = _parentPanel,
            };
            _selectCategory.ValueChanged += delegate {
                _dailyCategory = (_selectCategory.SelectedItem.Equals("All") ? "" : _selectCategory.SelectedItem);
                UpdateDailyPanel();
            };
            _selectTracked = new Dropdown() {
                Location = new Point(_selectCategory.Location.X, _selectCategory.Bottom + 3),
                Width = _parentPanel.Width - 30,
                Parent = _parentPanel,
            };
            _selectTracked.Items.Add("Tracked - Incomplete");
            _selectTracked.Items.Add("Tracked - All");
            _selectTracked.Items.Add("All");
            _selectTracked.SelectedItem = "Tracked - Incomplete";
            _selectTracked.ValueChanged += delegate {
                _dailyCategory = (_selectCategory.SelectedItem.Equals("All") ? "" : _selectCategory.SelectedItem);
                UpdateDailyPanel();
            };

            _dailyPanel = new Panel() {
                Location = new Point(10, _selectTracked.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 20, _parentPanel.Size.Y - _selectTracked.Bottom - 15),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = true,
            };

            int curY = 0;
            foreach (Category c in Module._eventGroups) {
                Panel catPanel = new Panel() {
                    Location = new Point(0, curY),
                    Parent = _dailyPanel,
                    BackgroundColor = Color.Blue,
                    Size = new Point(_dailyPanel.Size.X, 20),
                };
                Label catLabel = new Label() {
                    Parent = catPanel,
                    Text = c.Name,
                    Size = new Point(100,20),
                    BackgroundColor = Color.Red,
                };
                foreach (Event e in Module._events) {
                    if (e.Group.Equals(c.Name)) 
                        e.Button = CreateDailyButton(catPanel, e, 0);
                }
                curY += 20;
            }

            _dailyCategory = "";
            UpdateDailyPanel();
        }

        public Panel CreateDailyButton(Panel panel, Event e, int Y) {
            int width = (int)((WinSize.X - 100) / 40);
            int buttonwidth = (e.Duration / 15) * width;
            int buttonstart = (e.StartTime.Hour * 4 * width) + ((e.StartTime.Minute / 15) * width) + 100;

            Panel EventButton = new Panel() {
                Size = new Point(buttonwidth, 20),
                Location = new Point(buttonstart, Y),
                Parent = panel,
                BackgroundColor = Color.Red
            };

            Label Desc = new Label() {
                //Location = new Point(EventButton.Left, EventButton.Top),
                Width = EventButton.Width,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = EventButton,
                Text = e.Name,
                BasicTooltipText = e.Name + "\n" + e.StartTime.Hour.ToString() + ":" + e.StartTime.Minute.ToString(),
            };

            return EventButton;
        }

        public void UpdateDailyPanel() {
            List<Category> categories = Module._eventGroups;
            foreach (Category cat in categories) {
                cat.IsActive = false;
            }

            foreach (Event e in Module._events) {
                Daily d = Module._dailies.Find(x => x.Id.Equals(e.DailyID));
                e.Button.Visible = true;
                if (Module.InSection(d, "Tracked - Incomplete", "", "")) {
                    categories.Find(x => x.Name.Equals(d.TimesGroup)).IsActive = true;
                }
                if (Module.InSection(d, "Tracked - Incomplete", "", _dailyCategory)) {
                    e.Button.Visible = true;
                }
            }

            _dailyPanel.RecalculateLayout();

            _selectCategory.Items.Clear();
            _selectCategory.Items.Add("All");
            foreach (Category cat in categories) {
                if (cat.IsActive)
                    _selectCategory.Items.Add(cat.Name);
            }
            _selectCategory.SelectedItem = (_dailyCategory.Equals("") ? "All" : _dailyCategory);
        }
    }
}
