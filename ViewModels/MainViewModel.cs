using DeathWorm.Clients;
using DeathWorm.Clients;
using DeathWorm.Models;
using DeathWorm.Repositories;

namespace DeathWorm.ViewModels
{
    public class MainViewModel
    {
        private readonly SettingsRepository _settingsRepository;
        private readonly ArchipelagoClientService _archipelagoService;

        public AppSettings Settings { get; private set; }
        public bool IsConnected => _archipelagoService.IsConnected;

        public MainViewModel(
            SettingsRepository settingsRepository,
            ArchipelagoClientService archipelagoService)
        {
            _settingsRepository = settingsRepository;
            _archipelagoService = archipelagoService;
            Settings = _settingsRepository.Load();
        }

        public void RefreshSettings()
        {
            Settings = _settingsRepository.Load();
        }

        public void UpdateServer(string server)
        {
            Settings.Server = server;
            _settingsRepository.Save(Settings);
        }

        public void UpdatePort(int port)
        {
            Settings.Port = port;
            _settingsRepository.Save(Settings);
        }

        public void UpdateUserName(string userName)
        {
            Settings.UserName = userName;
            _settingsRepository.Save(Settings);
        }

        public void UpdateGameName(string gameName)
        {
            Settings.GameName = gameName;
            _settingsRepository.Save(Settings);
        }

        public ConnectResult Connect()
        {
            RefreshSettings();
            return _archipelagoService.Connect();
        }

        public ConnectResult SendDeathLink(string? message = null)
        {
            return _archipelagoService.SendDeathLink(message);
        }
    }
}
