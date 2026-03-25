using DeathWorm.Clients;
using DeathWorm.Models;
using DeathWorm.Repositories;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace DeathWorm.Services
{
    public class MainService : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly SettingsRepository _settingsRepository;
        private readonly List<string> _packetLog = new();
        private readonly object _logLock = new();

        private AppSettings _settings;

        public MainService(IHostApplicationLifetime lifetime, SettingsRepository settingsRepository)
        {
            _lifetime = lifetime;
            _settingsRepository = settingsRepository;
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

        private void OnClientPacketReceived(string message)
        {
            lock (_logLock)
            {
                _packetLog.Add($"[grey]{DateTime.Now:HH:mm:ss}[/] {message}");
                if (_packetLog.Count > 5)
                    _packetLog.RemoveAt(0);
            }
        }

        private void ShowPacketLog()
        {
            lock (_logLock)
            {
                if (_packetLog.Count > 0)
                {
                    AnsiConsole.MarkupLine("\n[yellow]Letzte Pakete:[/]");
                    foreach (var log in _packetLog)
                    {
                        AnsiConsole.MarkupLine(log);
                    }
                }
            }
        }

        private void Run(CancellationToken cancellationToken)
        {
            ArchipelagoClient? client = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("DeathWorm").Color(Color.Red));
                ShowCurrentSettings();
                ShowPacketLog();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Hauptmenü[/]")
                        .AddChoices(
                            "Aktualisieren",
                            "Einstellungen bearbeiten",
                            "Verbinden",
                            "Death Link senden",
                            "Beenden"));

                switch (choice)
                {
                    case "Aktualisieren":
                        // Menü wird automatisch neu gezeichnet
                        break;

                    case "Einstellungen bearbeiten":
                        EditSettings();
                        break;

                    case "Verbinden":
                        client = Connect();
                        AnsiConsole.MarkupLine("[grey]Drücke eine Taste um fortzufahren...[/]");
                        Console.ReadKey(true);
                        break;

                    case "Death Link senden":
                        if (client != null)
                        {
                            client.SendDeathLink();
                            AnsiConsole.MarkupLine("[green]Death Link gesendet![/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Nicht verbunden! Bitte zuerst verbinden.[/]");
                        }
                        AnsiConsole.MarkupLine("[grey]Drücke eine Taste um fortzufahren...[/]");
                        Console.ReadKey(true);
                        break;

                    case "Beenden":
                        _lifetime.StopApplication();
                        return;
                }
            }
        }

        private void EditSettings()
        {
            var settingChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Welche Einstellung möchtest du ändern?[/]")
                    .AddChoices(
                        "Server",
                        "Port",
                        "Benutzername",
                        "Spielname",
                        "Zurück"));

            switch (settingChoice)
            {
                case "Server":
                    _settings.Server = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Server:[/]")
                            .DefaultValue(_settings.Server));
                    _settingsRepository.Save(_settings);
                    break;

                case "Port":
                    _settings.Port = AnsiConsole.Prompt(
                        new TextPrompt<int>("[blue]Port:[/]")
                            .DefaultValue(_settings.Port));
                    _settingsRepository.Save(_settings);
                    break;

                case "Benutzername":
                    _settings.UserName = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Benutzername:[/]")
                            .DefaultValue(_settings.UserName));
                    _settingsRepository.Save(_settings);
                    break;

                case "Spielname":
                    _settings.GameName = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Spielname:[/]")
                            .DefaultValue(_settings.GameName));
                    _settingsRepository.Save(_settings);
                    break;

                case "Zurück":
                    break;
            }
        }

        private void ShowCurrentSettings()
        {
            var table = new Table();
            table.AddColumn("Einstellung");
            table.AddColumn("Wert");

            table.AddRow("Server", _settings.Server);
            table.AddRow("Port", _settings.Port.ToString());
            table.AddRow("Benutzername", _settings.UserName);
            table.AddRow("Spielname", _settings.GameName);

            AnsiConsole.Write(table);
        }

        private ArchipelagoClient? Connect()
        {
            AnsiConsole.MarkupLine($"[yellow]Verbinde mit {_settings.Server}:{_settings.Port}...[/]");

            var client = new ArchipelagoClient(
                server: _settings.Server,
                port: _settings.Port,
                userName: _settings.UserName,
                gameName: _settings.GameName);

            client.OnPacketReceived += OnClientPacketReceived;

            var result = client.Connect();

            if (result.Success)
            {
                AnsiConsole.MarkupLine("[green]Erfolgreich verbunden![/]");
                return client;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Verbindung fehlgeschlagen![/]");
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(result.ErrorMessage ?? "Unbekannter Fehler")}[/]");
                client.OnPacketReceived -= OnClientPacketReceived;
                return null;
            }
        }
    }
}
