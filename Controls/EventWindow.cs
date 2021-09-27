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
        private int _hoursToShow = 4;
        private int _categoryHeight = 25;
        private float _minWidth = 0;
        private Panel _timeMarker;

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
                Width = ((_parentPanel.Width - 50) / 2 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
                Parent = _parentPanel,
            };
            _selectCategory.ValueChanged += delegate {
                _dailyCategory = (_selectCategory.SelectedItem.Equals("All") ? "" : _selectCategory.SelectedItem);
                UpdateDailyPanel();
            };
            _selectTracked = new Dropdown() {
                Location = new Point(_selectCategory.Right + 10, _selectCategory.Top),
                Width = ((_parentPanel.Width - 50) / 2 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
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

            _minWidth = (float)((_parentPanel.Size.X - 40 - 100 - 15) / (float)(_hoursToShow * 60));


            Panel timePanel = new Panel() {
                Location = new Point(15, _selectCategory.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 40, _categoryHeight),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            string timeformat = "h:mm tt";
            if (Module._setting24HrTime.Value) timeformat = "H:mm";

            for (int i = 0; i < (_hoursToShow * 4); i++) {
                var t = RoundDown(DateTime.UtcNow.AddMinutes(-15).AddMinutes(i * 15).ToLocalTime(), TimeSpan.FromMinutes(15));
                float w = _minWidth * 15;
                float y = _minWidth * 15 * i;
                Label timeLabel = new Label() {
                    Size = new Point((int)w, _categoryHeight),
                    Location = new Point(100 + (int)y, 0),
                    Parent = timePanel,
                    Text = t.ToString(timeformat),
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
            }


            _dailyPanel = new Panel() {
                Location = new Point(15, timePanel.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 40, _parentPanel.Size.Y - timePanel.Bottom - 15),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            int curY = 0;
            int cnt = 0;
            foreach (Category c in Module._eventGroups) {
                Panel catPanel = new Panel() {
                    Location = new Point(0, curY),
                    Parent = _dailyPanel,
                    Size = new Point(_dailyPanel.Size.X - 15, _categoryHeight),
                    BackgroundTexture = (cnt % 2 == 0) ? _btnBackground : _blankBackground,
                };
                Label catLabel = new Label() {
                    Parent = catPanel,
                    Text = c.Name,
                    Size = new Point(100, _categoryHeight),
                    Location = new Point(5,0),
                };
                foreach (Event e in Module._events) {
                    if (e.Group.Equals(c.Name)) 
                        e.Button = CreateDailyButton(catPanel, e);
                }
                curY += _categoryHeight + 2;
                cnt++;
            }

            float offset = (DateTime.UtcNow.Hour * 60 * _minWidth) + ((RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15)).Minute) * _minWidth);
            float curtime = ((DateTime.UtcNow.Hour * 60 * _minWidth) + (DateTime.UtcNow.Minute) * _minWidth);
            float timeloc = 100 + (curtime - offset);
            _timeMarker = new Panel() {
                Parent = _dailyPanel,
                Size = new Point(2, _dailyPanel.Height),
                BackgroundColor = Color.White,
                Location = new Point((int)(timeloc), 0),
                ZIndex = 10
            };

            _dailyCategory = "";
            UpdateDailyPanel();
        }

        public Panel CreateDailyButton(Panel panel, Event e) {
            float offset = (DateTime.UtcNow.Hour * 60 * _minWidth) + ((RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15)).Minute) * _minWidth);
            float buttonwidth = (e.Duration) * _minWidth;
            float buttonstart = (e.StartTime.Hour * 60 * _minWidth) + ((e.StartTime.Minute) * _minWidth) + 100 - offset;

            if ((buttonstart + buttonwidth) > 100 && (buttonstart + buttonwidth) < WinSize.X) {
                if (buttonstart < 100) {
                    buttonwidth = buttonstart + buttonwidth - 100;
                    buttonstart = 100;
                }

                Panel EventButton = new Panel() {
                    Size = new Point((int)buttonwidth-2, _categoryHeight),
                    Location = new Point((int)buttonstart, 0),
                    Parent = panel,
                    BackgroundColor = e.Color,
                };

                Label EventDesc = new Label() {
                    Size = new Point(EventButton.Size.X - 10, EventButton.Size.Y),
                    Location = new Point(5, 0),
                    AutoSizeHeight = false,
                    WrapText = false,
                    Parent = EventButton,
                    Text = e.Name,
                    BasicTooltipText = e.Name + "\n" + e.StartTime.Hour.ToString() + ":" + e.StartTime.Minute.ToString(),
                    TextColor = Color.Black,
                    Font = Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
                };

                return EventButton;
            } else {
                return new Panel();
            }
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

        private static DateTime RoundDown(DateTime dt, TimeSpan d) {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }
    }
}
