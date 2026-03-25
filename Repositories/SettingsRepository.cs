using System.Text.Json;
using DeathWorm.Models;

namespace DeathWorm.Repositories
{
    public class SettingsRepository
    {
        private readonly string _settingsDirectory;
        private readonly string _settingsFilePath;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public SettingsRepository()
        {
            _settingsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeathWorm");
            _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
        }

        public AppSettings Load()
        {
            if (!File.Exists(_settingsFilePath))
            {
                var defaultSettings = new AppSettings();
                Save(defaultSettings);
                return defaultSettings;
            }

            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            if (!Directory.Exists(_settingsDirectory))
            {
                Directory.CreateDirectory(_settingsDirectory);
            }

            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_settingsFilePath, json);
        }
    }
}
