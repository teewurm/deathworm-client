using DeathWorm.ViewModels;
using Spectre.Console;

namespace DeathWorm.Views
{
    public class MainView
    {
        private readonly MainViewModel _viewModel;

        public MainView(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Clear()
        {
            AnsiConsole.Clear();
        }

        public void ShowHeader()
        {
            AnsiConsole.Write(new FigletText("DeathWorm").Color(Color.Red));
        }

        public void ShowSettings()
        {
            var table = new Table();
            table.AddColumn("Einstellung");
            table.AddColumn("Wert");

            table.AddRow("Server", _viewModel.Settings.Server);
            table.AddRow("Port", _viewModel.Settings.Port.ToString());
            table.AddRow("Benutzername", _viewModel.Settings.UserName);
            table.AddRow("Spielname", _viewModel.Settings.GameName);

            AnsiConsole.Write(table);
        }

        public string ShowMainMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Hauptmenü[/]")
                    .AddChoices(
                        "Einstellungen bearbeiten",
                        "Verbinden",
                        "Nachrichten anzeigen",
                        "Death Link senden",
                        "Beenden"));
        }

        public string ShowSettingsMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Welche Einstellung möchtest du ändern?[/]")
                    .AddChoices(
                        "Server",
                        "Port",
                        "Benutzername",
                        "Spielname",
                        "Zurück"));
        }

        public string PromptString(string label, string defaultValue)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"[blue]{label}:[/]")
                    .DefaultValue(defaultValue));
        }

        public int PromptInt(string label, int defaultValue)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>($"[blue]{label}:[/]")
                    .DefaultValue(defaultValue));
        }

        public void ShowConnecting()
        {
            AnsiConsole.MarkupLine($"[yellow]Verbinde mit {_viewModel.Settings.Server}:{_viewModel.Settings.Port}...[/]");
        }

        public void ShowConnectionSuccess()
        {
            AnsiConsole.MarkupLine("[green]Erfolgreich verbunden![/]");
        }

        public void ShowConnectionError(string errorMessage)
        {
            AnsiConsole.MarkupLine("[red]Verbindung fehlgeschlagen![/]");
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(errorMessage)}[/]");
        }

        public void ShowDeathLinkSent()
        {
            AnsiConsole.MarkupLine("[green]Death Link gesendet![/]");
        }

        public void ShowError(string errorMessage)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(errorMessage)}[/]");
        }

        public void WaitForKeyPress()
        {
            AnsiConsole.MarkupLine("[grey]Drücke eine Taste um fortzufahren...[/]");
            Console.ReadKey(true);
        }
    }
}
