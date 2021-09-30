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
            _pageIcon = Module.ModuleInstance.ContentsManager.GetTexture("clock.png");
        }
        #endregion


        private Dropdown _selectTracked, _selectSet;
        private Point WinSize = new Point();
        private Panel _eventPanel;
        private string _selectedTracked = "";
        //private string _selectedSet = "";
        private Panel _parentPanel;
        private int _categoryHeight = 25;
        private float _minuteWidth = 0;
        private Panel _timeMarker;
        private Panel _timePanel;
        private DateTime _prevTime = new DateTime();
        private List<string> _eventSets = new List<string>();
        private List<Event> _events = new List<Event>();
        private List<Category> _eventGroups = new List<Category>();


        public EventWindow(Point size) : base() {
            _eventSets.Add("Daily");
            _eventSets.Add("Core");
            _eventSets.Add("Heart of Thorns");
            _eventSets.Add("Path of Fire");
            _eventSets.Add("Living World Season 4");
            _eventSets.Add("Icebrood Saga");
            foreach (Daily d in Module._dailies) {
                if (d.TimesSet.Length>0 && !_eventSets.Contains(d.TimesSet))
                    _eventSets.Add(d.TimesSet);
            }

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

            _selectSet = new Dropdown() {
                Location = new Point(15, 15),
                Width = ((_parentPanel.Width - 50) / 2 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
                Parent = _parentPanel,
            };
            _selectSet.Items.Add("All");
            foreach (string s in _eventSets) {
                _selectSet.Items.Add(s);
            }
            _selectSet.SelectedItem = "All";
            _selectSet.ValueChanged += delegate {
                //_selectedSet = (_selectSet.SelectedItem.Equals("All") ? "" : _selectSet.SelectedItem);
                UpdatePanel();
            };
            //_selectedSet = "";

            _selectTracked = new Dropdown() {
                Location = new Point(_selectSet.Right + 5, _selectSet.Top),
                Width = ((_parentPanel.Width - 50) / 2 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
                Parent = _parentPanel,
            };
            _selectTracked.Items.Add("Tracked - Incomplete");
            _selectTracked.Items.Add("Tracked - All");
            _selectTracked.Items.Add("All");
            _selectTracked.SelectedItem = "Tracked - Incomplete";
            _selectTracked.ValueChanged += delegate {
                _selectedTracked = (_selectTracked.SelectedItem.Equals("All") ? "" : _selectTracked.SelectedItem);
                UpdatePanel();
            };
            _selectedTracked = "Tracked - Incomplete";

            _minuteWidth = ((float)_parentPanel.Size.X - 25 - 100 - 15) / (float.Parse(Module._settingEventHours.Value) * 60);


            _timePanel = new Panel() {
                Location = new Point(15, _selectSet.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 25, _categoryHeight),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            _eventPanel = new Panel() {
                Location = new Point(15, _timePanel.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 25, _parentPanel.Size.Y - _timePanel.Bottom - 15),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            _timeMarker = new Panel() {
                Parent = _parentPanel,
                Size = new Point(3, _eventPanel.Height),
                BackgroundColor = Color.White,
                ZIndex = 10
            };

            UpdatePanel();
        }

        public Panel CreateButton(Panel panel, Event e) {
            float offset = (DateTime.UtcNow.Hour * 60 * _minuteWidth) + ((RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15)).Minute) * _minuteWidth);
            float buttonwidth = (e.Duration) * _minuteWidth;
            float buttonstart = ((e.StartTime.Date-DateTime.UtcNow.Date).Days * 1440 * _minuteWidth) + (e.StartTime.Hour * 60 * _minuteWidth) + ((e.StartTime.Minute) * _minuteWidth) + 100 - offset;

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

            string timeformat = "h:mm tt";
            if (Module._setting24HrTime.Value) timeformat = "H:mm";

            Label EventDesc = new Label() {
                Size = new Point(EventButton.Size.X - 10, _categoryHeight),
                Location = new Point(5, 0),
                AutoSizeHeight = false,
                WrapText = false,
                Parent = EventButton,
                Text = e.Name,
                BasicTooltipText = e.Name + "\n" + e.StartTime.ToLocalTime().ToString(timeformat) + " - " + e.EndTime.ToLocalTime().ToString(timeformat) + (Module._settingDebug.Value ? "\n\nbutton: " + buttonstart.ToString() + "\nPanel: "+panel.Size.X.ToString()+","+panel.Size.Y.ToString() : ""),
                TextColor = Color.Black,
                Font = Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            };
            EventDesc.Click += delegate {
                ClipboardUtil.WindowsClipboardService.SetTextAsync(e.Waypoint)
                                .ContinueWith((clipboardResult) => {
                                    if (clipboardResult.IsFaulted) {
                                        ScreenNotification.ShowNotification("Failed to copy waypoint to clipboard. Try again.", ScreenNotification.NotificationType.Red, duration: 2);
                                    }
                                    else {
                                        ScreenNotification.ShowNotification("Copied waypoint to clipboard!", duration: 2);
                                    }
                                });
            };

            return EventButton;
        }

        public void PopulateEvents() {
            _events = new List<Event>();
            _eventGroups = new List<Category>();
            foreach (Daily d in Module._dailies) {
                if (d.Times != null && d.Times.Length > 0) {
                    if (!_eventGroups.Exists(x => x.Name.Equals(d.TimesGroup)))
                        _eventGroups.Add(new Category() { Name = d.TimesGroup, IsActive = false, Set = d.TimesSet });
                    foreach (string s in d.Times) {
                        _events.Add(
                            new Event() {
                                DailyID = d.Id,
                                Name = d.Name,
                                StartTime = DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s),
                                EndTime = DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).AddMinutes(d.TimesDuration),
                                Duration = d.TimesDuration,
                                Group = d.TimesGroup,
                                Button = new Panel(),
                                Color = FindColor(d.TimesColor),
                                Waypoint = d.Waypoint,
                                Daily = new Daily() { IsTracked = d.IsTracked, IsComplete = d.IsComplete, Name = d.Name, Category = d.Category, IsDaily = d.IsDaily },
                            }
                            );
                        _events.Add(
                            new Event() {
                                DailyID = d.Id,
                                Name = d.Name,
                                StartTime = DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).AddDays(1),
                                EndTime = DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).AddDays(1).AddMinutes(d.TimesDuration),
                                Duration = d.TimesDuration,
                                Group = d.TimesGroup,
                                Button = new Panel(),
                                Color = FindColor(d.TimesColor),
                                Waypoint = d.Waypoint,
                                Daily = new Daily() { IsTracked = d.IsTracked, IsComplete = d.IsComplete, Name = d.Name, Category = d.Category, IsDaily = d.IsDaily },
                            }
                            );
                    }
                }
            }
            _events.Sort(delegate (Event x, Event y) {
                if (x.StartTime == null && y.StartTime == null) return 0;
                else if (x.StartTime == null) return -1;
                else if (y.StartTime == null) return 1;
                else return x.StartTime.CompareTo(y.StartTime);
            });
            _eventGroups.Sort(delegate (Category x, Category y) {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });
        }

        public void UpdatePanel() {
            PopulateEvents();

            string timeformat = "h:mm tt";
            if (Module._setting24HrTime.Value) timeformat = "H:mm";

            DateTime panelStartTime = RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15));
            DateTime panelEndTime = RoundDown(DateTime.UtcNow.AddMinutes(-15).AddMinutes((int.Parse(Module._settingEventHours.Value) * 4) * 15), TimeSpan.FromMinutes(15));

            _timePanel.ClearChildren();
            for (int i = 0; i < (int.Parse(Module._settingEventHours.Value) * 4)+1; i++) {
                var t = RoundDown(DateTime.UtcNow.AddMinutes(-15).AddMinutes(i * 15).ToLocalTime(), TimeSpan.FromMinutes(15));
                float w = _minuteWidth * 15;
                float y = _minuteWidth * 15 * i;
                Label timeLabel = new Label() {
                    Size = new Point((int)w, _categoryHeight),
                    Location = new Point(100 + (int)y - (int)(w/2), 0),
                    Parent = _timePanel,
                    Text = t.ToString(timeformat),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Font = Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size12, ContentService.FontStyle.Regular),
                };
                Panel timeIndicator = new Panel() {
                    Parent = _timePanel,
                    Size = new Point(3, 3),
                    Location = new Point(100 + (int)y, _categoryHeight - 3),
                    BackgroundColor = Color.Gray,
                    ZIndex = 11
                };
            }


            _eventPanel.ClearChildren();
            int curY = 0;
            int cnt = 0;
            foreach (Category c in _eventGroups) {
                if (_selectSet.SelectedItem.Equals("All") || _selectSet.SelectedItem.Equals(c.Set)) {
                    c.CategoryPanel = new Panel() {
                        Location = new Point(0, curY),
                        Parent = _eventPanel,
                        Size = new Point(_eventPanel.Size.X - 15, _categoryHeight),
                        BackgroundTexture = (cnt % 2 == 0) ? _btnBackground : _blankBackground,
                        Visible = false,
                    };
                    Label catLabel = new Label() {
                        Parent = c.CategoryPanel,
                        Text = c.Name,
                        Size = new Point(100, _categoryHeight),
                        Location = new Point(5, 0),
                    };

                    int btnCount = 0;
                    foreach (Event e in _events) {
                        if (e.Daily.IsDaily && e.Group.Equals(c.Name) && Module.InSection(e.Daily, _selectedTracked, "", "") ) {
                            btnCount++;
                            if (e.EndTime > panelStartTime && e.StartTime < panelEndTime) {
                                e.Button = CreateButton(c.CategoryPanel, e);
                            }
                        }
                    }

                    if (btnCount > 0) {
                        c.CategoryPanel.Visible = true;
                        curY += _categoryHeight + 2;
                        cnt++;
                    }
                }
            }

            float offset = (DateTime.UtcNow.Hour * 60 * _minuteWidth) + ((RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15)).Minute) * _minuteWidth);
            float curtime = ((DateTime.UtcNow.Hour * 60 * _minuteWidth) + (DateTime.UtcNow.Minute) * _minuteWidth);
            float timeloc = 100 + (curtime - offset) + _eventPanel.Location.X;
            _timeMarker.Location = new Point((int)(timeloc), _eventPanel.Top);
            _eventPanel.RecalculateLayout();
        }

        private static DateTime RoundDown(DateTime dt, TimeSpan d) {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }
        private Color FindColor(string colorname) {
            if (colorname == null) colorname = "Black";
            System.Drawing.Color systemColor = System.Drawing.Color.FromName(colorname);
            return new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
        }

    }
}
