using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Packets;
using DeathWorm.Extensions;
using DeathWorm.Models;
using DeathWorm.Repositories;
using DeathWorm.Services;
using DeathWorm.Utils;

namespace DeathWorm.Clients
{
    public record ConnectResult(bool Success, string? ErrorMessage = null);

    public class ArchipelagoClientService
    {
        private readonly SettingsRepository _settingsRepository;
        private readonly DeathDataService _deathDataService;
        private readonly MessageService _messageService;

        private ArchipelagoSession? _session;
        private DeathLinkService? _deathLinkService;

        private bool _isConnected;
        public bool IsConnected => _isConnected;

        public ArchipelagoClientService(SettingsRepository settingsRepository, DeathDataService deathDataService, MessageService messageService)
        {
            _settingsRepository = settingsRepository;
            _deathDataService = deathDataService;
            _messageService = messageService;
        }

        private void InitializeSession(AppSettings settings)
        {
            CleanUpSession();

            _session = ArchipelagoSessionFactory.CreateSession(settings.Server, settings.Port);

            _session.Socket.PacketReceived += HandlePacketReceived;
            _session.Socket.ErrorReceived += OnErrorReceived;
            _session.Socket.SocketClosed += OnSocketClosed;

            _deathLinkService = _session.CreateDeathLinkService();
        }

        private void CleanUpSession()
        {
            // Cleanup old session if exists
            if (_session?.Socket != null)
            {
                _session.Socket.PacketReceived -= HandlePacketReceived;
                _session.Socket.ErrorReceived -= OnErrorReceived;
                _session.Socket.SocketClosed -= OnSocketClosed;
            }

            _session = null;
        }

        private void OnErrorReceived(Exception e, string message)
        {
            _isConnected = false;
            CleanUpSession();
        }

        private void OnSocketClosed(string reason)
        {
            _isConnected = false;
            CleanUpSession();
        }

        private void HandlePacketReceived(ArchipelagoPacketBase packet)
        {           
            _messageService.AddMessage(packet, _session.Players, _session.Items);

            if (packet.TryGetDeathLink(out var deathLink))
            {
                _deathDataService.AddDeath(deathLink.Source, deathLink.Timestamp);
            }
        }

        public ConnectResult Connect()
        {
            if (IsConnected) return new ConnectResult(false, "Bereits verbunden");


            var settings = _settingsRepository.Load();
            InitializeSession(settings);

            LoginResult result;

            try
            {
                result = _session!.TryConnectAndLogin(settings.GameName, settings.UserName, ItemsHandlingFlags.AllItems,
                    version: new Version(0, 6, 6),
                    tags: ["DeathLink"]);
            }
            catch (Exception e)
            {
                return new ConnectResult(false, e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {settings.Server} as {settings.UserName}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                return new ConnectResult(false, errorMessage);
            }

            _deathLinkService!.EnableDeathLink();

            // Alle Spieler in der DeathData anlegen
            var playerNames = _session.Players.AllPlayers
                .Where(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name != "Server")
                .Select(p => p.Name);
            _deathDataService.EnsurePlayersExist(playerNames);

            try
            {
                _session.SetGoalAchieved();
            }
            catch
            {
                return new ConnectResult(false, "Failed to mark goal as achieved.");
            }

            _isConnected = true;

            return new ConnectResult(true);
        }

        public ConnectResult SendDeathLink(string? message = null)
        {
            if (!_isConnected)
            {
                return new ConnectResult(false, "Nicht verbunden. Bitte zuerst verbinden.");
            }

            var settings = _settingsRepository.Load();
            var deathMessage = string.IsNullOrWhiteSpace(message)
                ? DeathLinkMessages.GetRandomMessage()
                : message;

            _deathLinkService!.SendDeathLink(new DeathLink(sourcePlayer: settings.UserName, deathMessage));
            return new ConnectResult(true);
        }

        public ConnectResult Say(string message)
        {
            if (!_isConnected)
            {
                return new ConnectResult(false, "Nicht verbunden. Bitte zuerst verbinden.");
            }

            try
            {
                _session!.Say(message);
            }
            catch (Exception e)
            {
                return new ConnectResult(false, e.GetBaseException().Message);
            }


            return new ConnectResult(true);
        }
    }
}
