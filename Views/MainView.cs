using DeathWorm.Models;
using Spectre.Console;

namespace DeathWorm.Views
{
    public class MainView
    {
        public void Clear()
        {
            AnsiConsole.Clear();
        }

        public void ShowHeader()
        {
            AnsiConsole.Write(new FigletText("DeathWorm").Color(Color.Red));
        }

        public void ShowSettings(AppSettings settings)
        {
            var table = new Table();
            table.AddColumn("Einstellung");
            table.AddColumn("Wert");

            table.AddRow("Server", settings.Server);
            table.AddRow("Port", settings.Port.ToString());
            table.AddRow("Benutzername", settings.UserName);
            table.AddRow("Spielname", settings.GameName);

            AnsiConsole.Write(table);
        }

        public void ShowMessages(List<string> messages)
        {
            if (messages.Count > 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]Letzte Nachrichten:[/]");
                foreach (var message in messages)
                {
                    AnsiConsole.MarkupLine(message);
                }
            }
        }

        public string ShowMainMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Hauptmenü[/]")
                    .AddChoices(
                        "Aktualisieren",
                        "Einstellungen bearbeiten",
                        "Verbinden",
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

        public void ShowConnecting(string server, int port)
        {
            AnsiConsole.MarkupLine($"[yellow]Verbinde mit {server}:{port}...[/]");
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
