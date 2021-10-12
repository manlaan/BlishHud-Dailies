using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.Dailies.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace Manlaan.Dailies.Controls
{
    class AlertWindow : Panel
    {
        #region Load Static

        private static Texture2D _btnBackground, _defaultIcon, _closeIcon;
        private static Texture2D _noteIcon, _wikiIcon, _timerIcon, _copyIcon, _complete1Icon, _complete0Icon, _auto1Icon, _auto0Icon, _wpIcon;

        static AlertWindow() {
            _btnBackground = Module.ModuleInstance.ContentsManager.GetTexture("button.png");
            _defaultIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\42684.png");
            _wikiIcon = Module.ModuleInstance.ContentsManager.GetTexture("102530.png");
            _timerIcon = Module.ModuleInstance.ContentsManager.GetTexture("496252.png");
            _noteIcon = Module.ModuleInstance.ContentsManager.GetTexture("440023.png");
            _copyIcon = Module.ModuleInstance.ContentsManager.GetTexture("563464.png");
            _complete1Icon = Module.ModuleInstance.ContentsManager.GetTexture("784259.png");
            _complete0Icon = Module.ModuleInstance.ContentsManager.GetTexture("784261.png");
            _wpIcon = Module.ModuleInstance.ContentsManager.GetTexture("156628.png");
            _closeIcon = Module.ModuleInstance.ContentsManager.GetTexture("button-exit.png");
            _auto1Icon = Module.ModuleInstance.ContentsManager.GetTexture("155061.png");
            _auto0Icon = Module.ModuleInstance.ContentsManager.GetTexture("156708.png");
        }
        #endregion


        private Panel _dragBox = new Panel();
        private FlowPanel _alertPanel;
        private List<Alert> _alerts = new List<Alert>();
        private bool _dragging = false;
        private Point _dragStart = Point.Zero;

        public AlertWindow(Point size) : base() {
            Size = size;
            BuildWindow();
        }

        private void BuildWindow() {
            _alertPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Location = new Point(0, 0),
                Size = this.Size,
                CanScroll = true,
                Parent = this,
                ShowBorder = false,
            };

            _dragBox = new Panel() {
                Parent = this,
                Location = new Point(0, 0),
                Size = new Point(20, 20),
                BackgroundColor = Color.White,
                ZIndex = 10,
                Visible = false,
            };
            _dragBox.LeftMouseButtonPressed += delegate {
                _dragging = true;
                _dragStart = InputService.Input.Mouse.Position;
            };
            _dragBox.LeftMouseButtonReleased += delegate {
                _dragging = false;
                Module._settingAlertLocation.Value = this.Location;
            };
        }

        public DailyDetailsButton CreateButton(Daily d, string start) {
            Point iconSize = new Point(26, 26);

            DailyDetailsButton dailyButton = new DailyDetailsButton() {
                CanScroll = false,
                Size = new Point(_alertPanel.Size.X - 20, 73),
                Parent = _alertPanel,
            };

            Image buttonbackground = new Image(_btnBackground) {
                Size = dailyButton.Size,
                Parent = dailyButton,
            };

            Texture2D buttonIcon = _defaultIcon;
            if (!string.IsNullOrEmpty(d.Icon)) {
                try {
                    if (File.Exists(Module.ModuleInstance.DirectoriesManager.GetFullDirectoryPath("dailies") + "\\" + d.Icon))
                        buttonIcon = Texture2D.FromFile(Graphics.GraphicsDevice, Module.ModuleInstance.DirectoriesManager.GetFullDirectoryPath("dailies") + "\\" + d.Icon);
                    else
                        buttonIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\" + d.Icon);
                }
                catch {
                    buttonIcon = _defaultIcon;
                }
            }
            Image icon = new Image(buttonIcon) {
                Size = new Point(35, 35),
                Parent = dailyButton,
                Location = new Point(5, 5),
                BasicTooltipText = d.Name,
            };

            Label Category = new Label() {
                Location = new Point(48, 2),
                Width = dailyButton.Size.X - 15 - 42,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = dailyButton,
                Text = d.Category,
                Font = Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size12, ContentService.FontStyle.Italic),
            };
            Label Desc = new Label() {
                Location = new Point(Category.Left, Category.Bottom - 1),
                Width = Category.Width,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = dailyButton,
                Text = d.Name,
            };

            Image buttonbackground2 = new Image(_btnBackground) {
                Size = new Point(dailyButton.Size.X, iconSize.Y + 4),
                Parent = dailyButton,
                Location = new Point(0, dailyButton.Height - iconSize.Y - 4),
            };

            int xloc = 5;
            if (!string.IsNullOrEmpty(d.Note) || Module._settingDebug.Value) {
                GlowButton NoteBtn = new GlowButton {
                    Icon = _noteIcon,
                    BasicTooltipText = d.Note + (Module._settingDebug.Value ? "\n\n" + d.Achievement + " - " + d.API + " - " + d.Id : ""),
                    Parent = dailyButton,
                    GlowColor = Color.White * 0.1f,
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2),
                    Size = iconSize,
                };
                xloc = NoteBtn.Right + 5;
            }
            if (!string.IsNullOrEmpty(d.Wiki)) {
                GlowButton WikiBtn = new GlowButton {
                    Icon = _wikiIcon,
                    BasicTooltipText = "Read about this on the wiki",
                    Parent = dailyButton,
                    GlowColor = Color.White * 0.1f,
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2),
                    Size = iconSize,
                };
                WikiBtn.Click += delegate {
                    if (Module.UrlIsValid(d.Wiki)) {
                        Process.Start(d.Wiki);
                    }
                };
                xloc = WikiBtn.Right + 5;
            }
            if (!string.IsNullOrEmpty(d.Timer)) {
                GlowButton TimeBtn = new GlowButton {
                    Icon = _timerIcon,
                    BasicTooltipText = "See timer on Wiki",
                    Parent = dailyButton,
                    GlowColor = Color.White * 0.1f,
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2),
                    Size = iconSize,
                };
                TimeBtn.Click += delegate {
                    if (Module.UrlIsValid(d.Timer)) {
                        Process.Start(d.Timer);
                    }
                };
                xloc = TimeBtn.Right + 5;
            }
            if (!string.IsNullOrEmpty(d.Waypoint)) {
                GlowButton WaypointBtn = new GlowButton {
                    Icon = _wpIcon,
                    BasicTooltipText = "Copy waypoint to clipboard",
                    Parent = dailyButton,
                    GlowColor = Color.White * 0.1f,
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2),
                    Size = iconSize,
                };
                WaypointBtn.Click += delegate {
                    ClipboardUtil.WindowsClipboardService.SetTextAsync(d.Waypoint)
                                    .ContinueWith((clipboardResult) => {
                                        if (clipboardResult.IsFaulted) {
                                            ScreenNotification.ShowNotification("Failed to copy waypoint to clipboard. Try again.", ScreenNotification.NotificationType.Red, duration: 2);
                                        }
                                        else {
                                            ScreenNotification.ShowNotification("Copied waypoint to clipboard!", duration: 2);
                                        }
                                    });
                };
                xloc = WaypointBtn.Right + 5;
            }
            if (d.Times != null && d.Times.Length > 0) {
                dailyButton.TimeButton = new Label() {
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2 + 3),
                    Width = 75,
                    AutoSizeHeight = false,
                    WrapText = false,
                    Parent = dailyButton,
                    Text = "",
                    Font = Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
                };
                xloc = dailyButton.TimeButton.Right + 5;
            }
            if (!string.IsNullOrEmpty(d.Clipboard)) {
                GlowButton CopyBtn = new GlowButton {
                    Icon = _copyIcon,
                    BasicTooltipText = "Copy text to clipboard",
                    Parent = dailyButton,
                    GlowColor = Color.White * 0.1f,
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2),
                    Size = iconSize,
                };
                CopyBtn.Click += delegate {
                    ClipboardUtil.WindowsClipboardService.SetTextAsync(d.Clipboard)
                                    .ContinueWith((clipboardResult) => {
                                        if (clipboardResult.IsFaulted) {
                                            ScreenNotification.ShowNotification("Failed to copy to clipboard. Try again.", ScreenNotification.NotificationType.Red, duration: 2);
                                        }
                                        else {
                                            ScreenNotification.ShowNotification("Copied to clipboard!", duration: 2);
                                        }
                                    });
                };
                xloc = CopyBtn.Right + 5;
            }

            dailyButton.TrackedButton = new GlowButton() {
                Icon = _closeIcon,
                ActiveIcon = _closeIcon,
                BasicTooltipText = "Close Alert",
                ToggleGlow = true,
                Checked = false,
                Parent = dailyButton,
                Location = new Point(dailyButton.Size.X - iconSize.X - 5, 0),
                Size = iconSize,
            };
            dailyButton.TrackedButton.Click += delegate {
                Alert alert = _alerts.Find(x => x.ID.Equals(d.Id + "-" + start));
                alert.IsActive = false;
                alert.Button.Hide();
                _alertPanel.RecalculateLayout();
            };

            dailyButton.CompleteButton = new GlowButton() {
                Icon = _complete0Icon,
                ActiveIcon = _complete1Icon,
                BasicTooltipText = "Toggle complete",
                ToggleGlow = true,
                Checked = d.IsComplete,
                Parent = dailyButton,
                Location = new Point(dailyButton.Size.X - iconSize.X - 5, dailyButton.Height - iconSize.Y - 2),
                Size = iconSize,
            };
            dailyButton.CompleteButton.Click += delegate {
                if (d.Button.CompleteButton.BasicTooltipText.Equals("Auto")) {
                    dailyButton.CompleteButton.Checked = d.IsComplete;
                }
                else {
                    Module._dailySettings.SetComplete(d.Id, dailyButton.CompleteButton.Checked);
                    d.IsComplete = dailyButton.CompleteButton.Checked;
                    d.Button.CompleteButton.Checked = dailyButton.CompleteButton.Checked;
                    d.MiniButton.CompleteButton.Checked = dailyButton.CompleteButton.Checked;
                    if (dailyButton.CompleteButton.Checked) {
                        Alert alert = _alerts.Find(x => x.ID.Equals(d.Id + "-" + start));
                        alert.IsActive = false;
                        alert.Button.Hide();
                        _alertPanel.RecalculateLayout();
                        d.MiniButton.Visible = false;
                    }
                    Module.ModuleInstance.UpdateDailyPanel();
                }
            };

            return dailyButton;
        }

        public void AddAlert(Daily d, string start) {
            _alerts.Add(new Alert() {
                ID = d.Id + "-" + start,
                DailyID = d.Id,
                Button = CreateButton(d, start),
                IsActive = true,
                StartTime = DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + start),
                EndTime = DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + start).AddMinutes(d.TimesDuration),
                Duration = d.TimesDuration,
            });
        }

        public void UpdatePanel() {
            foreach (Daily d in Module._dailies) {
                if (d.Times != null && d.Times.Length > 0) {
                    foreach (string s in d.Times) {
                        Alert alert = _alerts.Find(x => x.ID.Equals(d.Id + "-" + s));
                        if (DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).AddMinutes(-int.Parse(Module._settingAlertNotify.Value)) < DateTime.UtcNow &&
                                    DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).AddMinutes(d.TimesDuration) > DateTime.UtcNow &&
                                    d.IsDaily && d.IsTracked) {
                            if (alert == null) {
                                AddAlert(d, s);
                                alert = _alerts.Find(x => x.ID.Equals(d.Id + "-" + s));
                            }
                            if (!alert.IsActive || d.IsComplete) {
                                alert.IsActive = false;
                                alert.Button.Hide();
                            }
                            if (!string.IsNullOrEmpty(d.Achievement) &&
                                        !string.IsNullOrEmpty(d.API) &&
                                        Module._autoCompleteAchievements.Contains(d.API) &&
                                        Module.ModuleInstance.Gw2ApiManager.HavePermissions(new[] { Gw2Sharp.WebApi.V2.Models.TokenPermission.Account, Gw2Sharp.WebApi.V2.Models.TokenPermission.Progression })) {
                                if (!alert.Button.CompleteButton.BasicTooltipText.Equals("Auto")) {
                                    alert.Button.CompleteButton.Icon = _auto0Icon;
                                    alert.Button.CompleteButton.ActiveIcon = _auto1Icon;
                                    alert.Button.CompleteButton.BasicTooltipText = "Auto";
                                }
                            }
                            else {
                                if (alert.Button.CompleteButton.BasicTooltipText.Equals("Auto")) {
                                    alert.Button.CompleteButton.Icon = _complete0Icon;
                                    alert.Button.CompleteButton.ActiveIcon = _complete1Icon;
                                    alert.Button.CompleteButton.BasicTooltipText = "Toggle complete";
                                }
                            }
                        }
                        else {
                            if (alert != null) {
                                alert.Button.Dispose();
                                _alerts.Remove(new Alert() { ID = d.Id + "-" + s });
                            }
                        }
                    }
                }
            }
            _alertPanel.RecalculateLayout();

            if (Module._settingAlertDrag.Value) {
                _dragBox.Visible = true;
                this.BackgroundTexture = _btnBackground;
            } else {
                _dragBox.Visible = false;
                this.BackgroundTexture = null;
            }
        }

        public override void UpdateContainer(GameTime gameTime) {
            if (_dragging) {
                var nOffset = Input.Mouse.Position - _dragStart;
                Location += nOffset;

                _dragStart = Input.Mouse.Position;
            }
        }
    }
}
