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


        private Dropdown _selectGroup, _selectTracked, _selectSet;
        private Point WinSize = new Point();
        private Panel _dailyPanel;
        private string _selectedTracked = "";
        private string _selectedGroup = "";
        private string _selectedSet = "";
        private Panel _parentPanel;
        private int _categoryHeight = 25;
        private float _minWidth = 0;
        private Panel _timeMarker;
        private Panel _timePanel;
        private DateTime _prevTime = new DateTime();
        private List<string> _eventSets = new List<string>();


        public EventWindow(Point size) : base() {
            foreach (Category cat in Module._eventGroups) {
                if (!_eventSets.Contains(cat.Set))
                    _eventSets.Add(cat.Set);
            }
            _eventSets.Sort();

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
                Width = ((_parentPanel.Width - 50) / 3 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
                Parent = _parentPanel,
            };
            _selectSet.Items.Add("All");
            foreach (string s in _eventSets) {
                _selectSet.Items.Add(s);
            }
            _selectSet.ValueChanged += delegate {
                _selectedSet = (_selectSet.SelectedItem.Equals("All") ? "" : _selectSet.SelectedItem);
                UpdateDailyPanel();
            };
            _selectedSet = "";

            _selectGroup = new Dropdown() {
                Location = new Point(_selectSet.Right + 10, _selectSet.Top),
                Width = ((_parentPanel.Width - 50) / 3 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
                Parent = _parentPanel,
            };
            _selectGroup.ValueChanged += delegate {
                _selectedGroup = (_selectGroup.SelectedItem.Equals("All") ? "" : _selectGroup.SelectedItem);
                UpdateDailyPanel();
            };
            _selectedGroup = "";

            _selectTracked = new Dropdown() {
                Location = new Point(_selectGroup.Right + 10, _selectGroup.Top),
                Width = ((_parentPanel.Width - 50) / 3 > 170 ? 170 : (_parentPanel.Width - 50) / 2),
                Parent = _parentPanel,
            };
            _selectTracked.Items.Add("Tracked - Incomplete");
            _selectTracked.Items.Add("Tracked - All");
            _selectTracked.Items.Add("All");
            _selectTracked.SelectedItem = "Tracked - Incomplete";
            _selectTracked.ValueChanged += delegate {
                _selectedTracked = (_selectTracked.SelectedItem.Equals("All") ? "" : _selectTracked.SelectedItem);
                _selectGroup.SelectedItem = "All";
                _selectedGroup = "";
                UpdateDailyPanel();
            };
            _selectedTracked = "Tracked - Incomplete";

            _minWidth = (float)((_parentPanel.Size.X - 40 - 100 - 15) / (float)(int.Parse(Module._settingEventHours.Value) * 60));


            _timePanel = new Panel() {
                Location = new Point(15, _selectGroup.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 40, _categoryHeight),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            _dailyPanel = new Panel() {
                Location = new Point(15, _timePanel.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 40, _parentPanel.Size.Y - _timePanel.Bottom - 15),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            //UpdateTimes();
            //UpdateDailyPanel();
        }

        public Panel CreateDailyButton(Panel panel, Event e) {
            float offset = (DateTime.UtcNow.Hour * 60 * _minWidth) + ((RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15)).Minute) * _minWidth);
            float buttonwidth = (e.Duration) * _minWidth;
            float buttonstart = ((e.StartTime.Date-DateTime.UtcNow.Date).Days * 1440 * _minWidth) + (e.StartTime.Hour * 60 * _minWidth) + ((e.StartTime.Minute) * _minWidth) + 100 - offset;

            if ((buttonstart + buttonwidth) > 100 && buttonstart < WinSize.X) {
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
            foreach (Category cat in Module._eventGroups) {
                cat.IsActive = false;
            }
            foreach (Event e in Module._events) {
                Daily d = Module._dailies.Find(x => x.Id.Equals(e.DailyID));
                if (Module.InSection(d, _selectedTracked, "", "")) {
                    Module._eventGroups.Find(x => x.Name.Equals(d.TimesGroup)).IsActive = true;
                }
            }


            DateTime newTime = RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15));
            if (newTime != _prevTime) {
                _prevTime = newTime;

                string timeformat = "h:mm tt";
                if (Module._setting24HrTime.Value) timeformat = "H:mm";

                _timePanel.ClearChildren();
                for (int i = 0; i < (int.Parse(Module._settingEventHours.Value) * 4); i++) {
                    var t = RoundDown(DateTime.UtcNow.AddMinutes(-15).AddMinutes(i * 15).ToLocalTime(), TimeSpan.FromMinutes(15));
                    float w = _minWidth * 15;
                    float y = _minWidth * 15 * i;
                    Label timeLabel = new Label() {
                        Size = new Point((int)w, _categoryHeight),
                        Location = new Point(100 + (int)y, 0),
                        Parent = _timePanel,
                        Text = t.ToString(timeformat),
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                }


                _dailyPanel.ClearChildren();
                int curY = 0;
                int cnt = 0;
                foreach (Category c in Module._eventGroups) {
                    if (c.IsActive && (_selectedGroup.Equals("") || _selectedGroup.Equals(c.Name)) && (_selectedSet.Equals("") || _selectedSet.Equals(c.Set))) {
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
                            Location = new Point(5, 0),
                        };
                        foreach (Event e in Module._events) {
                            if (e.Group.Equals(c.Name))
                                e.Button = CreateDailyButton(catPanel, e);
                        }

                        c.CategoryPanel = catPanel;
                        curY += _categoryHeight + 2;
                        cnt++;
                    }
                }

                _timeMarker = new Panel() {
                    Parent = _dailyPanel,
                    Size = new Point(3, _dailyPanel.Height),
                    BackgroundColor = Color.White,
                    Location = new Point(100, 0),
                    ZIndex = 10
                };
            }

            float offset = (DateTime.UtcNow.Hour * 60 * _minWidth) + ((RoundDown(DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromMinutes(15)).Minute) * _minWidth);
            float curtime = ((DateTime.UtcNow.Hour * 60 * _minWidth) + (DateTime.UtcNow.Minute) * _minWidth);
            float timeloc = 100 + (curtime - offset);
            _timeMarker.Location = new Point((int)(timeloc), 0);



            foreach (Event e in Module._events) {
                Daily d = Module._dailies.Find(x => x.Id.Equals(e.DailyID));
                e.Button.Visible = false;
                if (Module.InSection(d, _selectedTracked, "", "")) {
                    if ((e.Group.Equals(_selectedGroup) || _selectedGroup.Equals("")) && d.IsDaily) {
                        e.Button.Visible = true;
                    }
                }
            }

            _dailyPanel.RecalculateLayout();

            _selectGroup.Items.Clear();
            _selectGroup.Items.Add("All");
            foreach (Category cat in Module._eventGroups) {
                if (cat.IsActive) {
                    _selectGroup.Items.Add(cat.Name);
                }
            }
            _selectGroup.SelectedItem = (_selectedGroup.Equals("") ? "All" : _selectedGroup);
            _prevTime = new DateTime();
        }

        private static DateTime RoundDown(DateTime dt, TimeSpan d) {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }
    }
}
