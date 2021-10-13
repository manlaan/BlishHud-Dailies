using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;

namespace Manlaan.Dailies.Views
{
    class SettingsView : View
    {
        protected override void Build(Container buildPanel) {
            int labelMainWidth = 100;
            int labelSubWidth = 60;
            int textboxWidth = 50;

            Panel parentPanel = new Panel() {
                Location = new Point(10, 10),
                CanScroll = false,
                Parent = buildPanel,
                Size = new Point(700, buildPanel.Size.Y),
                HeightSizingMode = SizingMode.AutoSize,
            };

            Label settingMiniWindow_Label = new Label() {
                Location = new Point(0, 2),
                Width = labelMainWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Mini Window: ",
            };
            Label settingMiniWindowWidth_Label = new Label() {
                Location = new Point(settingMiniWindow_Label.Right + 4, settingMiniWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Width: ",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            TextBox settingMiniWindowWidth_Textbox = new TextBox() {
                Location = new Point(settingMiniWindowWidth_Label.Right + 4, settingMiniWindow_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingMiniSizeW.Value,
            };
            settingMiniWindowWidth_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingMiniWindowWidth_Textbox.Text);
                    Module._settingMiniSizeW.Value = settingMiniWindowWidth_Textbox.Text;
                }
                catch {
                    settingMiniWindowWidth_Textbox.Text = Module._settingMiniSizeW.Value;
                }
            };
            Label settingMiniWindowHeight_Label = new Label() {
                Location = new Point(settingMiniWindowWidth_Textbox.Right + 4, settingMiniWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Height: ",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            TextBox settingMiniWindowHeight_Textbox = new TextBox() {
                Location = new Point(settingMiniWindowHeight_Label.Right + 4, settingMiniWindow_Label.Top- 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingMiniSizeH.Value,
            };
            settingMiniWindowHeight_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingMiniWindowHeight_Textbox.Text);
                    Module._settingMiniSizeH.Value = settingMiniWindowHeight_Textbox.Text;
                }
                catch {
                    settingMiniWindowHeight_Textbox.Text = Module._settingMiniSizeH.Value;
                }
            };


            Label settingEventWindow_Label = new Label() {
                Location = new Point(0, settingMiniWindow_Label.Bottom + 8),
                Width = labelMainWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Event Window: ",
            };
            Label settingEventWindowWidth_Label = new Label() {
                Location = new Point(settingEventWindow_Label.Right + 4, settingEventWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Width: ",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            TextBox settingEventWindowWidth_Textbox = new TextBox() {
                Location = new Point(settingEventWindowWidth_Label.Right + 4, settingEventWindow_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingEventSizeW.Value,
            };
            settingEventWindowWidth_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingEventWindowWidth_Textbox.Text);
                    Module._settingEventSizeW.Value = settingEventWindowWidth_Textbox.Text;
                }
                catch {
                    settingEventWindowWidth_Textbox.Text = Module._settingEventSizeW.Value;
                }
            };
            Label settingEventWindowHeight_Label = new Label() {
                Location = new Point(settingEventWindowWidth_Textbox.Right + 4, settingEventWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Height: ",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            TextBox settingEventWindowHeight_Textbox = new TextBox() {
                Location = new Point(settingEventWindowHeight_Label.Right + 4, settingEventWindow_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingEventSizeH.Value,
            };
            settingEventWindowHeight_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingEventWindowHeight_Textbox.Text);
                    Module._settingEventSizeH.Value = settingEventWindowHeight_Textbox.Text;
                }
                catch {
                    settingEventWindowHeight_Textbox.Text = Module._settingEventSizeH.Value;
                }
            };
            Label settingEventWindowHours_Label = new Label() {
                Location = new Point(settingEventWindowHeight_Textbox.Right + 4, settingEventWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Hours: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                BasicTooltipText = "Hours to display",
            };
            TextBox settingEventWindowHours_Textbox = new TextBox() {
                Location = new Point(settingEventWindowHours_Label.Right + 4, settingEventWindow_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingEventHours.Value,
                BasicTooltipText = "Hours to show in Event Window",
            };
            settingEventWindowHours_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingEventWindowHours_Textbox.Text);
                    Module._settingEventHours.Value = settingEventWindowHeight_Textbox.Text;
                }
                catch {
                    settingEventWindowHours_Textbox.Text = Module._settingEventHours.Value;
                }
            };

            Label settingAlertWindow_Label = new Label() {
                Location = new Point(0, settingEventWindow_Label.Bottom + 8),
                Width = labelMainWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Alert Window: ",
            };
            IView settingAlertEnabled_View = SettingView.FromType(Module._settingAlertEnabled, buildPanel.Width);
            ViewContainer settingAlertEnabled_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(labelMainWidth - 10, settingAlertWindow_Label.Top - 3),
                Parent = parentPanel
            };
            settingAlertEnabled_Container.Show(settingAlertEnabled_View);
            Label settingAlertWindowWidth_Label = new Label() {
                Location = new Point(settingAlertWindow_Label.Right + 4, settingAlertWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Width: ",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            TextBox settingAlertWindowWidth_Textbox = new TextBox() {
                Location = new Point(settingAlertWindowWidth_Label.Right + 4, settingAlertWindow_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingAlertSizeW.Value,
            };
            settingAlertWindowWidth_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingAlertWindowWidth_Textbox.Text);
                    Module._settingAlertSizeW.Value = settingAlertWindowWidth_Textbox.Text;
                }
                catch {
                    settingAlertWindowWidth_Textbox.Text = Module._settingAlertSizeW.Value;
                }
            };
            Label settingAlertNotify_Label = new Label() {
                Location = new Point(settingAlertWindowWidth_Textbox.Right + 4, settingAlertWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Notify: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                BasicTooltipText = "Amount of time to notify before\nevent starts, in minutes.\nAlso sets how early dailies are reset.",
            };
            TextBox settingAlertNotify_Textbox = new TextBox() {
                Location = new Point(settingAlertNotify_Label.Right + 4, settingAlertWindow_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingAlertNotify.Value,
                BasicTooltipText = "Amount of time to notify before\nevent starts, in minutes.\nAlso sets how early dailies are reset.",
            };
            settingAlertNotify_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingAlertNotify_Textbox.Text);
                    Module._settingAlertNotify.Value = settingAlertNotify_Textbox.Text;
                }
                catch {
                    settingAlertNotify_Textbox.Text = Module._settingAlertNotify.Value;
                }
            };
            Label settingAlertDuration_Label = new Label() {
                Location = new Point(settingAlertNotify_Textbox.Right + 4, settingAlertWindow_Label.Top),
                Width = labelSubWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Duration: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                BasicTooltipText = "Duration the alert stays visable, in seconds.\n0 shows the alert for the length of event.",
            };
            TextBox settingAlertDuration_Textbox = new TextBox() {
                Location = new Point(settingAlertDuration_Label.Right + 4, settingAlertDuration_Label.Top - 2),
                Width = textboxWidth,
                Parent = parentPanel,
                Text = Module._settingAlertDuration.Value,
                BasicTooltipText = "Duration the alert stays visable, in seconds.\n0 shows the alert for the length of event.",
            };
            settingAlertDuration_Textbox.TextChanged += delegate {
                try {
                    int.Parse(settingAlertDuration_Textbox.Text);
                    Module._settingAlertDuration.Value = settingAlertDuration_Textbox.Text;
                }
                catch {
                    settingAlertDuration_Textbox.Text = Module._settingAlertDuration.Value;
                }
            };
            IView settingAlertDrag_View = SettingView.FromType(Module._settingAlertDrag, buildPanel.Width);
            ViewContainer settingAlertDrag_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(settingAlertDuration_Textbox.Right + 4, settingAlertDuration_Label.Top - 3),
                Parent = parentPanel
            };
            settingAlertDrag_Container.Show(settingAlertDrag_View);


            IView setting24HrTime_View = SettingView.FromType(Module._setting24HrTime, buildPanel.Width);
            ViewContainer setting24HrTime_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(0, settingAlertDrag_Container.Bottom + 6),
                Parent = parentPanel
            };
            setting24HrTime_Container.Show(setting24HrTime_View);

            IView settingDebug_View = SettingView.FromType(Module._settingDebug, buildPanel.Width);
            ViewContainer settingDebug_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(0, setting24HrTime_Container.Bottom + 4),
                Parent = parentPanel
            };
            settingDebug_Container.Show(settingDebug_View);
        }
    }
}
