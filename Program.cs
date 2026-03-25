using DeathWorm.Clients;
using DeathWorm.Repositories;
using DeathWorm.Services;
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
                    services.AddSingleton<SettingsRepository>();
                    services.AddSingleton<DeathDataRepository>();
                    services.AddSingleton<MessageService>();
                    services.AddSingleton<DeathDataService>();
                    services.AddSingleton<ArchipelagoClientService>();
                    services.AddSingleton<MainView>();
                    services.AddHostedService<MainService>();
                })
                .Build();

            host.Run();
        }
    }
}
