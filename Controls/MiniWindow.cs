using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.Dailies.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Manlaan.Dailies.Controls
{
    public class MiniWindow : WindowBase2
    {

        #region Load Static

        private static Texture2D _blankBackground, _wndBackground, _btnBackground, _pageIcon, _defaultIcon;
        private static Texture2D _noteIcon, _wikiIcon, _timerIcon, _copyIcon, _auto1Icon, _auto0Icon, _complete1Icon, _complete0Icon, _wpIcon;

        static MiniWindow() {
            _blankBackground = Module.ModuleInstance.ContentsManager.GetTexture("blank.png");
            _wndBackground = Module.ModuleInstance.ContentsManager.GetTexture("1863949.png");
            _btnBackground = Module.ModuleInstance.ContentsManager.GetTexture("button.png");
            _pageIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\42684.png");
            _defaultIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\42684.png");
            _wikiIcon = Module.ModuleInstance.ContentsManager.GetTexture("102530.png");
            _timerIcon = Module.ModuleInstance.ContentsManager.GetTexture("496252.png");
            _noteIcon = Module.ModuleInstance.ContentsManager.GetTexture("440023.png");
            _copyIcon = Module.ModuleInstance.ContentsManager.GetTexture("563464.png");
            _complete1Icon = Module.ModuleInstance.ContentsManager.GetTexture("784259.png");
            _complete0Icon = Module.ModuleInstance.ContentsManager.GetTexture("784261.png");
            _wpIcon = Module.ModuleInstance.ContentsManager.GetTexture("156628.png");
            _auto1Icon = Module.ModuleInstance.ContentsManager.GetTexture("155061.png");
            _auto0Icon = Module.ModuleInstance.ContentsManager.GetTexture("156708.png");
        }
        #endregion


        private Dropdown _selectCategory;
        private Point WinSize = new Point();
        private FlowPanel _dailyPanel;
        private string _dailyCategory = "";
        private Panel _parentPanel;
        private bool _running = false;

        public MiniWindow(Point size) : base() {
            WinSize = size;
            this.CanClose = false;
            this.Title = "Dailies";
            this.Emblem = _pageIcon;
            this.CanResize = false;
            BuildWindow();
        }

        private void BuildWindow() {
            ConstructWindow(_blankBackground, new Rectangle(0, 0, WinSize.X, WinSize.Y+6), new Rectangle(0, 0, WinSize.X, WinSize.Y+6));
            _parentPanel = new Panel() {
                CanScroll = false,
                Size = new Point(WinSize.X, WinSize.Y),
                Location = new Point(0, 6),
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
            _selectCategory.ValueChanged += SelectCategoryChanged;

            _dailyPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Location = new Point(15, _selectCategory.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 20, _parentPanel.Size.Y - _selectCategory.Bottom - 15),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            foreach (Daily d in Module._dailies) {
                if (d.IsTracked)
                    d.MiniButton = CreateButton(d);
            }

            _dailyCategory = "";
        }

        public DailyDetailsButton CreateButton(Daily d) {
            Point iconSize = new Point(26, 26);

            DailyDetailsButton dailyButton = new DailyDetailsButton() {
                CanScroll = false,
                Size = new Point(_dailyPanel.Size.X - 20, 73),
                Parent = _dailyPanel,
                BackgroundTexture = _btnBackground,
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
                BasicTooltipText = d.Name,
            };
            Label Desc = new Label() {
                Location = new Point(Category.Left, Category.Bottom - 1),
                Width = Category.Width,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = dailyButton,
                Text = d.Name,
                BasicTooltipText = d.Name,
            };

            Image buttonbackground = new Image(_btnBackground) {
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
                        d.MiniButton.Visible = false;
                        _dailyPanel.RecalculateLayout();
                    }
                    Module.ModuleInstance.UpdateDailyPanel();
                }
            };

            return dailyButton;
        }

        public void UpdatePanel() {
            if (_running) return;

            _running = true;

            List<Category> categories = Module._categories;
            foreach (Category cat in categories) {
                cat.IsActive = false;
            }

            foreach (Daily d in Module._dailies) {
                if (Module.InSection(d, "Tracked - Incomplete", "", "")) {
                    if (d.MiniButton.Parent == null) {
                        d.MiniButton = CreateButton(d);
                        d.MiniButton.Visible = false;
                    }
                    categories.Find(x => x.Name.Equals(d.Category)).IsActive = true;
                }
                if (Module.InSection(d, "Tracked - Incomplete", "", _dailyCategory)) {
                    if (!d.MiniButton.Visible)
                        d.MiniButton.Visible = true;
                }
                else {
                    if (d.MiniButton.Visible)
                        d.MiniButton.Visible = false;
                }

                if (d.MiniButton.Parent != null) {
                    if (!string.IsNullOrEmpty(d.Achievement) &&
                                !string.IsNullOrEmpty(d.API) &&
                                Module._autoCompleteAchievements.Contains(d.API) && 
                                Module.ModuleInstance.Gw2ApiManager.HavePermissions(new[] { Gw2Sharp.WebApi.V2.Models.TokenPermission.Account, Gw2Sharp.WebApi.V2.Models.TokenPermission.Progression })) {
                        if (!d.MiniButton.CompleteButton.BasicTooltipText.Equals("Auto")) {
                            d.MiniButton.CompleteButton.Icon = _auto0Icon;
                            d.MiniButton.CompleteButton.ActiveIcon = _auto1Icon;
                            d.MiniButton.CompleteButton.BasicTooltipText = "Auto";
                        }
                    }
                    else {
                        if (d.MiniButton.CompleteButton.BasicTooltipText.Equals("Auto")) {
                            d.MiniButton.CompleteButton.Icon = _complete0Icon;
                            d.MiniButton.CompleteButton.ActiveIcon = _complete1Icon;
                            d.MiniButton.CompleteButton.BasicTooltipText = "Toggle complete";
                        }
                    }
                }
            }
            _dailyPanel.RecalculateLayout();

            _selectCategory.ValueChanged -= SelectCategoryChanged;
            _selectCategory.Items.Clear();
            _selectCategory.Items.Add("All");
            foreach (Category cat in categories) {
                if (cat.IsActive)
                    _selectCategory.Items.Add(cat.Name);
            }
            _selectCategory.SelectedItem = (_dailyCategory.Equals("") ? "All" : _dailyCategory);
            _selectCategory.ValueChanged += SelectCategoryChanged;

            _running = false;
        }

        private void SelectCategoryChanged(object sender = null, ValueChangedEventArgs e = null) {
            _dailyCategory = (e.CurrentValue.Equals("All") ? "" : e.CurrentValue);
            UpdatePanel();
        }
    }
}
