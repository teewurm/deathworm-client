using DeathWorm.Clients;
using DeathWorm.Models;
using DeathWorm.Repositories;
using DeathWorm.Views;
using Microsoft.Extensions.Hosting;

namespace DeathWorm.Services
{
    public class MainService : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly SettingsRepository _settingsRepository;
        private readonly ArchipelagoClientService _archipelagoService;
        private readonly MessageService _messageService;
        private readonly MainView _view;

        private AppSettings _settings;

        public MainService(
            IHostApplicationLifetime lifetime, 
            SettingsRepository settingsRepository,
            ArchipelagoClientService archipelagoService,
            MessageService messageService,
            MainView view)
        {
            _lifetime = lifetime;
            _settingsRepository = settingsRepository;
            _archipelagoService = archipelagoService;
            _messageService = messageService;
            _view = view;
            _settings = _settingsRepository.Load();
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
                _view.ShowSettings(_settings);
                _view.ShowMessages(_messageService.GetMessages());

                var choice = _view.ShowMainMenu();

                switch (choice)
                {
                    case "Aktualisieren":
                        break;

                    case "Einstellungen bearbeiten":
                        EditSettings();
                        break;

                    case "Verbinden":
                        Connect();
                        _view.WaitForKeyPress();
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
                    _settings.Server = _view.PromptString("Server", _settings.Server);
                    _settingsRepository.Save(_settings);
                    break;

                case "Port":
                    _settings.Port = _view.PromptInt("Port", _settings.Port);
                    _settingsRepository.Save(_settings);
                    break;

                case "Benutzername":
                    _settings.UserName = _view.PromptString("Benutzername", _settings.UserName);
                    _settingsRepository.Save(_settings);
                    break;

                case "Spielname":
                    _settings.GameName = _view.PromptString("Spielname", _settings.GameName);
                    _settingsRepository.Save(_settings);
                    break;

                case "Zurück":
                    break;
            }
        }

        private void Connect()
        {
            _settings = _settingsRepository.Load();
            _view.ShowConnecting(_settings.Server, _settings.Port);

            var result = _archipelagoService.Connect();

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
            var result = _archipelagoService.SendDeathLink();

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
