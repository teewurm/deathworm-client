using DeathWorm.Clients;
using DeathWorm.Repositories;
using DeathWorm.Services;
using DeathWorm.ViewModels;
using DeathWorm.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeathWorm
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
                .ConfigureServices(services =>
                {
                    // Repositories
                    services.AddSingleton<SettingsRepository>();
                    services.AddSingleton<DeathDataRepository>();

                    // Services
                    services.AddSingleton<MessageService>();
                    services.AddSingleton<DeathDataService>();
                    services.AddSingleton<ArchipelagoClientService>();

                    // MVVM
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainView>();
                    services.AddSingleton<MessagesView>();

                    services.AddHostedService<MainService>();
                })
                .Build();

            host.Run();
        }
    }
}
