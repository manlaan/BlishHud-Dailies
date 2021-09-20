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
    public class MiniWindow : WindowBase
    {

        #region Load Static

        private static Texture2D _wndBackground, _btnBackground, _pageIcon, _defaulticon;
        private static Texture2D _noteIcon, _wikiIcon, _timerIcon, _copyIcon, _auto1Icon, _auto0Icon, _complete1Icon, _complete0Icon, _wpIcon;

        static MiniWindow() {
            _wndBackground = Module.ModuleInstance.ContentsManager.GetTexture("155985.png");
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
            _pageIcon = Module.ModuleInstance.ContentsManager.GetTexture("42684bw.png");
        }
        #endregion


        private Dropdown _selectCategory;
        private Point WinSize = new Point();
        private FlowPanel _dailyPanel;
        private string _dailyCategory = "";
        private Panel _parentPanel;

        public MiniWindow(Point size) : base() {
            WinSize = size;
            BuildWindow();
        }

        private void BuildWindow() {
            ConstructWindow(
                _wndBackground,
                new Vector2(0, 0),
                new Rectangle(0, 0, WinSize.X, WinSize.Y),
                Thickness.Zero,
                45, false);
            this.ContentRegion = new Rectangle(0, 0, WinSize.X, WinSize.Y);
            _parentPanel = new Panel() {
                CanScroll = false,
                Size = new Point(this.ContentRegion.Size.X, this.ContentRegion.Size.Y - 10),
                Location = new Point(10, 10),
                Parent = this,
            };
            Image bgimage = new Image(_wndBackground) {
                Location = new Point(0, 0),
                Size = _parentPanel.Size,
                Parent = _parentPanel,
            };
            Image headimage = new Image(_pageIcon) {
                Location = new Point(10, 11),
                Size = new Point(23, 23),
                Parent = _parentPanel,
            };
            Label header = new Label() {
                Location = new Point(40, 11),
                Width = _parentPanel.Width - 80,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Dailies",
                HorizontalAlignment = HorizontalAlignment.Left,
                Font = Content.DefaultFont18,
            };

            _selectCategory = new Dropdown() {
                Location = new Point(10, 35),
                Width = _parentPanel.Width - 30,
                Parent = _parentPanel,
            };
            _selectCategory.ValueChanged += delegate {
                _dailyCategory = (_selectCategory.SelectedItem.Equals("All") ? "" : _selectCategory.SelectedItem);
                UpdateDailyPanel();
            };

            _dailyPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Location = new Point(10, _selectCategory.Bottom + 5),
                Size = new Point(_parentPanel.Size.X - 20, _parentPanel.Size.Y - _selectCategory.Bottom - 25),
                CanScroll = true,
                Parent = _parentPanel,
                ShowBorder = false,
            };

            foreach (Daily d in Module._dailies) {
                d.MiniButton = CreateDailyButton(d);
            }

            _dailyCategory = "";
            UpdateDailyPanel();
        }

        public DailyDetailsButton CreateDailyButton(Daily d) {
            Point iconSize = new Point(26, 26);

            DailyDetailsButton dailyButton = new DailyDetailsButton() {
                CanScroll = false,
                Size = new Point(_dailyPanel.Size.X - 20, 73),
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
            if (!string.IsNullOrEmpty(d.Note)) {
                GlowButton NoteBtn = new GlowButton {
                    Icon = _noteIcon,
                    BasicTooltipText = d.Note,
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
                    //Daily daily = Module._combinedDailies.Find(x => x.Id.Equals(d.Id));
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
            List<Category> categories = Module._categories;
            foreach (Category cat in categories) {
                cat.IsActive = false;
            }

            foreach (Daily d in Module._dailies) {
                d.MiniButton.Visible = false;
                if (Module.InSection(d, "Tracked - Incomplete", "", "")) {
                    categories.Find(x => x.Name.Equals(d.Category)).IsActive = true;
                }
                if (Module.InSection(d, "Tracked - Incomplete", "", _dailyCategory)) {
                    d.MiniButton.Visible = true;
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


        /// <inheritdoc />
        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds) {
        }

    }
}
