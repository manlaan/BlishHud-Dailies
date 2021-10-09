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
using System.Text;
using System.Net.Http;

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
        private List<string> _APIDailies = new List<string>();

        public static SettingEntry<DateTime> _settingLastReset;
        //private SettingEntry<string> _settingFestivalStart;
        private SettingEntry<Point> _settingMiniLocation;
        private SettingEntry<Point> _settingEventLocation;
        private SettingEntry<string> _settingMiniSizeW, _settingMiniSizeH;
        private SettingEntry<string> _settingEventSizeW, _settingEventSizeH;
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

        private WindowTab _moduleTab;


        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion
        internal static Module Instance { get; private set; }

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings) {
            //_settingFestivalStart = settings.DefineSetting("DailyFestivalStart", @"01/01/2000", "Festival Start Date", "");
            _setting24HrTime = settings.DefineSetting("Daily24HrTime", false, "24 Hour Time", "");
            _settingDebug = settings.DefineSetting("DailyDebug", false, "Debug (restart required)", "");
            _settingLastReset = settings.DefineSetting("DailyLastUpdate", new DateTime());
            _settingMiniLocation = settings.DefineSetting("DailyMiniLoc", new Point(100, 100));
            _settingEventLocation = settings.DefineSetting("DailyEventLoc", new Point(100, 100));
            _settingEventHours = settings.DefineSetting("DailyEventHours", "2", "Event Hours", "");
            _settingMiniSizeW = settings.DefineSetting("DailyMiniSizeH", @"280", "Mini Window Width", "");
            _settingMiniSizeH = settings.DefineSetting("DailyMiniSizeW", @"450", "Mini Window Height", "");
            _settingEventSizeW = settings.DefineSetting("DailyEventSizeH", @"600", "Event Window Width", "");
            _settingEventSizeH = settings.DefineSetting("DailyEventSizeW", @"300", "Event Window Height", "");
            _settingDontShowIntro = settings.DefineSetting("DailyDontShowIntro", false, "Don't Show Intro", "");
            _setting24HrTime.SettingChanged += UpdateSettings_bool;
            _settingDontShowIntro.SettingChanged += UpdateSettings_bool;
            _settingMiniSizeH.SettingChanged += UpdateSettings_string;
            _settingMiniSizeW.SettingChanged += UpdateSettings_string;
            _settingEventSizeH.SettingChanged += UpdateSettings_string;
            _settingEventSizeW.SettingChanged += UpdateSettings_string;
            _settingEventHours.SettingChanged += UpdateSettings_string;

            //_dailiesTracked = settings.AddSubCollection("Tracked");
            //_dailiesComplete = settings.AddSubCollection("Complete");
            //var selfManagedSettings = settings.AddSubCollection("Managed Settings");
        }
        private void UpdateSettings_string(object sender = null, ValueChangedEventArgs<string> e = null) {
            int prevMiniSizeH = _miniSizeH;
            int prevMiniSizeW = _miniSizeW;
            int prevEventSizeH = _eventSizeH;
            int prevEventSizeW = _eventSizeW;
            int prevEventHours = _eventHours;
            try {
                _miniSizeW = int.Parse(_settingMiniSizeW.Value);
                if (_miniSizeW < 0)
                    _miniSizeW = prevMiniSizeW;
                _miniSizeH = int.Parse(_settingMiniSizeH.Value);
                if (_miniSizeH < 0)
                    _miniSizeH = prevMiniSizeH;
            }
            catch {
                _settingMiniSizeW.Value = _miniSizeW.ToString();
                _settingMiniSizeH.Value = _miniSizeH.ToString();
            }
            try {
                _eventSizeW = int.Parse(_settingEventSizeW.Value);
                if (_eventSizeW < 0)
                    _eventSizeW = prevEventSizeW;
                _eventSizeH = int.Parse(_settingEventSizeH.Value);
                if (_eventSizeH < 0)
                    _eventSizeH = prevEventSizeH;
                _eventHours = int.Parse(_settingEventHours.Value);
                if (_eventHours < 0)
                    _eventHours = prevEventHours;
            }
            catch {
                _settingEventSizeW.Value = _eventSizeW.ToString();
                _settingEventSizeH.Value = _eventSizeH.ToString();
                _settingEventHours.Value = _eventHours.ToString();
            }


            if (prevMiniSizeH != _miniSizeH || prevMiniSizeW != _miniSizeH) {
                _miniWindow.Dispose();
                _miniWindow = new MiniWindow(new Point(int.Parse(_settingMiniSizeW.Value), int.Parse(_settingMiniSizeH.Value))) {
                    Location = _settingMiniLocation.Value,
                    Parent = GameService.Graphics.SpriteScreen,
                };
            }
            if (prevEventSizeH != _eventSizeH || prevEventSizeW != _eventSizeW || prevEventHours != _eventHours) {
                _eventWindow.Dispose();
                _eventWindow = new EventWindow(new Point(int.Parse(_settingEventSizeW.Value), int.Parse(_settingEventSizeH.Value))) {
                    Location = _settingEventLocation.Value,
                    Parent = GameService.Graphics.SpriteScreen,
                };
            }
            UpdateDailyPanel();
        }
        private void UpdateSettings_bool(object sender = null, ValueChangedEventArgs<bool> e = null) {
            UpdateDailyPanel();
        }

        protected override void Initialize() {
            _dailies = new List<Daily>();
            _newdailies = new List<Daily>();
            _categories = new List<Category>();

            _pageIcon = ContentsManager.GetTexture("42684bw.png");
            _eventIcon = ContentsManager.GetTexture("clockbw.png");
        }
        protected override async Task LoadAsync() {
            var sw = Stopwatch.StartNew();
            UpdateGW2API();

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
                new WebClient().DownloadFile("https://raw.githubusercontent.com/manlaan/BlishHud-Dailies/main/DailyFiles/sample.txt", dailiesDirectory + "/sample.txt");

                Directory.CreateDirectory(dailiesDirectory + "/cache");
                _cornerIcon.LoadingMessage = "Downloading Daily Files...";
                new WebClient().DownloadFile("https://raw.githubusercontent.com/manlaan/BlishHud-Dailies/main/DailyFiles/files.json", dailiesDirectory + "/cache/files.json");
                List<FileList> files = readJsonFileList(new FileStream(dailiesDirectory + "/cache/files.json", FileMode.Open, FileAccess.Read), "files.json");
                foreach (FileList file in files) {
                    _cornerIcon.LoadingMessage = "Downloading and Adding Dailies: " + file.File + "...";
                    string filepath = dailiesDirectory + "\\cache\\" + file.File;
                    var a = File.Exists(filepath);
                    var b = File.GetLastWriteTime(filepath);
                    var c = DateTime.Parse(file.Date);
                    if (!File.Exists(filepath) || File.GetLastWriteTime(filepath) < DateTime.Parse(file.Date)) {
                        new WebClient().DownloadFile("https://raw.githubusercontent.com/manlaan/BlishHud-Dailies/main/DailyFiles/" + file.File, dailiesDirectory + "/cache/" + file.File);
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

            _cornerIcon.Click += delegate { _miniWindow.ToggleWindow(); };
            _cornerEventIcon.Click += delegate { _eventWindow.ToggleWindow(); };

            sw.Stop();
            Logger.Debug($"Took {sw.ElapsedMilliseconds} ms to complete loading...");
            _cornerIcon.LoadingMessage = "";
            _cornerEventIcon.LoadingMessage = "";

        }

        protected override void OnModuleLoaded(EventArgs e) {
            Timer_AchieveUpdate.Start();
            Timer_TimesUpdate.Start();
            Timer_APIUpdate.Start();
            _moduleTab = Overlay.BlishHudWindow.AddTab("Dailies", _pageIcon, _mainWindow._parentPanel);

            if (!_settingDontShowIntro.Value) {
                _introWindow = new IntroWindow(new Point(375, 260)) {
                    Location = new Point(200, 200),
                    Parent = GameService.Graphics.SpriteScreen,
                };
                _introWindow.Show();
            }

            UpdateAchievements();
            UpdateTimes();

            // Base handler must be called
            base.OnModuleLoaded(e);
        }


        internal async void UpdateDailyPanel() {
            try {
                await Task.Run(() => _dailySettings.SaveSettings());
                //await Task.Run(() => _miniWindow.UpdatePanel());
                //await Task.Run(() => _mainWindow.UpdatePanel());
                await Task.Run(() => _eventWindow.UpdatePanel());
                //await Task.Run(() => UpdateTimes());
                //_dailySettings.SaveSettings();
                _miniWindow.UpdatePanel();
                _mainWindow.UpdatePanel();
                //_eventWindow.UpdatePanel();
                UpdateTimes();
            }
            catch { }
        }
        internal async void UpdateAchievements() {
            if (_runningAchieve) return;
            _runningAchieve = true;

            Timer_AchieveUpdate.Restart();
            List<string> TodayAchieve = _APIDailies;
            List<Achievement> AllAchieves = new List<Achievement>();
            bool newDaily = false;

            try {
                var apiAchieveDay = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.Daily.GetAsync();
                foreach (var a in apiAchieveDay.Fractals)
                    if (a.Level.Max == 80)
                        TodayAchieve.Add(a.Id.ToString());
                foreach (var a in apiAchieveDay.Pve)
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

                /// This doesn't work correctly due to GW2Sharp caching results for some categories for over an hour past reset.  <see cref="UpdateGW2API"/> 
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
                    switch (d.API) {
                        default:
                            d.IsDaily = string.IsNullOrEmpty(TodayAchieve.Find(x => x.Equals(d.Achievement))) ? false : true;
                            break;
                        //Autocompletes
                        case "dungeons":
                        case "mapchests":
                        case "raids":
                        case "worldbosses":
                        case "dailycrafting":
                            Achievement ach = AllAchieves.Find(x => x.Id.Equals(d.Achievement));
                            bool complete = (ach == null) ? false : ach.Done;

                            _dailySettings.SetComplete(d.Id, complete);

                            d.IsComplete = complete;
                            //d.Button.CompleteButton.Checked = complete;
                            //if (d.MiniButton.Parent != null)
                            //    d.MiniButton.CompleteButton.Checked = complete;
                            d.IsDaily = true;
                            break;
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
        
        /* Doing this because gw2sharp seems to cache some of the categories for over an hour past reset */
        private async void UpdateGW2API () {
            Timer_APIUpdate.Restart();

            List<string> achiev = new List<string>();
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                IgnoreNullValues = true
            };

            try {
                string contents = new WebClient().DownloadString("https://api.guildwars2.com/v2/achievements/groups/18DB115A-8637-4290-A636-821362A3C4A8");
                GW2API_Category json = JsonSerializer.Deserialize<GW2API_Category>(contents, jsonOptions);

                foreach (int i in json.Categories) {
                    try {
                        string contents2 = new WebClient().DownloadString("https://api.guildwars2.com/v2/achievements/categories/" + i.ToString());
                        GW2API_Group json2 = JsonSerializer.Deserialize<GW2API_Group>(contents2, jsonOptions);

                        foreach (int j in json2.Achievements) {
                            if (j != 97)  //dailies - retrieved from apiAchieveDay
                                achiev.Add(j.ToString());
                        }
                    }
                    catch { }
                }
            }
            catch { }

            _APIDailies = achiev;
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
            if (Timer_AchieveUpdate.ElapsedMilliseconds > 120000) {   //2 minutes  (5min * 60sec * 1000ms)
                await Task.Run(() => UpdateAchievements());
            }
            if (Timer_APIUpdate.ElapsedMilliseconds > 300000) {   //5 minutes
                await Task.Run(() => UpdateGW2API());
            }
            if (Timer_TimesUpdate.ElapsedMilliseconds > 60000) {   //1 minute
                await Task.Run(() => _eventWindow.UpdatePanel());
                await Task.Run(() => UpdateTimes());
            }
            if (_settingLastReset.Value <= DateTime.UtcNow.Date.AddDays(-1)) {
                _settingLastReset.Value = DateTime.UtcNow.Date;
                _mainWindow.SetAllComplete(false, true);
                await Task.Run(() => UpdateGW2API());
                await Task.Run(() => UpdateAchievements());
            }
            if (_miniWindow.Location != _settingMiniLocation.Value)
                _settingMiniLocation.Value = _miniWindow.Location;
            if (_eventWindow.Location != _settingEventLocation.Value)
                _settingEventLocation.Value = _eventWindow.Location;
        }

        /// <inheritdoc />
        protected override void Unload() {
            // Unload here
            Overlay.BlishHudWindow.RemoveTab(_moduleTab);
            _setting24HrTime.SettingChanged -= UpdateSettings_bool;
            _settingMiniSizeH.SettingChanged -= UpdateSettings_string;
            _settingMiniSizeW.SettingChanged -= UpdateSettings_string;
            _settingDontShowIntro.SettingChanged -= UpdateSettings_bool;
            _settingEventHours.SettingChanged -= UpdateSettings_string;


            _mainWindow?.Dispose();
            _miniWindow?.Dispose();
            _eventWindow?.Dispose();
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
            catch (Exception ex) {
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
            catch (Exception ex) {
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
