using DeathWorm.Utils;
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
        private readonly StatusView _messagesView;

        public MainService(
            IHostApplicationLifetime lifetime,
            MainViewModel viewModel,
            MainView view,
            StatusView messagesView)
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
                    case MenuChoices.EditSettings:
                        EditSettings();
                        break;

                    case MenuChoices.Connect:
                        Connect();
                        _view.WaitForKeyPress();
                        break;

                    case MenuChoices.ShowStatus:
                        _messagesView.Show(cancellationToken);
                        break;

                    case MenuChoices.SendDeathLink:
                        SendDeathLink();
                        _view.WaitForKeyPress();
                        break;

                    case MenuChoices.Exit:
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
                case MenuChoices.Server:
                    var server = _view.PromptString("Server", _viewModel.Settings.Server);
                    _viewModel.UpdateServer(server);
                    break;

                case MenuChoices.Port:
                    var port = _view.PromptInt("Port", _viewModel.Settings.Port);
                    _viewModel.UpdatePort(port);
                    break;

                case MenuChoices.UserName:
                    var userName = _view.PromptString("Benutzername", _viewModel.Settings.UserName);
                    _viewModel.UpdateUserName(userName);
                    break;

                case MenuChoices.GameName:
                    var gameName = _view.PromptString("Spielname", _viewModel.Settings.GameName);
                    _viewModel.UpdateGameName(gameName);
                    break;

                case MenuChoices.Back:
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
            if (!_view.ConfirmDeathLink())
            {
                _view.ShowDeathLinkCancelled();
                return;
            }

            var message = _view.PromptDeathLinkMessage();
            var result = _viewModel.SendDeathLink(message);

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
