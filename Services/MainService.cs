using DeathWorm.Utils;
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
        private readonly StatusView _statusView;
        private readonly TranslationService _t;

        public MainService(
            IHostApplicationLifetime lifetime,
            MainViewModel viewModel,
            MainView view,
            StatusView statusView,
            TranslationService translationService)
        {
            _lifetime = lifetime;
            _viewModel = viewModel;
            _view = view;
            _statusView = statusView;
            _t = translationService;
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

                if (choice == _t.Get(TranslationKeys.EditSettings))
                {
                    EditSettings();
                }
                else if (choice == _t.Get(TranslationKeys.Connect))
                {
                    Connect();
                    _view.WaitForKeyPress();
                }
                else if (choice == _t.Get(TranslationKeys.ShowStatus))
                {
                    _statusView.Show(cancellationToken);
                }
                else if (choice == _t.Get(TranslationKeys.SendDeathLink))
                {
                    SendDeathLink();
                    _view.WaitForKeyPress();
                }
                else if (choice == _t.Get(TranslationKeys.SendChat))
                {
                    SendChat();
                    _view.WaitForKeyPress();
                }
                else if (choice == _t.Get(TranslationKeys.Exit))
                {
                    _lifetime.StopApplication();
                    return;
                }
            }
        }

        private void EditSettings()
        {
            var settingChoice = _view.ShowSettingsMenu();

            if (settingChoice == _t.Get(TranslationKeys.Server))
            {
                var server = _view.PromptString(TranslationKeys.Server, _viewModel.Settings.Server);
                _viewModel.UpdateServer(server);
            }
            else if (settingChoice == _t.Get(TranslationKeys.Port))
            {
                var port = _view.PromptInt(TranslationKeys.Port, _viewModel.Settings.Port);
                _viewModel.UpdatePort(port);
            }
            else if (settingChoice == _t.Get(TranslationKeys.UserName))
            {
                var userName = _view.PromptString(TranslationKeys.UserName, _viewModel.Settings.UserName);
                _viewModel.UpdateUserName(userName);
            }
            else if (settingChoice == _t.Get(TranslationKeys.GameName))
            {
                var gameName = _view.PromptString(TranslationKeys.GameName, _viewModel.Settings.GameName);
                _viewModel.UpdateGameName(gameName);
            }
            else if (settingChoice == _t.Get(TranslationKeys.Language))
            {
                var language = _view.ShowLanguageMenu();
                _t.SetLanguage(language);
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
                _view.ShowConnectionError(result.ErrorMessage ?? "Unknown error");
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
                _view.ShowError(result.ErrorMessage ?? "Unknown error");
            }
        }

        private void SendChat()
        {
            var message = _view.PromptChatMessage();

            if (string.IsNullOrWhiteSpace(message))
            {
                _view.ShowChatCancelled();
                return;
            }

            var result = _viewModel.Say(message);

            if (result.Success)
            {
                _view.ShowChatSent();
            }
            else
            {
                _view.ShowError(result.ErrorMessage ?? "Unknown error");
            }
        }
    }
}
