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
    public class MainWindow : WindowBase2
    {

        #region Load Static

        private static Texture2D _btnBackground, _defaulticon, _resetIcon, _updateIcon;
        private static Texture2D _noteIcon, _wikiIcon, _timerIcon, _copyIcon, _auto1Icon, _auto0Icon, _complete1Icon, _complete0Icon, _wpIcon, _fav1Icon, _fav0Icon;

        static MainWindow() {
            _btnBackground = Module.ModuleInstance.ContentsManager.GetTexture("button.png");
            _defaulticon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\42684.png");
            _wikiIcon = Module.ModuleInstance.ContentsManager.GetTexture("102530.png");
            _timerIcon = Module.ModuleInstance.ContentsManager.GetTexture("496252.png");
            _noteIcon = Module.ModuleInstance.ContentsManager.GetTexture("440023.png");
            _copyIcon = Module.ModuleInstance.ContentsManager.GetTexture("563464.png");
            _complete1Icon = Module.ModuleInstance.ContentsManager.GetTexture("784259.png");
            _complete0Icon = Module.ModuleInstance.ContentsManager.GetTexture("784261.png");
            _wpIcon = Module.ModuleInstance.ContentsManager.GetTexture("156628.png");
            _auto1Icon = Module.ModuleInstance.ContentsManager.GetTexture("155061.png");
            _auto0Icon = Module.ModuleInstance.ContentsManager.GetTexture("156708.png");
            _resetIcon = Module.ModuleInstance.ContentsManager.GetTexture("2256687.png");
            _updateIcon = Module.ModuleInstance.ContentsManager.GetTexture("2208348.png");
            _fav1Icon = Module.ModuleInstance.ContentsManager.GetTexture("605019.png");
            _fav0Icon = Module.ModuleInstance.ContentsManager.GetTexture("605021.png");
        }
        #endregion


        private Point WinSize = new Point();
        private FlowPanel _dailyPanel;
        public Panel _parentPanel;

        private Menu _categoriesMenu;
        public string _dailySearch = "", _dailyShow = "", _dailyCategory = "";
        private Label _dailyCount;

        public MainWindow(Point size) : base() {
            WinSize = size;
            BuildWindow();
        }

        private void BuildWindow() {
            _parentPanel = new Panel() {
                CanScroll = false,
                Size = WinSize,
            };
            TextBox searchBox = new TextBox() {
                PlaceholderText = "Search",
                Width = 150,
                Location = new Point(Dropdown.Standard.ControlOffset.X, 10),
                Parent = _parentPanel
            };
            searchBox.TextChanged += delegate (object sender, EventArgs args) {
                _dailySearch = searchBox.Text;
               UpdateDailyPanel();
            };
            Dropdown ShowItems = new Dropdown() {
                Location = new Point(searchBox.Right + 8, 10),
                Width = 175,
                Parent = _parentPanel,
            };
            ShowItems.Items.Add("Tracked - Incomplete");
            ShowItems.Items.Add("Tracked - Complete");
            ShowItems.Items.Add("Tracked - All Daily");
            ShowItems.Items.Add("Tracked - Not Daily");
            ShowItems.Items.Add("Tracked - All");
            ShowItems.Items.Add("Untracked");
            ShowItems.Items.Add("All");
            ShowItems.SelectedItem = "Tracked - Incomplete";
            _dailyShow = ShowItems.SelectedItem;
            ShowItems.ValueChanged += delegate {
                _dailyShow = ShowItems.SelectedItem;
                UpdateDailyPanel();
            };
            Image updateIcon = new Image(_updateIcon) {
                Size = new Point(ShowItems.Height, ShowItems.Height),
                Location = new Point(ShowItems.Right + 8, 10),
                Parent = _parentPanel,
                BasicTooltipText = "Force API update\nAPI data can be 5+ min behind game data",
            };
            updateIcon.Click += delegate { Module.ModuleInstance.UpdateAchievements(); };
            Image resetIcon = new Image(_resetIcon) {
                Size = new Point(ShowItems.Height, ShowItems.Height),
                Location = new Point(_parentPanel.Width - ShowItems.Height, 10),
                Parent = _parentPanel,
                BasicTooltipText = "Reset all items to incomplete",
            };
            resetIcon.Click += delegate {
                Module._settingLastReset.Value = DateTime.Today.AddDays(-10);
            };
            _dailyCount = new Label() {
                Location = new Point(resetIcon.Left - 110, resetIcon.Top + 7),
                Width = 100,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                //Text = "Count: 8888",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Panel categoryPanel = new Panel() {
                ShowBorder = true,
                Title = "Categories",
                Size = new Point(150, _parentPanel.Size.Y - ShowItems.Height - 8),
                Location = new Point(Panel.MenuStandard.PanelOffset.X, ShowItems.Bottom + 5),
                CanScroll = true,
                Parent = _parentPanel,
            };
            _dailyPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Location = new Point(categoryPanel.Right + Panel.MenuStandard.ControlOffset.X, categoryPanel.Top),
                Size = new Point(_parentPanel.Width - categoryPanel.Right - Control.ControlStandard.ControlOffset.X, _parentPanel.Height - ShowItems.Height - 15 - 25 - 8),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            StandardButton trackAllButton = new StandardButton() {
                Text = "Track All",
                Size = new Point(110, 25),
                Location = new Point(_dailyPanel.Left, _dailyPanel.Bottom + 8),
                Parent = _parentPanel,
            };
            trackAllButton.Click += delegate { SetAllTracked(true); };
            StandardButton untrackAllButton = new StandardButton() {
                Text = "Untrack All",
                Size = trackAllButton.Size,
                Location = new Point(trackAllButton.Right + 8, trackAllButton.Top),
                Parent = _parentPanel,
            };
            untrackAllButton.Click += delegate { SetAllTracked(false); };
            StandardButton uncompleteAllButton = new StandardButton() {
                Text = "Incomplete All",
                Size = trackAllButton.Size,
                Location = new Point(_dailyPanel.Right - 110, trackAllButton.Top),
                Parent = _parentPanel,
            };
            uncompleteAllButton.Click += delegate { SetAllComplete(false); };
            StandardButton completeAllButton = new StandardButton() {
                Text = "Complete All",
                Size = trackAllButton.Size,
                Location = new Point(uncompleteAllButton.Left - trackAllButton.Width - 8, trackAllButton.Top),
                Parent = _parentPanel,
            };
            completeAllButton.Click += delegate { SetAllComplete(true); };

            _categoriesMenu = new Menu {
                Size = categoryPanel.ContentRegion.Size,
                MenuItemHeight = 40,
                Parent = categoryPanel,
                CanSelect = true
            };

            foreach (Daily d in Module._dailies) {
               // d.Button = CreateDailyButton(d);
            }
        }
         

        public DailyDetailsButton CreateDailyButton(Daily d) {
            Point iconSize = new Point(30, 30);

            DailyDetailsButton dailyButton = new DailyDetailsButton() {
                CanScroll = false,
                Size = new Point((int)(_dailyPanel.Width / 3) - 12, 100),
                Parent = _dailyPanel,
            };
            Image buttonbackground = new Image(_btnBackground) {
                Size = dailyButton.Size,
                Parent = dailyButton,
            };

            Texture2D buttonIcon = _defaulticon;
            if (!string.IsNullOrEmpty(d.Icon)) {
                try {
                    if (File.Exists(Module.ModuleInstance.DirectoriesManager.GetFullDirectoryPath("dailies") + "\\" + d.Icon))
                        buttonIcon = Texture2D.FromFile(Graphics.GraphicsDevice, Module.ModuleInstance.DirectoriesManager.GetFullDirectoryPath("dailies") + "\\" + d.Icon);
                    else
                        buttonIcon = Module.ModuleInstance.ContentsManager.GetTexture("icons\\" + d.Icon);
                }
                catch {
                    buttonIcon = _defaulticon;
                }
            }
            Image icon = new Image(buttonIcon) {
                Size = new Point(55,55),
                Parent = dailyButton,
                Location = new Point (5,5),
            };

            Label Category = new Label() {
                Location = new Point(70, 5),
                Width = dailyButton.Size.X - 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = dailyButton,
                Text = d.Category,
                Font = Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Italic),
            };
            Label Desc = new Label() {
                Location = new Point(Category.Left, Category.Bottom),
                Width = Category.Width,
                Height = dailyButton.Size.Y - (iconSize.Y + 4) - Category.Height,
                AutoSizeHeight = false,
                WrapText = true,
                Parent = dailyButton,
                Text = d.Name,
                VerticalAlignment = VerticalAlignment.Top,
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
                    Location = new Point(xloc, dailyButton.Height - iconSize.Y - 2 + 5),
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
                Icon = _fav0Icon,
                ActiveIcon = _fav1Icon,
                BasicTooltipText = "Toggle tracking",
                ToggleGlow = true,
                Checked = d.IsTracked,
                Parent = dailyButton,
                Location = new Point(dailyButton.Size.X - iconSize.X - 5, 0),
                Size = iconSize,
            };

            dailyButton.TrackedButton.Click += delegate {
                Module._dailySettings.SetTracked(d.Id, dailyButton.TrackedButton.Checked);
                d.IsTracked = dailyButton.TrackedButton.Checked;
                d.Button.TrackedButton.Checked = dailyButton.TrackedButton.Checked;
                //d.MiniButton.TrackedButton.Checked = dailyButton.CompleteButton.Checked;
                Module.ModuleInstance.UpdateDailyPanel();
            };

            bool setAutoComplete = false;
            switch (d.API) {
                default:
                    break;
                //Autocompletes
                case "dungeons":
                case "mapchests":
                case "raids":
                case "worldbosses":
                case "dailycrafting":
                    if (!string.IsNullOrEmpty(d.Achievement) && !string.IsNullOrEmpty(d.API) && Module.ModuleInstance.Gw2ApiManager.HavePermissions(new[] { Gw2Sharp.WebApi.V2.Models.TokenPermission.Account, Gw2Sharp.WebApi.V2.Models.TokenPermission.Progression }))
                        setAutoComplete = true;
                    break;
            }

            if (!setAutoComplete) {
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
                    Module._dailySettings.SetComplete(d.Id, dailyButton.CompleteButton.Checked);
                    d.IsComplete = dailyButton.CompleteButton.Checked;
                    d.Button.CompleteButton.Checked = dailyButton.CompleteButton.Checked;
                    d.MiniButton.CompleteButton.Checked = dailyButton.CompleteButton.Checked;
                    Module.ModuleInstance.UpdateDailyPanel();
                };
            }
            else {
                dailyButton.CompleteButton = new GlowButton() {
                    Icon = _auto0Icon,
                    ActiveIcon = _auto1Icon,
                    BasicTooltipText = "Auto",
                    ToggleGlow = true,
                    Checked = d.IsComplete,
                    Parent = dailyButton,
                    Location = new Point(dailyButton.Size.X - iconSize.X - 5, dailyButton.Height - iconSize.Y - 2),
                    Size = iconSize,
                };
                dailyButton.CompleteButton.Click += delegate {
                    dailyButton.CompleteButton.Checked = d.IsComplete;
                };
            }

            return dailyButton;
        }

        public void UpdateDailyPanel() {
            int count = 0;

            foreach (Category cat in Module._categories) {
                cat.IsActive = false;
            }

            foreach (Daily d in Module._dailies) {
                d.Button.Visible = false;
                if (Module.InSection(d, _dailyShow, _dailySearch, "")) {
                    Module._categories.Find(x => x.Name.Equals(d.Category)).IsActive = true;
                }
                if (Module.InSection(d, _dailyShow, _dailySearch, _dailyCategory)) {
                    count++;
                    d.Button.Visible = true;
                }
            }
            _dailyPanel.RecalculateLayout();
            _dailyCount.Text = "Count: " + count.ToString();

            _categoriesMenu.ClearChildren();

            MenuItem categoryItem = _categoriesMenu.AddMenuItem("All");
            if (_dailyCategory.Equals(""))
                categoryItem.Select();
            categoryItem.Click += delegate {
                _dailyCategory = "";
                UpdateDailyPanel();
            };

            foreach (Category cat in Module._categories) {
                if (cat.IsActive) {
                    categoryItem = _categoriesMenu.AddMenuItem(cat.Name);
                    categoryItem.Click += delegate {
                        _dailyCategory = cat.Name;
                        UpdateDailyPanel();
                    };
                    if (_dailyCategory.Equals(cat.Name))
                        categoryItem.Select();
                }
            }
        }

        public void SetAllComplete(bool complete, bool ignoreCategory = false) {
            if (ignoreCategory) {
                foreach (Daily d in Module._dailies) {
                    Module._dailySettings.SetComplete(d.Id, complete);
                    d.IsComplete = complete;
                    d.Button.CompleteButton.Checked = complete;
                    d.MiniButton.CompleteButton.Checked = complete;
                }
            }
            else {
                foreach (Daily d in Module._dailies) {
                    if (Module.InSection(d, _dailyShow, _dailySearch, _dailyCategory)) {
                        Module._dailySettings.SetComplete(d.Id, complete);
                        d.IsComplete = complete;
                        d.Button.CompleteButton.Checked = complete;
                        d.MiniButton.CompleteButton.Checked = complete;
                    }
                }
            }
            Module.ModuleInstance.UpdateDailyPanel();
        }
        public void SetAllTracked(bool tracked) {
            foreach (Daily d in Module._dailies) {
                if (Module.InSection(d, _dailyShow, _dailySearch, _dailyCategory)) {
                    Module._dailySettings.SetTracked(d.Id, tracked);
                    d.IsTracked = tracked;
                    d.Button.TrackedButton.Checked = tracked;
                    //d.MiniButton.TrackedButton.Checked = tracked;
                }
            }
            Module.ModuleInstance.UpdateDailyPanel();
        }

    }
}
