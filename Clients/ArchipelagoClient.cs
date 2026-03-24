using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using System.Net.Sockets;

namespace DeathWorm.Clients
{
    public record ConnectResult(bool Success, string? ErrorMessage = null);

    internal class ArchipelagoClient
    {
        private ArchipelagoSession _session;
        private DeathLinkService _deathLinkService;

        private string _server;
        private int _port;
        private string _userName;
        private string _gameName;

        public event Action<string>? OnPacketReceived;

        public ArchipelagoClient(string server, int port, string userName, string gameName)
        {
            _server = server;
            _port = port;
            _userName = userName;
            _gameName = gameName;

            _session = ArchipelagoSessionFactory.CreateSession(_server, _port);

            _session.Socket.PacketReceived += HandlePacketReceived;

            _session.Socket.ErrorReceived += OnErrorReceived;

            _deathLinkService = _session.CreateDeathLinkService();
        }

        private void OnErrorReceived(Exception e, string message)
        {
            Console.WriteLine(e.ToString());
        }

        ~ArchipelagoClient()
        {
            if (_session?.Socket != null)
                _session.Socket.PacketReceived -= HandlePacketReceived;
        }

        private void HandlePacketReceived(ArchipelagoPacketBase packet)
        {
            OnPacketReceived?.Invoke($"Packet received: {packet.PacketType}");
        }

        public ConnectResult Connect()
        {
            LoginResult result;

            try
            {
                result = _session.TryConnectAndLogin(_gameName, _userName, ItemsHandlingFlags.AllItems,
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
                string errorMessage = $"Failed to Connect to {_server} as {_userName}:";
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

            _deathLinkService.EnableDeathLink();

            return new ConnectResult(true);
        }

        public void SendDeathLink()
        {
            _deathLinkService.SendDeathLink(new DeathLink(sourcePlayer: _userName, "Fabi ist schuld"));
        }
    }
}
