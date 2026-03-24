using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;

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

        public bool IsConnected => _session.Socket.Connected;

        public ArchipelagoClient(string server, int port, string userName, string gameName)
        {
            _server = server;
            _port = port;
            _userName = userName;
            _gameName = gameName;

            _session = ArchipelagoSessionFactory.CreateSession(_server, _port);

            _deathLinkService = _session.CreateDeathLinkService();
            _deathLinkService.OnDeathLinkReceived += OnDeathLinkReceived;
        }

        ~ArchipelagoClient()
        {
            if (_deathLinkService != null)
                _deathLinkService.OnDeathLinkReceived -= OnDeathLinkReceived;
        }

        private void OnDeathLinkReceived(DeathLink deathLink)
        {
        }

        public async Task<ConnectResult> ConnectAsync()
        {
            LoginResult result;

            try
            {
                await _session.ConnectAsync();
                result = await _session.LoginAsync(_gameName, _userName, ItemsHandlingFlags.NoItems,
                    version: null,
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
            _deathLinkService.SendDeathLink(new DeathLink(sourcePlayer: _userName, "Died to exposure."));
        }
    }
}
