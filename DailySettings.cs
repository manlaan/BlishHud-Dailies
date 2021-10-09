using System.Collections.Generic;
using System.IO;
using Manlaan.Dailies.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manlaan.Dailies
{
    public class DailySettings
    {
        private const string SETTINGS_FILENAME = "settings.json";
        public List<DailySettingEntry> _dailySettings = new List<DailySettingEntry>();
        private string _settingsFile = "";
        private bool _running = false;

        public DailySettings (string modulepath) {
            _settingsFile = modulepath + "\\" + SETTINGS_FILENAME;
        }

        public void Set(string id, bool tracked = false, bool complete = false) {
            DailySettingEntry search = _dailySettings.Find(x => x.Id.Equals(id));
            if (search == null || string.IsNullOrEmpty(search.Id)) {
                _dailySettings.Add(new DailySettingEntry() { Id = id, IsComplete = complete, IsTracked = tracked });
            }
            else {
                search.IsComplete = complete;
                search.IsTracked = tracked;
            }
            //SaveSettings();
        }
        public void SetTracked(string id, bool tracked = false) {
            DailySettingEntry search = _dailySettings.Find(x => x.Id.Equals(id));
            if (search == null || string.IsNullOrEmpty(search.Id)) {
                _dailySettings.Add(new DailySettingEntry() { Id = id, IsComplete = false, IsTracked = tracked });
            }
            else {
                search.IsTracked = tracked;
            }
            //SaveSettings();
        }
        public void SetComplete(string id, bool complete = false) {
            DailySettingEntry search = _dailySettings.Find(x => x.Id.Equals(id));
            if (search == null || string.IsNullOrEmpty(search.Id)) {
                _dailySettings.Add(new DailySettingEntry() { Id = id, IsComplete = complete, IsTracked = false });
            } 
            else { 
                search.IsComplete = complete;
            }
            //SaveSettings();
        }
        public DailySettingEntry Get(string id) {
            DailySettingEntry search = _dailySettings.Find(x => x.Id.Equals(id));
            if (search == null || string.IsNullOrEmpty(search.Id)) {
                _dailySettings.Add(new DailySettingEntry() { Id = id, IsComplete = false, IsTracked = false });
                return _dailySettings.Find(x => x.Id.Equals(id));
            }
            else {
                return search;
            }
        }
        public void LoadSettings() {

            if (File.Exists(_settingsFile)) {
                var options = new JsonSerializerOptions { IncludeFields = true };
                try {
                    string jsonString = File.ReadAllText(_settingsFile);
                    var json = JsonSerializer.Deserialize<List<DailySettingEntry>>(jsonString, options);
                    _dailySettings = json ;
                }
                catch { }
            }
            else {
                _dailySettings = new List<DailySettingEntry>();
                SaveSettings();
            }
        }

        public async void SaveSettings() {
            List<DailySettingEntry> toSave = new List<DailySettingEntry>();
            foreach (DailySettingEntry d in _dailySettings) {
                if (d.IsTracked || d.IsComplete)
                    toSave.Add(d);
            }
            var options = new JsonSerializerOptions { WriteIndented = true };
            FileStream createStream = File.Create(_settingsFile);
            await JsonSerializer.SerializeAsync(createStream, toSave, options);
            createStream.Close();
        }
    }
}
