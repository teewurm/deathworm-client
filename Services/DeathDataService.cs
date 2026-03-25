using DeathWorm.Clients;
using DeathWorm.Models;
using DeathWorm.Repositories;
using DeathWorm.Utils;
using System.Diagnostics;

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
    }
}
