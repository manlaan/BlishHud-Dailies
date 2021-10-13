using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls;
using static Blish_HUD.GameService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Manlaan.Dailies.Models;
using Manlaan.Dailies.Controls;
using System.Net;
using Blish_HUD.Gw2WebApi;
using Blish_HUD.Graphics.UI;

namespace Manlaan.Dailies
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        internal static Module ModuleInstance;

        public static List<Daily> _dailies = new List<Daily>();
        public static List<Daily> _newdailies = new List<Daily>();
        public static List<Category> _categories = new List<Category>();

        public static SettingEntry<DateTime> _settingLastReset;
        //private SettingEntry<string> _settingFestivalStart;
        public static SettingEntry<Point> _settingMiniLocation;
        public static SettingEntry<Point> _settingEventLocation;
        public static SettingEntry<Point> _settingAlertLocation;
        public static SettingEntry<string> _settingMiniSizeW, _settingMiniSizeH;
        public static SettingEntry<string> _settingEventSizeW, _settingEventSizeH;
        public static SettingEntry<string> _settingAlertNotify;
        public static SettingEntry<string> _settingAlertDuration;
        public static SettingEntry<string> _settingAlertSizeW;
        public static SettingEntry<bool> _settingAlertEnabled;
        public static SettingEntry<bool> _settingAlertDrag;
        public static SettingEntry<string> _settingEventHours;
        public static SettingEntry<bool> _setting24HrTime;
        public static SettingEntry<bool> _settingDontShowIntro;
        public static SettingEntry<bool> _settingDebug;
        public static int _miniSizeW, _miniSizeH, _eventSizeW, _eventSizeH, _eventHours;
        private Stopwatch Timer_AchieveUpdate = new Stopwatch();
        private Stopwatch Timer_TimesUpdate = new Stopwatch();
        private Stopwatch Timer_APIUpdate = new Stopwatch();
        public static DailySettings _dailySettings;
        private Texture2D _pageIcon, _eventIcon;
        private bool _runningAchieve = false;
        private bool _runningTimes = false;

        private MainWindow _mainWindow;
        private MiniWindow _miniWindow;
        private CornerIcon _cornerIcon;
        private EventWindow _eventWindow;
        private CornerIcon _cornerEventIcon;
        private IntroWindow _introWindow;
        private AlertWindow _alertWindow;

        private WindowTab _moduleTab;

        public static string[] _autoCompleteAchievements = new string[] { "dungeons", "mapchests", "raids", "worldbosses", "dailycrafting" };


        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion
        internal static Module Instance { get; private set; }

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        #region Settings
        protected override void DefineSettings(SettingCollection settings) {
            //_settingFestivalStart = settings.DefineSetting("DailyFestivalStart", @"01/01/2000", "Festival Start Date", "");
            _setting24HrTime = settings.DefineSetting("Daily24HrTime", false, "24 Hour Time", "");
            _settingDebug = settings.DefineSetting("DailyDebug", false, "Debug (restart required)", "");
            _settingLastReset = settings.DefineSetting("DailyLastUpdate", new DateTime());
            _settingMiniLocation = settings.DefineSetting("DailyMiniLoc", new Point(100, 100));
            _settingEventLocation = settings.DefineSetting("DailyEventLoc", new Point(100, 100));
            _settingEventHours = settings.DefineSetting("DailyEventHours", "2", "Event Hours", "");
            _settingMiniSizeW = settings.DefineSetting("DailyMiniSizeH", @"265", "Mini Window Width", "");
            _settingMiniSizeH = settings.DefineSetting("DailyMiniSizeW", @"450", "Mini Window Height", "");
            _settingEventSizeW = settings.DefineSetting("DailyEventSizeH", @"600", "Event Window Width", "");
            _settingEventSizeH = settings.DefineSetting("DailyEventSizeW", @"300", "Event Window Height", "");
            _settingDontShowIntro = settings.DefineSetting("DailyDontShowIntro", false, "Don't Show Intro", "");
            _settingAlertSizeW = settings.DefineSetting("DailyAlertSizeW", @"250", "Alert Width", "");
            _settingAlertNotify = settings.DefineSetting("DailyAlertNotify", @"10", "Alert Notify (min)", "");
            _settingAlertDuration = settings.DefineSetting("DailyAlertDuration", @"0", "Alert Duration (sec) - 0 for Event Duration", "");
            _settingAlertLocation = settings.DefineSetting("DailyAlertLoc", new Point(100, 100));
            _settingAlertDrag = settings.DefineSetting("DailyAlertDrag", false, "Alert Draging (White Box)", "");
            _settingAlertEnabled = settings.DefineSetting("DailyAlertEnabled", false, " ", "");

            _setting24HrTime.SettingChanged += UpdateSettings_bool;
            _settingDontShowIntro.SettingChanged += UpdateSettings_bool;
            _settingMiniSizeH.SettingChanged += UpdateSettings_Mini_string;
            _settingMiniSizeW.SettingChanged += UpdateSettings_Mini_string;
            _settingEventSizeH.SettingChanged += UpdateSettings_Event_string;
            _settingEventSizeW.SettingChanged += UpdateSettings_Event_string;
            _settingEventHours.SettingChanged += UpdateSettings_Event_string;
            _settingAlertSizeW.SettingChanged += UpdateSettings_Alert_string;
            _settingAlertNotify.SettingChanged += UpdateSettings_Alert_string;
            _settingAlertDuration.SettingChanged += UpdateSettings_Alert_string;
            _settingAlertDrag.SettingChanged += UpdateSettings_bool;
            _settingAlertEnabled.SettingChanged += UpdateSettings_bool;
        }
        public override IView GetSettingsView() {
            return new Dailies.Views.SettingsView();
            //return new SettingsView( (this.ModuleParameters.SettingsManager.ModuleSettings);
        }

        private void UpdateSettings_Alert_string(object sender = null, ValueChangedEventArgs<string> e = null) {
            try {
                if (int.Parse(_settingAlertSizeW.Value) < 0)
                    _settingAlertSizeW.Value = "100";
            }
            catch {
                _settingAlertSizeW.Value = "100";
            }
            try {
                if (int.Parse(_settingAlertNotify.Value) < 0)
                    _settingEventHours.Value = "10";
            }
            catch {
                _settingAlertNotify.Value = "10";
            }
            try {
                if (int.Parse(_settingAlertDuration.Value) < 0)
                    _settingAlertDuration.Value = "0";
            }
            catch {
                _settingAlertDuration.Value = "0";
            }
            _alertWindow.Dispose();
            _alertWindow = new AlertWindow() {
                Location = _settingAlertLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
                Size = new Point(int.Parse(_settingAlertSizeW.Value), 0)
            };
            UpdateDailyPanel();
            if (_settingAlertEnabled.Value)
                _alertWindow.Show();
            else
                _alertWindow.Hide();
        }
        private void UpdateSettings_Mini_string(object sender = null, ValueChangedEventArgs<string> e = null) {
            try {
                if (int.Parse(_settingMiniSizeW.Value) < 0)
                    _settingMiniSizeW.Value = "100";
            }
            catch {
                _settingMiniSizeW.Value = "100";
            }
            try {
                if (int.Parse(_settingMiniSizeH.Value) < 0)
                    _settingMiniSizeH.Value = "100";
            }
            catch {
                _settingMiniSizeH.Value = "100";
            }
            _miniWindow.Dispose();
            _miniWindow = new MiniWindow(new Point(int.Parse(_settingMiniSizeW.Value), int.Parse(_settingMiniSizeH.Value))) {
                Location = _settingMiniLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
            };
            UpdateDailyPanel();
            _miniWindow.Show();
        }
        private void UpdateSettings_Event_string(object sender = null, ValueChangedEventArgs<string> e = null) {
            try {
                if (int.Parse(_settingEventSizeW.Value) < 0)
                    _settingEventSizeW.Value = "100";
            }
            catch {
                _settingEventSizeW.Value = "100";
            }
            try {
                if (int.Parse(_settingEventSizeH.Value) < 0)
                    _settingEventSizeH.Value = "100";
            }
            catch {
                _settingEventSizeH.Value = "100";
            }
            try {
                if (float.Parse(_settingEventHours.Value) < 0)
                    _settingEventHours.Value = "2";
            }
            catch {
                _settingEventHours.Value = "2";
            }
            _eventWindow.Dispose();
            _eventWindow = new EventWindow(new Point(int.Parse(_settingEventSizeW.Value), int.Parse(_settingEventSizeH.Value))) {
                Location = _settingEventLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
            };
            UpdateDailyPanel();
            _eventWindow.Show();
        }
        private void UpdateSettings_bool(object sender = null, ValueChangedEventArgs<bool> e = null) {
            UpdateDailyPanel();
            _alertWindow.Visible = _settingAlertEnabled.Value;
        }
        #endregion

        #region Startup
        protected override void Initialize() {
            _dailies = new List<Daily>();
            _newdailies = new List<Daily>();
            _categories = new List<Category>();

            _pageIcon = ContentsManager.GetTexture("42684bw.png");
            _eventIcon = ContentsManager.GetTexture("clockbw.png");
        }
        protected override async Task LoadAsync() {
            var sw = Stopwatch.StartNew();

            _cornerIcon = new CornerIcon() {
                IconName = "Dailies",
                Icon = _pageIcon,
                HoverIcon = _pageIcon,
                Priority = 10
            };
            _cornerIcon.LoadingMessage = "Loading Settings...";
            _cornerEventIcon = new CornerIcon() {
                IconName = "Events",
                Icon = _eventIcon,
                HoverIcon = _eventIcon,
                Priority = 10
            };
            _cornerEventIcon.LoadingMessage = "Loading...";

            _dailySettings = new DailySettings(DirectoriesManager.GetFullDirectoryPath("dailies"));
            _dailySettings.LoadSettings();
            if (_dailySettings._dailySettings.Count == 0)
                _dailySettings.SetTracked("gather_home", true);

            string dailiesDirectory = DirectoriesManager.GetFullDirectoryPath("dailies");
            try {
                //new WebClient().DownloadFile("https://raw.githubusercontent.com/manlaan/BlishHud-Dailies/main/DailyFiles/sample.txt", dailiesDirectory + "/sample.txt");
                new WebClient().DownloadFile("https://manlaan.000webhostapp.com/DailyFiles/sample.txt", dailiesDirectory + "/sample.txt");

                Directory.CreateDirectory(dailiesDirectory + "/cache");
                _cornerIcon.LoadingMessage = "Downloading Daily Files...";
                //new WebClient().DownloadFile("https://raw.githubusercontent.com/manlaan/BlishHud-Dailies/main/DailyFiles/files.json", dailiesDirectory + "/cache/files.json");
                new WebClient().DownloadFile("https://manlaan.000webhostapp.com/DailyFiles/files.json", dailiesDirectory + "/cache/files.json");
                List<FileList> files = readJsonFileList(new FileStream(dailiesDirectory + "/cache/files.json", FileMode.Open, FileAccess.Read), "files.json");
                foreach (FileList file in files) {
                    _cornerIcon.LoadingMessage = "Downloading and Adding Dailies: " + file.File + "...";
                    string filepath = dailiesDirectory + "\\cache\\" + file.File;
                    if (!File.Exists(filepath) || File.GetLastWriteTime(filepath) < DateTime.Parse(file.Date)) {
                        //new WebClient().DownloadFile("https://raw.githubusercontent.com/manlaan/BlishHud-Dailies/main/DailyFiles/" + file.File, dailiesDirectory + "/cache/" + file.File);
                        new WebClient().DownloadFile("https://manlaan.000webhostapp.com/DailyFiles/" + file.File, dailiesDirectory + "/cache/" + file.File);
                    }
                    if (file.File.ToLower().Contains(".json")) {
                        _dailies.AddRange(readJson(new FileStream(filepath, FileMode.Open, FileAccess.Read), file.File));
                    }
                }
            } catch { }


            foreach (string file in Directory.GetFiles(dailiesDirectory, ".")) {
                if (file.ToLower().Contains(".json") && !file.ToLower().Contains("settings.json")) {
                    if (file.ToLower().Contains("new.json")) {
                        _cornerIcon.LoadingMessage = "Loading Dailies: " + file + "...";
                        List<Daily> newdaily = readJson(new FileStream(file, FileMode.Open, FileAccess.Read), file);
                        foreach (Daily d in newdaily) {
                            if (_dailies.Find(x => x.Achievement.Equals(d.Achievement)) == null) {
                                _dailies.Add(d);
                                _newdailies.Add(d);
                            }
                        }
                    }
                    else {
                        _cornerIcon.LoadingMessage = "Loading Dailies: " + file + "...";
                        _dailies.AddRange(readJson(new FileStream(file, FileMode.Open, FileAccess.Read), file));
                    }
                }
            }

            FileStream createStream = File.Create(DirectoriesManager.GetFullDirectoryPath("dailies") + "\\new.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.SerializeAsync(createStream, _newdailies, options);
            createStream.Close();

            _cornerIcon.LoadingMessage = "Loading Categories...";
            foreach (Daily d in _dailies) {
                DailySettingEntry daySetting = _dailySettings.Get(d.Id);
                d.IsComplete = daySetting.IsComplete;
                d.IsTracked = daySetting.IsTracked;
                if (!_categories.Exists(x => x.Name.Equals(d.Category)))
                    _categories.Add(new Category() { Name = d.Category, IsActive = false });
            }

            _categories.Sort(delegate (Category x, Category y) {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });
            _dailies.Sort(delegate (Daily x, Daily y) {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });

            _cornerIcon.LoadingMessage = "Preloading Main Window...";
            _mainWindow = new MainWindow(Overlay.BlishHudWindow.ContentRegion.Size);
            _mainWindow.UpdatePanel();

            _cornerIcon.LoadingMessage = "Preloading Mini Window...";
            _miniWindow = new MiniWindow(new Point(int.Parse(_settingMiniSizeW.Value), int.Parse(_settingMiniSizeH.Value))) {
                Location = _settingMiniLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
            };
            _miniWindow.UpdatePanel();

            _cornerIcon.LoadingMessage = "Preloading Event Window...";
            _eventWindow = new EventWindow(new Point(int.Parse(_settingEventSizeW.Value), int.Parse(_settingEventSizeH.Value))) {
                Location = _settingEventLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
            };
            _eventWindow.UpdatePanel();

            _cornerIcon.LoadingMessage = "Preloading Alert Window...";
            _alertWindow = new AlertWindow() {
                Location = _settingAlertLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
                Size = new Point(int.Parse(_settingAlertSizeW.Value), 0)
            };

            UpdateAchievements();
            UpdateTimes();

            _cornerIcon.Click += delegate { _miniWindow.ToggleWindow(); };
            _cornerEventIcon.Click += delegate { _eventWindow.ToggleWindow(); };

            sw.Stop();
            Logger.Debug($"Took {sw.ElapsedMilliseconds} ms to complete loading...");
            _cornerIcon.LoadingMessage = "";
            _cornerEventIcon.LoadingMessage = "";

        }

        protected override void OnModuleLoaded(EventArgs e) {
            if (_settingAlertEnabled.Value)
                _alertWindow.Show();
            else
                _alertWindow.Hide();

            Timer_AchieveUpdate.Start();
            Timer_TimesUpdate.Start();
            Timer_APIUpdate.Start();
            _moduleTab = Overlay.BlishHudWindow.AddTab("Dailies", _pageIcon, _mainWindow._parentPanel);

            if (!_settingDontShowIntro.Value) {
                _introWindow = new IntroWindow() {
                    Location = new Point(200, 200),
                    Parent = GameService.Graphics.SpriteScreen,
                };
                _introWindow.Show();
            }

            // Base handler must be called
            base.OnModuleLoaded(e);
        }
        #endregion

        internal async void UpdateDailyPanel() {
            try {
                await Task.Run(() => _dailySettings.SaveSettings());
                await Task.Run(() => _eventWindow.UpdatePanel());
                await Task.Run(() => _alertWindow.UpdatePanel());
                _miniWindow.UpdatePanel();
                _mainWindow.UpdatePanel();
                UpdateTimes();
            }
            catch { }
        }
        internal async void UpdateAchievements() {
            if (_runningAchieve) return;
            _runningAchieve = true;

            Timer_AchieveUpdate.Restart();
            List<string> TodayAchieve = new List<string>(); //_APIDailies;
            List<Achievement> AllAchieves = new List<Achievement>();
            bool newDaily = false;

            try {
                var apiAchieveDay = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.Daily.GetAsync();
                foreach (var a in apiAchieveDay.Pve)
                    if (apiAchieveDay.Pve.Count(x => x.Id == a.Id) == 1)  //It appears that if an achievement is listed twice, usually once at a lower level, it is not available as a daily.
                        if (a.Level.Max == 80)
                            TodayAchieve.Add(a.Id.ToString());
                foreach (var a in apiAchieveDay.Fractals)
                    if (a.Level.Max == 80)
                        TodayAchieve.Add(a.Id.ToString());
                foreach (var a in apiAchieveDay.Pvp)
                    if (a.Level.Max == 80)
                        TodayAchieve.Add(a.Id.ToString());
                foreach (var a in apiAchieveDay.Special)
                    if (a.Level.Max == 80)
                        TodayAchieve.Add(a.Id.ToString());
                foreach (var a in apiAchieveDay.Wvw)
                    if (a.Level.Max == 80)
                        TodayAchieve.Add(a.Id.ToString());

                TodayAchieve.AddRange(UpdateGW2API());
                /// This doesn't work correctly due to GW2 API caching results for some categories for an hour.  <see cref="UpdateGW2API"/>  
                /// https://github.com/blish-hud/Blish-HUD/issues/445
                /*
                var apiGroup = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.Groups.GetAsync(new Guid("18DB115A-8637-4290-A636-821362A3C4A8"));
                foreach (int grp in apiGroup.Categories) {
                    if (grp != 97) { //dailies - retrieved from apiAchieveDay
                        var apidata = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.Categories.GetAsync(grp);
                        foreach (var data in apidata.Achievements) {
                            TodayAchieve.Add(data.ToString());
                        }
                    }
                }
                */

                foreach (string ach in TodayAchieve) {
                    Daily daily = _dailies.Find(x => x.Achievement.Equals(ach));
                    if (daily == null) {
                        newDaily = true;
                        var apidata = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.GetAsync(Int32.Parse(ach));

                        _newdailies.Add(new Daily { Id = "new_" + ach, Name = apidata.Name, Category = "New", Achievement = ach, API = "achievements", Note = apidata.Description });

                        _dailies.Add(new Daily { Id = "new_" + ach, Name = apidata.Name, Category = "New", Achievement = ach, API = "achievements", Note = apidata.Description });
                        daily = _dailies.Find(x => x.Achievement.Equals(ach));
                        daily.Button = _mainWindow.CreateButton(daily);
                        daily.MiniButton = _miniWindow.CreateButton(daily);

                        if (!_categories.Exists(x => x.Name.Equals("New")))
                            _categories.Add(new Category() { Name = "New", IsActive = false });
                    }
                }
            }
            catch { }


            TodayAchieve.AddRange(DailyList.Activity.Dailies());
            TodayAchieve.AddRange(DailyList.Merchants.Dailies());
            TodayAchieve.AddRange(DailyList.Awakened.Dailies());
            //TodayAchieve.AddRange(DailyList.FourWinds.Dailies(_settingFestivalStart.Value));  //Not needed. Part of Daily API
            //TodayAchieve.AddRange(DailyList.Halloween.Dailies(_settingFestivalStart.Value));  //Not needed. Part of Daily API
            //TodayAchieve.AddRange(DailyList.StrikeMissions.Dailies());  //For Weekly - Broken due to daily reset and completion not sent
            //TodayAchieve.AddRange(DailyList.DragonBash.Dailies(_settingFestivalStart.Value)); //Maybe part of daily api 
            //TodayAchieve.AddRange(DailyList.LunarNewYear.Dailies(_settingFestivalStart.Value));  //Maybe part of daily api
            //TodayAchieve.AddRange(DailyList.Wintersday.Dailies(_settingFestivalStart.Value));  //Maybe part of daily api


            if (Gw2ApiManager.HavePermissions(new[] { Gw2Sharp.WebApi.V2.Models.TokenPermission.Account, Gw2Sharp.WebApi.V2.Models.TokenPermission.Progression })) {
                try {
                    var apiDungeon = await Gw2ApiManager.Gw2ApiClient.V2.Account.Dungeons.GetAsync();
                    foreach (var a in apiDungeon) {
                        AllAchieves.Add(new Achievement { Id = a, Done = true, API = "dungeons" });
                    }
                    var apiChests = await Gw2ApiManager.Gw2ApiClient.V2.Account.MapChests.GetAsync();
                    foreach (var a in apiChests) {
                        AllAchieves.Add(new Achievement { Id = a, Done = true, API = "mapchests" });
                    }
                    var apiRaids = await Gw2ApiManager.Gw2ApiClient.V2.Account.Raids.GetAsync();
                    foreach (var a in apiRaids) {
                        AllAchieves.Add(new Achievement { Id = a, Done = true, API = "raids" });
                    }
                    var apiBosses = await Gw2ApiManager.Gw2ApiClient.V2.Account.WorldBosses.GetAsync();
                    foreach (var a in apiBosses) {
                        AllAchieves.Add(new Achievement { Id = a, Done = true, API = "worldbosses" });
                    }
                    var apiCraft = await Gw2ApiManager.Gw2ApiClient.V2.Account.DailyCrafting.GetAsync();
                    foreach (var a in apiCraft) {
                        AllAchieves.Add(new Achievement { Id = a, Done = true, API = "dailycrafting" });
                    }

                    foreach (Achievement ach in AllAchieves) {
                        Daily daily = _dailies.Find(x => x.Achievement.Equals(ach.Id));
                        if (daily == null) {
                            newDaily = true;
                            _newdailies.Add(new Daily { Id = "new_" + ach.Id, Name = ach.Id, Category = "New", Achievement = ach.Id, API = ach.API, Note = ach.API });

                            _dailies.Add(new Daily { Id = "new_" + ach.Id, Name = ach.Id, Category = "New", Achievement = ach.Id, API = ach.API, Note = ach.API });
                            daily = _dailies.Find(x => x.Achievement.Equals(ach.Id));
                            daily.Button = _mainWindow.CreateButton(daily);
                            daily.MiniButton = _miniWindow.CreateButton(daily);

                            if (!_categories.Exists(x => x.Name.Equals("New")))
                                _categories.Add(new Category() { Name = "New", IsActive = false });
                        }
                    }
                }
                catch { }
            }

            foreach (Daily d in _dailies) {
                if (!string.IsNullOrEmpty(d.Achievement) && !string.IsNullOrEmpty(d.API)) {
                    if (_autoCompleteAchievements.Contains(d.API)) { 
                        Achievement ach = AllAchieves.Find(x => x.Id.Equals(d.Achievement));
                        bool complete = (ach == null) ? false : ach.Done;
                        _dailySettings.SetComplete(d.Id, complete);
                        d.Button.CompleteButton.Checked = complete;
                        d.MiniButton.CompleteButton.Checked = complete;
                        d.IsComplete = complete;
                        d.IsDaily = true;
                    }
                    else {
                        d.IsDaily = string.IsNullOrEmpty(TodayAchieve.Find(x => x.Equals(d.Achievement))) ? false : true;
                    }
                }
                else {
                    d.IsDaily = true;
                }
            }

            if (newDaily) {
                var options = new JsonSerializerOptions { WriteIndented = true };
                FileStream createStream = File.Create(DirectoriesManager.GetFullDirectoryPath("dailies") + "\\new.json");
                await JsonSerializer.SerializeAsync(createStream, _newdailies, options);
                createStream.Close();
            }
            UpdateDailyPanel();

            _runningAchieve = false;
        }

        /// Doing this because GW2 API caches some of the categories for over an hour past reset
        /// https://github.com/blish-hud/Blish-HUD/issues/445
        private List<string> UpdateGW2API () {
            Timer_APIUpdate.Restart();

            List<string> achiev = new List<string>();
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                IgnoreNullValues = true
            };

            try {
                string contents = new WebClient().DownloadString("https://api.guildwars2.com/v2/achievements/groups/18DB115A-8637-4290-A636-821362A3C4A8?rand=" + DateTime.Now.Ticks.ToString());
                GW2API_Category json = JsonSerializer.Deserialize<GW2API_Category>(contents, jsonOptions);

                foreach (int i in json.Categories) {
                    if (i != 97) { //dailies - retrieved from apiAchieveDay
                        try {
                            string contents2 = new WebClient().DownloadString("https://api.guildwars2.com/v2/achievements/categories/" + i.ToString() + "? rand = " + DateTime.Now.Ticks.ToString());
                            GW2API_Group json2 = JsonSerializer.Deserialize<GW2API_Group>(contents2, jsonOptions);

                            foreach (int j in json2.Achievements) {
                                achiev.Add(j.ToString());
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return achiev;
        }

        private void UpdateTimes() {
            if (_runningTimes) return;
            _runningTimes = true;

            Timer_TimesUpdate.Restart();
            string timeformat = "h:mm tt";
            if (_setting24HrTime.Value) timeformat = "H:mm";
            foreach (Daily day in _dailies) {
                if (day.Times != null && day.Times.Length > 0) {
                    List<DateTime> times = new List<DateTime>();
                    foreach (string s in day.Times) {
                        times.Add(DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).ToLocalTime());
                    }
                    List<string> timestring_start = new List<string>();
                    List<string> timestring_end = new List<string>();
                    foreach (DateTime t in times) {
                        if (t > DateTime.Now) {
                            timestring_start.Add(t.ToString(timeformat));
                        }
                        else {
                            timestring_end.Add(t.ToString(timeformat));
                        }
                    }
                    timestring_start = timestring_start.Concat(timestring_end).ToList();
                    day.Button.TimeButton.Text = timestring_start.First();
                    day.Button.TimeButton.BasicTooltipText = string.Join("\n", timestring_start.ToArray());
                    if (day.MiniButton.Parent != null) {
                        day.MiniButton.TimeButton.Text = timestring_start.First();
                        day.MiniButton.TimeButton.BasicTooltipText = string.Join("\n", timestring_start.ToArray());
                    }
                }
            }

            _runningTimes = false;
        }

        protected override async void Update(GameTime gameTime) {
            if (Timer_APIUpdate.ElapsedMilliseconds > 300000) {   //5 minutes
                await Task.Run(() => UpdateAchievements());
            }
            if (Timer_TimesUpdate.ElapsedMilliseconds > 60000) {   //1 minute
                await Task.Run(() => _eventWindow.UpdatePanel());
                await Task.Run(() => _alertWindow.UpdatePanel());
                await Task.Run(() => UpdateTimes());
            }
            if (_settingLastReset.Value <= DateTime.UtcNow.Date.AddDays(-1).AddMinutes(-int.Parse(_settingAlertNotify.Value))) {
                _settingLastReset.Value = DateTime.UtcNow.AddMinutes(int.Parse(_settingAlertNotify.Value)).Date;
                _mainWindow.SetAllComplete(false, true);
                await Task.Run(() => UpdateAchievements());
            }
            if (_miniWindow.Location != _settingMiniLocation.Value)
                _settingMiniLocation.Value = _miniWindow.Location;
            if (_eventWindow.Location != _settingEventLocation.Value)
                _settingEventLocation.Value = _eventWindow.Location;
            if (_alertWindow.Location != _settingAlertLocation.Value)
                _settingAlertLocation.Value = _alertWindow.Location;
        }

        /// <inheritdoc />
        protected override void Unload() {
            Overlay.BlishHudWindow.RemoveTab(_moduleTab);
            _setting24HrTime.SettingChanged -= UpdateSettings_bool;
            _settingDontShowIntro.SettingChanged -= UpdateSettings_bool;
            _settingMiniSizeH.SettingChanged -= UpdateSettings_Mini_string;
            _settingMiniSizeW.SettingChanged -= UpdateSettings_Mini_string;
            _settingEventSizeH.SettingChanged -= UpdateSettings_Event_string;
            _settingEventSizeW.SettingChanged -= UpdateSettings_Event_string;
            _settingEventHours.SettingChanged -= UpdateSettings_Event_string;
            _settingAlertSizeW.SettingChanged -= UpdateSettings_Alert_string;
            _settingAlertNotify.SettingChanged -= UpdateSettings_Alert_string;
            _settingAlertDuration.SettingChanged -= UpdateSettings_Alert_string; 
            _settingAlertDrag.SettingChanged -= UpdateSettings_bool;
            _settingAlertEnabled.SettingChanged -= UpdateSettings_bool;

            _mainWindow?.Dispose();
            _miniWindow?.Dispose();
            _eventWindow?.Dispose();
            _alertWindow?.Dispose();
            _cornerIcon?.Dispose();
            _cornerEventIcon?.Dispose();

            _dailies = null;
            _newdailies = null;
            _categories = null;
            _settingLastReset = null;
            _dailySettings = null;
            ModuleInstance = null;
        }


        private List<Daily> readJson(Stream fileStream, string filename) {
            List<Daily> data = new List<Daily>();
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                IgnoreNullValues = true
            };

            string jsonContent;
            using (var jsonReader = new StreamReader(fileStream)) {
                jsonContent = jsonReader.ReadToEnd();
            }

            try {
                data = JsonSerializer.Deserialize<List<Daily>>(jsonContent, jsonOptions);
                Logger.Info("Loaded File: " + filename);
            }
            catch {
                Logger.Error("Failed Deserialization: " + filename);
            }
            return data;
        }
        private List<FileList> readJsonFileList(Stream fileStream, string filename) {
            List<FileList> data = new List<FileList>();
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                IgnoreNullValues = true
            };

            string jsonContent;
            using (var jsonReader = new StreamReader(fileStream)) {
                jsonContent = jsonReader.ReadToEnd();
            }

            try {
                data = JsonSerializer.Deserialize<List<FileList>>(jsonContent, jsonOptions);
                Logger.Info("Loaded File: " + filename);
            }
            catch {
                Logger.Error("Failed Deserialization: " + filename);
            }
            return data;
        }

        public static bool UrlIsValid(string source) => Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

        public static bool InSection(Daily d, string section, string search, string category) {
            switch (section) {
                case "Tracked - Incomplete":
                    if (d.IsTracked && !d.IsComplete && d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")) && d.IsDaily)
                        return true;
                    break;
                case "Tracked - Complete":
                    if (d.IsTracked && d.IsComplete && d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")) && d.IsDaily)
                        return true;
                    break;
                case "Tracked - All Daily":
                    if (d.IsTracked && d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")) && d.IsDaily)
                        return true;
                    break;
                case "Tracked - Not Daily":
                    if (d.IsTracked && d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")) && !d.IsDaily)
                        return true;
                    break;
                case "Tracked - All":
                    if (d.IsTracked && d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")))
                        return true;
                    break;
                case "Untracked":
                    if (!d.IsTracked && d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")))
                        return true;
                    break;
                default:
                    if (d.Name.ToLower().Contains(search.ToLower()) && (d.Category.Equals(category) || category.Equals("")))
                        return true;
                    break;
            }
            return false;
        }
    }
}
