using System.Text.Json;
using DeathWorm.Models;
using DeathWorm.Utils;

namespace DeathWorm.Repositories
{
    public class DeathDataRepository
    {
        private readonly string _dataDirectory;
        private readonly string _dataFilePath;
        private readonly object _fileLock = new();
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public DeathDataRepository()
        {
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeathWorm");
            _dataFilePath = Path.Combine(_dataDirectory, Constants.DeathDataFile);
        }

        public void AddDeath(string playerName, DateTime timestamp)
        {
            lock (_fileLock)
            {
                var allDeathData = Load();

                var playerData = allDeathData.FirstOrDefault(d => d.PlayerName == playerName);

                if (playerData == null)
                {
                    playerData = new DeathData
                    {
                        PlayerName = playerName,
                        Timestamps = [timestamp]
                    };
                    allDeathData.Add(playerData);
                }
                else
                {
                    playerData.Timestamps.Add(timestamp);
                }

                Save(allDeathData);
            }
        }

        public List<DeathData> Load()
        {
            if (!File.Exists(_dataFilePath))
            {
                return [];
            }

            try
            {
                var json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<DeathData>>(json, _jsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }

        private void Save(List<DeathData> deathData)
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            var json = JsonSerializer.Serialize(deathData, _jsonOptions);
            File.WriteAllText(_dataFilePath, json);
        }

        public void EnsurePlayerExists(string playerName)
        {
            lock (_fileLock)
            {
                var allDeathData = Load();

                var existingPlayer = allDeathData.FirstOrDefault(d => d.PlayerName == playerName);
                if (existingPlayer == null)
                {
                    allDeathData.Add(new DeathData
                    {
                        PlayerName = playerName,
                        Timestamps = []
                    });
                    Save(allDeathData);
                }
            }
        }

        public void Clear()
        {
            lock (_fileLock)
            {
                Save([]);
            }
        }
    }
}
