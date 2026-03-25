using DeathWorm.Utils;
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
            var choices = new List<string>
            {
                MenuChoices.EditSettings,
                MenuChoices.Connect
            };

            if (_viewModel.IsConnected)
            {
                choices.Add(MenuChoices.ShowStatus);
                choices.Add(MenuChoices.SendDeathLink);
                choices.Add(MenuChoices.SendChat);
            }

            choices.Add(MenuChoices.Exit);

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Hauptmenü[/]")
                    .AddChoices(choices));
        }

        public string ShowSettingsMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Welche Einstellung möchtest du ändern?[/]")
                    .AddChoices(
                        MenuChoices.Server,
                        MenuChoices.Port,
                        MenuChoices.UserName,
                        MenuChoices.GameName,
                        MenuChoices.Back));
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

        public void ShowDeathLinkCancelled()
        {
            AnsiConsole.MarkupLine("[yellow]Death Link abgebrochen.[/]");
        }

        public bool ConfirmDeathLink()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[red]Möchtest du wirklich einen Death Link senden?[/]")
                    .AddChoices(MenuChoices.Yes, MenuChoices.No));

            return choice == MenuChoices.Yes;
        }

        public string PromptDeathLinkMessage()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("[blue]Nachricht (leer für zufällige Nachricht):[/]")
                    .AllowEmpty());
        }

        public string PromptChatMessage()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("[blue]Chat-Nachricht (leer zum Abbrechen):[/]")
                    .AllowEmpty());
        }

        public void ShowChatSent()
        {
            AnsiConsole.MarkupLine("[green]Chat-Nachricht gesendet![/]");
        }

        public void ShowChatCancelled()
        {
            AnsiConsole.MarkupLine("[yellow]Chat abgebrochen.[/]");
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
