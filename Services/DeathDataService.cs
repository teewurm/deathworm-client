using DeathWorm.Models;
using DeathWorm.Repositories;

namespace DeathWorm.Services
{
    public class DeathDataService
    {
        private readonly DeathDataRepository _deathDataRepository;
        private readonly object _cacheLock = new();
        private List<DeathData> _cachedDeathData;

        public DeathDataService(DeathDataRepository deathDataRepository)
        {
            _deathDataRepository = deathDataRepository;
            _cachedDeathData = _deathDataRepository.Load();
        }

        public void RefreshCache()
        {
            lock (_cacheLock)
            {
                _cachedDeathData = _deathDataRepository.Load();
            }
        }

        public void AddDeath(string playerName, DateTime timestamp)
        {
            lock (_cacheLock)
            {
                _deathDataRepository.AddDeath(playerName, timestamp);

                // Cache aktualisieren
                var playerData = _cachedDeathData.FirstOrDefault(d => d.PlayerName == playerName);
                if (playerData == null)
                {
                    playerData = new DeathData
                    {
                        PlayerName = playerName,
                        Timestamps = [timestamp]
                    };
                    _cachedDeathData.Add(playerData);
                }
                else
                {
                    playerData.Timestamps.Add(timestamp);
                }
            }
        }

        public List<DeathData> GetAllDeathData()
        {
            lock (_cacheLock)
            {
                return _cachedDeathData.ToList();
            }
        }

        public DeathData? GetDeathDataForPlayer(string playerName)
        {
            lock (_cacheLock)
            {
                return _cachedDeathData.FirstOrDefault(d => d.PlayerName == playerName);
            }
        }

        public int GetTotalDeathCount()
        {
            lock (_cacheLock)
            {
                return _cachedDeathData.Sum(d => d.Timestamps.Count);
            }
        }

        public void EnsurePlayersExist(IEnumerable<string> playerNames)
        {
            lock (_cacheLock)
            {
                foreach (var playerName in playerNames)
                {
                    if (string.IsNullOrWhiteSpace(playerName))
                        continue;

                    var existingPlayer = _cachedDeathData.FirstOrDefault(d => d.PlayerName == playerName);
                    if (existingPlayer == null)
                    {
                        var newPlayer = new DeathData
                        {
                            PlayerName = playerName,
                            Timestamps = []
                        };
                        _cachedDeathData.Add(newPlayer);
                        _deathDataRepository.EnsurePlayerExists(playerName);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (_cacheLock)
            {
                _deathDataRepository.Clear();
                _cachedDeathData = [];
            }
        }

        public string GetOtherDeathDataDirectory()
        {
            return _deathDataRepository.OtherDeathDataDirectory;
        }

        public bool HasOtherDeathDataFiles()
        {
            return _deathDataRepository.GetOtherDeathDataFiles().Length > 0;
        }

        public (bool Success, int MergedCount) MergeFromOtherDeathDataFolder()
        {
            lock (_cacheLock)
            {
                var mergedCount = _deathDataRepository.MergeAllFromOtherDeathDataFolder();
                _cachedDeathData = _deathDataRepository.Load();

                return (mergedCount > 0, mergedCount);
            }
        }
    }
}
