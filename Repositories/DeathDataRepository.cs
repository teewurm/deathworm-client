using System.Text.Json;
using System.Text.Json;
using DeathWorm.Models;
using DeathWorm.Utils;

namespace DeathWorm.Repositories
{
    public class DeathDataRepository
    {
        private readonly string _dataDirectory;
        private readonly string _dataFilePath;
        private readonly string _otherDeathDataDirectory;
        private readonly object _fileLock = new();
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public string OtherDeathDataDirectory => _otherDeathDataDirectory;

        public DeathDataRepository()
        {
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeathWorm");
            _dataFilePath = Path.Combine(_dataDirectory, Constants.DeathDataFile);
            _otherDeathDataDirectory = Path.Combine(_dataDirectory, Constants.OtherDeathDataFolder);

            EnsureDirectoriesExist();
        }

        private void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            if (!Directory.Exists(_otherDeathDataDirectory))
            {
                Directory.CreateDirectory(_otherDeathDataDirectory);
            }
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

        public List<DeathData>? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<DeathData>>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public string[] GetOtherDeathDataFiles()
        {
            if (!Directory.Exists(_otherDeathDataDirectory))
            {
                return [];
            }

            return Directory.GetFiles(_otherDeathDataDirectory, "*.json");
        }

        public int MergeAllFromOtherDeathDataFolder()
        {
            var files = GetOtherDeathDataFiles();
            var mergedCount = 0;

            foreach (var file in files)
            {
                var externalData = LoadFromFile(file);
                if (externalData != null)
                {
                    Merge(externalData);
                    mergedCount++;
                }
            }

            return mergedCount;
        }

        public void Merge(List<DeathData> externalData)
        {
            lock (_fileLock)
            {
                var currentData = Load();

                foreach (var externalPlayer in externalData)
                {
                    var existingPlayer = currentData.FirstOrDefault(d => d.PlayerName == externalPlayer.PlayerName);

                    if (existingPlayer == null)
                    {
                        // Spieler existiert nicht, komplett hinzufügen
                        currentData.Add(new DeathData
                        {
                            PlayerName = externalPlayer.PlayerName,
                            Timestamps = externalPlayer.Timestamps.Distinct().ToList()
                        });
                    }
                    else
                    {
                        // Spieler existiert, Timestamps zusammenführen (ohne Duplikate)
                        var mergedTimestamps = existingPlayer.Timestamps
                            .Union(externalPlayer.Timestamps)
                            .Distinct()
                            .OrderBy(t => t)
                            .ToList();

                        existingPlayer.Timestamps = mergedTimestamps;
                    }
                }

                Save(currentData);
            }
        }
    }
}
