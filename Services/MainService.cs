using DeathWorm.Clients;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace DeathWorm.Services
{
    public class MainService : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly List<string> _packetLog = new();
        private readonly object _logLock = new();

        //private string _server = "archipelago.gg";
        private string _server = "localhost";
        private int _port = 38281;
        private string _userName = "deathworm";
        private string _gameName = "Deathworm";

        public MainService(IHostApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
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
                    _server = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Server:[/]")
                            .DefaultValue(_server));
                    break;

                case "Port":
                    _port = AnsiConsole.Prompt(
                        new TextPrompt<int>("[blue]Port:[/]")
                            .DefaultValue(_port));
                    break;

                case "Benutzername":
                    _userName = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Benutzername:[/]")
                            .DefaultValue(_userName));
                    break;

                case "Spielname":
                    _gameName = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Spielname:[/]")
                            .DefaultValue(_gameName));
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

            table.AddRow("Server", _server);
            table.AddRow("Port", _port.ToString());
            table.AddRow("Benutzername", _userName);
            table.AddRow("Spielname", _gameName);

            AnsiConsole.Write(table);
        }

        private ArchipelagoClient? Connect()
        {
            AnsiConsole.MarkupLine($"[yellow]Verbinde mit {_server}:{_port}...[/]");

            var client = new ArchipelagoClient(
                server: _server,
                port: _port,
                userName: _userName,
                gameName: _gameName);

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
