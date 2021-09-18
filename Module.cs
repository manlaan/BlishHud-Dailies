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
using Manlaan.Dailies.Views;

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
        private SettingEntry<string> _settingFestivalStart;
        private SettingEntry<Point> _settingMiniLocation;
        private SettingEntry<string> _settingMiniSizeW, _settingMiniSizeH;
        private SettingEntry<bool> _setting24HrTime;
        private Stopwatch Timer_AchieveUpdate = new Stopwatch();
        private Stopwatch Timer_TimesUpdate = new Stopwatch();
        public static DailySettings _dailySettings;
        private Texture2D _pageIcon;

        private MainWindow _mainWindow;
        private MiniWindow _miniWindow;
        private CornerIcon _cornerIcon;
        private MiniWindow2 _miniWindow2;
        private MiniWindowView _miniWindow2View;
        private CornerIcon _cornerIcon2;

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
            _settingFestivalStart = settings.DefineSetting("DailyFestivalStart", @"01/01/2000", "Festival Start Date", "");
            _setting24HrTime = settings.DefineSetting("Daily24HrTime", false, "24 Hour Time", "");
            _settingLastReset = settings.DefineSetting("DailyLastUpdate", new DateTime());
            _settingMiniLocation = settings.DefineSetting("DailyMiniLoc", new Point(100, 100));
            _settingMiniSizeW = settings.DefineSetting("DailyMiniSizeH", @"280", "Mini Window Width", "");
            _settingMiniSizeH = settings.DefineSetting("DailyMiniSizeW", @"450", "Mini Window Height", "");
            _setting24HrTime.SettingChanged += UpdateSettings_bool;
            _settingMiniSizeH.SettingChanged += UpdateSettings_string;
            _settingMiniSizeW.SettingChanged += UpdateSettings_string;

            //_dailiesTracked = settings.AddSubCollection("Tracked");
            //_dailiesComplete = settings.AddSubCollection("Complete");
            //var selfManagedSettings = settings.AddSubCollection("Managed Settings");
        }
        private void UpdateSettings_string(object sender = null, ValueChangedEventArgs<string> e = null) {
            if (int.Parse(_settingMiniSizeH.Value) < 0)
                _settingMiniSizeH.Value = "0";
            if (int.Parse(_settingMiniSizeW.Value) < 0)
                _settingMiniSizeW.Value = "0";

            _miniWindow.Dispose();
            _miniWindow = new MiniWindow(new Point(int.Parse(_settingMiniSizeW.Value), int.Parse(_settingMiniSizeH.Value))) {
                Location = _settingMiniLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
            };
            //foreach (Daily d in _dailies) {
            //    d.MiniButton = _miniWindow.CreateDailyButton(d);
                //d.MiniButton2 = _miniWindow2View.CreateDailyButton(d);
            //}
            UpdateDailyPanel();
        }
        private void UpdateSettings_bool(object sender = null, ValueChangedEventArgs<bool> e = null) {
            UpdateTimes();
        }

        protected override void Initialize() {
            _dailies = new List<Daily>();
            _newdailies = new List<Daily>();
            _categories = new List<Category>();

            _pageIcon = ContentsManager.GetTexture("42684bw.png");

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

            _dailySettings = new DailySettings(DirectoriesManager.GetFullDirectoryPath("dailies"));
            _dailySettings.LoadSettings();
            if (_dailySettings._dailySettings.Count == 0)
                _dailySettings.SetTracked("gather_home", true);

            _cornerIcon.LoadingMessage = "Extracting Sample...";
            ExtractFile("sample.txt");

            string[] jsonfiles = {
                "Daily.json",
                "Adventures.json",
                "BjoraMarches.json",
                "Champions.json",
                "Converters.json",
                "Crafting.json",
                "Drizzlewood.json",
                "Dungeons.json",
                "Fractals.json",
                "Gathering.json",
                "GrothmarValley.json",
                "JumpingPuzzles.json",
                "LivingWorld.json",
                "Merchants.json",
                "Raids.json",
                "StrikeMissions.json",
                "WorldBosses.json",

                "DragonBash.json",
                "FourWinds.json",
                "Halloween.json",
                "LunarNewYear.json",
                "SuperAdventure.json",
                "Wintersday.json",
            };

            foreach (string file in jsonfiles) {
                _cornerIcon.LoadingMessage = "Loading Dailies: " + file + "...";
                _dailies.AddRange(readJson(ContentsManager.GetFileStream(file), file));
            }
            string dailiesDirectory = DirectoriesManager.GetFullDirectoryPath("dailies");
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            FileStream createStream = File.Create(DirectoriesManager.GetFullDirectoryPath("dailies") + "\\new.json");
            JsonSerializer.SerializeAsync(createStream, _newdailies, options);

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


            _cornerIcon.LoadingMessage = "Creating Main Window Buttons...";
            _mainWindow = new MainWindow(Overlay.BlishHudWindow.ContentRegion.Size);
            _cornerIcon.LoadingMessage = "Creating Mini Window Buttons...";
            _miniWindow = new MiniWindow(new Point(int.Parse(_settingMiniSizeW.Value), int.Parse(_settingMiniSizeH.Value))) {
                Location = _settingMiniLocation.Value,
                Parent = GameService.Graphics.SpriteScreen,
            };
            _miniWindow2 = new MiniWindow2(ContentsManager.GetTexture("155985.png"), new Rectangle(0, 0, 100, 100), new Rectangle(10, 20, 80, 80)) {
                Parent = Graphics.SpriteScreen,
                Title = "Dailies",
                Emblem = ContentsManager.GetTexture("icons\\42684.png"),
                SavesPosition = true,
                //SavesSize = true,
                Id = "Manlaan_Dailies_MiniWindow",
                CanClose = true,
                CanResize = true,
            };
            _miniWindow2View = new MiniWindowView();

            _cornerIcon.Click += delegate { _miniWindow.ToggleWindow(); };

            _cornerIcon2 = new CornerIcon() {
                IconName = "Dailies 2",
                Icon = _pageIcon,
                HoverIcon = _pageIcon,
                Priority = 10
            };
            _cornerIcon2.Click += delegate {
                _miniWindow2.ToggleWindow(_miniWindow2View);
            };


            sw.Stop();
            Logger.Debug($"Took {sw.ElapsedMilliseconds} ms to complete loading...");
            _cornerIcon.LoadingMessage = "";

            //Loading Icon Off
        }

        protected override void OnModuleLoaded(EventArgs e) {
            Timer_AchieveUpdate.Start();
            Timer_TimesUpdate.Start();
            _moduleTab = Overlay.BlishHudWindow.AddTab("Dailies", _pageIcon, _mainWindow._parentPanel);

            UpdateAchievements();
            UpdateTimes();

            // Base handler must be called
            base.OnModuleLoaded(e);
        }


        internal void UpdateDailyPanel() {
            _dailySettings.SaveSettings();
            _miniWindow.UpdateDailyPanel();
            _mainWindow.UpdateDailyPanel();
            //_miniWindow2View.UpdateDailyPanel();
        }
        internal async void UpdateAchievements() {
            Timer_AchieveUpdate.Restart();
            List<string> TodayAchieve = new List<string>();
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

                var apiGroup = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.Groups.GetAsync(new Guid("18DB115A-8637-4290-A636-821362A3C4A8"));
                foreach (int grp in apiGroup.Categories) {
                    if (grp != 9) { //dailies - retrieved from apiAchieveDay
                        var apidata = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.Categories.GetAsync(grp);
                        foreach (var data in apidata.Achievements) {
                            TodayAchieve.Add(data.ToString());
                        }
                    }
                }

                foreach (string ach in TodayAchieve) {
                    Daily daily = _dailies.Find(x => x.Achievement.Equals(ach));
                    if (daily == null) {
                        newDaily = true;
                        var apidata = await Gw2ApiManager.Gw2ApiClient.V2.Achievements.GetAsync(Int32.Parse(ach));

                        _newdailies.Add(new Daily { Id = "new_" + ach, Name = apidata.Name, Category = "New", Achievement = ach, API = "achievements", Note = apidata.Description });

                        _dailies.Add(new Daily { Id = "new_" + ach, Name = apidata.Name, Category = "New", Achievement = ach, API = "achievements", Note = apidata.Description });
                        daily = _dailies.Find(x => x.Achievement.Equals(ach));
                        daily.Button = _mainWindow.CreateDailyButton(daily);
                        daily.MiniButton = _miniWindow.CreateDailyButton(daily);
                        //daily.MiniButton2 = _miniWindow2View.CreateDailyButton(daily);

                        if (!_categories.Exists(x => x.Name.Equals("New")))
                            _categories.Add(new Category() { Name = "New", IsActive = false });
                    }
                }
            }
            catch { }


            TodayAchieve.AddRange(DailyList.Activity.Dailies());
            TodayAchieve.AddRange(DailyList.Merchants.Dailies());
            //TodayAchieve.AddRange(DailyList.FourWinds.Dailies(_settingFestivalStart.Value));  //Not needed. Part of Daily API  -- API very slow at updating this, so using manual method instead
            //TodayAchieve.AddRange(DailyList.StrikeMissions.Dailies());  //For Weekly - Broken due to daily reset and completion not sent
            //TodayAchieve.AddRange(DailyList.DragonBash.Dailies(_settingFestivalStart.Value)); //Maybe part of daily api 
            //TodayAchieve.AddRange(DailyList.Halloween.Dailies(_settingFestivalStart.Value));  //Maybe part of daily api
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
                            _newdailies.Add(new Daily { Id = "new_" + ach.Id, Name = ach.Id, Category = "New", Achievement = ach.Id, API = ach.API, Note = ach.API});

                            _dailies.Add(new Daily { Id = "new_" + ach.Id, Name = ach.Id, Category = "New", Achievement = ach.Id, API = ach.API, Note = ach.API });
                            daily = _dailies.Find(x => x.Achievement.Equals(ach.Id));
                            daily.Button = _mainWindow.CreateDailyButton(daily);
                            daily.MiniButton = _miniWindow.CreateDailyButton(daily);
                            //daily.MiniButton2 = _miniWindow2View.CreateDailyButton(daily);

                            if (!_categories.Exists(x => x.Name.Equals("New")))
                                _categories.Add(new Category() { Name = "New", IsActive = false });
                        }
                    }
                }
                catch { }
            }

            foreach (Daily btn in _dailies) {
                if (!string.IsNullOrEmpty(btn.Achievement) && !string.IsNullOrEmpty(btn.API)) {
                    switch (btn.API) {
                        default:
                            if (btn.Achievement.Equals("4283")) {
                                var a = btn;
                            }
                            btn.IsDaily = string.IsNullOrEmpty(TodayAchieve.Find(x => x.Equals(btn.Achievement))) ? false : true;
                            break;
                        //Autocompletes
                        case "dungeons":
                        case "mapchests":
                        case "raids":
                        case "worldbosses":
                        case "dailycrafting":
                            Achievement ach = AllAchieves.Find(x => x.Id.Equals(btn.Achievement));
                            bool complete = (ach == null) ? false : ach.Done;

                            _dailySettings.SetComplete(btn.Id, complete);

                            btn.IsComplete = complete;
                            btn.Button.CompleteButton.Checked = complete;
                            btn.MiniButton.CompleteButton.Checked = complete;
                            btn.IsDaily = true;
                            break;
                    }
                }
                else {
                    btn.IsDaily = true;
                }
            }
            if (newDaily) {
                var options = new JsonSerializerOptions { WriteIndented = true };
                FileStream createStream = File.Create(DirectoriesManager.GetFullDirectoryPath("dailies") + "\\new.json");
                await JsonSerializer.SerializeAsync(createStream, _newdailies, options);
            }
            UpdateDailyPanel();
        }
        private void UpdateTimes() {
            Timer_TimesUpdate.Restart();
            string timeformat = "h:mm tt";
            if (_setting24HrTime.Value) timeformat = "H:mm";
            foreach (Daily day in _dailies) {
                if (day.Times.Length > 0) {
                    if (day.Times != null && day.Times.Length > 0) {
                        List<DateTime> times = new List<DateTime>();
                        foreach (string s in day.Times) {
                            times.Add(DateTime.Parse(DateTime.UtcNow.Date.ToString("MM/dd/yyyy") + " " + s).ToLocalTime());
                        }
                        List<string> timestring_start = new List<string>();
                        List<string> timestring_end = new List<string>();
                        foreach (DateTime t in times) {
                            if (t > DateTime.Now)
                                timestring_start.Add(t.ToString(timeformat));
                            else
                                timestring_end.Add(t.ToString(timeformat));
                        }
                        timestring_start = timestring_start.Concat(timestring_end).ToList();
                        day.Button.TimeButton.Text = timestring_start.First();
                        day.MiniButton.TimeButton.Text = timestring_start.First();
                        day.Button.TimeButton.BasicTooltipText = string.Join("\n", timestring_start.ToArray());
                        day.MiniButton.TimeButton.BasicTooltipText = string.Join("\n", timestring_start.ToArray());
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime) {
            if (Timer_AchieveUpdate.ElapsedMilliseconds > 120000) {   //2 minutes  (2min * 60sec * 1000ms)
                UpdateAchievements();
            }
            if (Timer_TimesUpdate.ElapsedMilliseconds > 60000) {   //1 minutes  (1min * 60sec * 1000ms)
                UpdateTimes();
            }
            if (_settingLastReset.Value <= DateTime.UtcNow.Date.AddDays(-1)) {
                _settingLastReset.Value = DateTime.UtcNow.Date;
                _mainWindow.SetAllComplete(false, true);
                UpdateAchievements();
            }
            if (_miniWindow.Location != _settingMiniLocation.Value)
                _settingMiniLocation.Value = _miniWindow.Location;
        }

        /// <inheritdoc />
        protected override void Unload() {
            // Unload here
            Overlay.BlishHudWindow.RemoveTab(_moduleTab);
            _setting24HrTime.SettingChanged -= UpdateSettings_bool;
            _settingMiniSizeH.SettingChanged -= UpdateSettings_string;
            _settingMiniSizeW.SettingChanged -= UpdateSettings_string;

            _mainWindow?.Dispose();
            _miniWindow?.Dispose();
            //_miniWindow2?.Dispose();
            _cornerIcon?.Dispose();
            //_cornerIcon2?.Dispose();
            // All static members must be manually unset

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
        private void ExtractFile(string filePath) {
            var fullPath = Path.Combine(DirectoriesManager.GetFullDirectoryPath("dailies"), filePath);
            //if (File.Exists(fullPath)) return;
            using (var fs = ContentsManager.GetFileStream(filePath)) {
                fs.Position = 0;
                byte[] buffer = new byte[fs.Length];
                var content = fs.Read(buffer, 0, (int)fs.Length);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.WriteAllBytes(fullPath, buffer);
            }
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
