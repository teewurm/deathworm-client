using DeathWorm.ViewModels;
using DeathWorm.Views;
using Microsoft.Extensions.Hosting;

namespace DeathWorm.Services
{
    public class MainService : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly MainViewModel _viewModel;
        private readonly MainView _view;
        private readonly MessagesView _messagesView;

        public MainService(
            IHostApplicationLifetime lifetime,
            MainViewModel viewModel,
            MainView view,
            MessagesView messagesView)
        {
            _lifetime = lifetime;
            _viewModel = viewModel;
            _view = view;
            _messagesView = messagesView;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(() => Run(cancellationToken));
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _view.Clear();
                _view.ShowHeader();
                _view.ShowSettings();

                var choice = _view.ShowMainMenu();

                switch (choice)
                {
                    case "Einstellungen bearbeiten":
                        EditSettings();
                        break;

                    case "Verbinden":
                        Connect();
                        _view.WaitForKeyPress();
                        break;

                    case "Nachrichten anzeigen":
                        _messagesView.Show(cancellationToken);
                        break;

                    case "Death Link senden":
                        SendDeathLink();
                        _view.WaitForKeyPress();
                        break;

                    case "Beenden":
                        _lifetime.StopApplication();
                        return;
                }
            }
        }

        private void EditSettings()
        {
            var settingChoice = _view.ShowSettingsMenu();

            switch (settingChoice)
            {
                case "Server":
                    var server = _view.PromptString("Server", _viewModel.Settings.Server);
                    _viewModel.UpdateServer(server);
                    break;

                case "Port":
                    var port = _view.PromptInt("Port", _viewModel.Settings.Port);
                    _viewModel.UpdatePort(port);
                    break;

                case "Benutzername":
                    var userName = _view.PromptString("Benutzername", _viewModel.Settings.UserName);
                    _viewModel.UpdateUserName(userName);
                    break;

                case "Spielname":
                    var gameName = _view.PromptString("Spielname", _viewModel.Settings.GameName);
                    _viewModel.UpdateGameName(gameName);
                    break;

                case "Zurück":
                    break;
            }
        }

        private void Connect()
        {
            _view.ShowConnecting();

            var result = _viewModel.Connect();

            if (result.Success)
            {
                _view.ShowConnectionSuccess();
            }
            else
            {
                _view.ShowConnectionError(result.ErrorMessage ?? "Unbekannter Fehler");
            }
        }

        private void SendDeathLink()
        {
            var result = _viewModel.SendDeathLink();

            if (result.Success)
            {
                _view.ShowDeathLinkSent();
            }
            else
            {
                _view.ShowError(result.ErrorMessage ?? "Unbekannter Fehler");
            }
        }
    }
}
