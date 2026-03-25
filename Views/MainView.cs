using DeathWorm.Utils;
using DeathWorm.Services;
using DeathWorm.Utils;
using DeathWorm.ViewModels;
using Spectre.Console;

namespace DeathWorm.Views
{
    public class MainView
    {
        private readonly MainViewModel _viewModel;
        private readonly TranslationService _t;

        public MainView(MainViewModel viewModel, TranslationService translationService)
        {
            _viewModel = viewModel;
            _t = translationService;
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
            table.AddColumn(_t.Get(TranslationKeys.SettingsTableTitle));
            table.AddColumn(_t.Get(TranslationKeys.SettingsTableValue));

            table.AddRow(_t.Get(TranslationKeys.Server), _viewModel.Settings.Server);
            table.AddRow(_t.Get(TranslationKeys.Port), _viewModel.Settings.Port.ToString());
            table.AddRow(_t.Get(TranslationKeys.UserName), _viewModel.Settings.UserName);
            table.AddRow(_t.Get(TranslationKeys.GameName), _viewModel.Settings.GameName);
            table.AddRow(_t.Get(TranslationKeys.Language), _t.CurrentLanguage.ToUpper());

            AnsiConsole.Write(table);
        }

        public string ShowMainMenu()
        {
            var choices = new List<string>
            {
                _t.Get(TranslationKeys.EditSettings),
                _t.Get(TranslationKeys.Connect),
                _t.Get(TranslationKeys.ShowStatus)
            };

            if (_viewModel.IsConnected)
            {
                choices.Add(_t.Get(TranslationKeys.SendDeathLink));
                choices.Add(_t.Get(TranslationKeys.SendChat));
            }

            choices.Add(_t.Get(TranslationKeys.Exit));

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[green]{_t.Get(TranslationKeys.MainMenuTitle)}[/]")
                    .AddChoices(choices));
        }

        public string ShowSettingsMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{_t.Get(TranslationKeys.SettingsMenuTitle)}[/]")
                    .AddChoices(
                        _t.Get(TranslationKeys.Server),
                        _t.Get(TranslationKeys.Port),
                        _t.Get(TranslationKeys.UserName),
                        _t.Get(TranslationKeys.GameName),
                        _t.Get(TranslationKeys.Language),
                        _t.Get(TranslationKeys.Back)));
        }

        public string ShowLanguageMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{_t.Get(TranslationKeys.Language)}[/]")
                    .AddChoices(Translations.SupportedLanguages.ToArray()));
        }

        public string PromptString(string key, string defaultValue)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"[blue]{_t.Get(key)}:[/]")
                    .DefaultValue(defaultValue));
        }

        public int PromptInt(string key, int defaultValue)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>($"[blue]{_t.Get(key)}:[/]")
                    .DefaultValue(defaultValue));
        }

        public void ShowConnecting()
        {
            AnsiConsole.MarkupLine($"[yellow]{_t.Get(TranslationKeys.Connecting, _viewModel.Settings.Server, _viewModel.Settings.Port)}[/]");
        }

        public void ShowConnectionSuccess()
        {
            AnsiConsole.MarkupLine($"[green]{_t.Get(TranslationKeys.ConnectionSuccess)}[/]");
        }

        public void ShowConnectionError(string errorMessage)
        {
            AnsiConsole.MarkupLine($"[red]{_t.Get(TranslationKeys.ConnectionFailed)}[/]");
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(errorMessage)}[/]");
        }

        public void ShowDeathLinkSent()
        {
            AnsiConsole.MarkupLine($"[green]{_t.Get(TranslationKeys.DeathLinkSent)}[/]");
        }

        public void ShowDeathLinkCancelled()
        {
            AnsiConsole.MarkupLine($"[yellow]{_t.Get(TranslationKeys.DeathLinkCancelled)}[/]");
        }

        public bool ConfirmDeathLink()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[red]{_t.Get(TranslationKeys.DeathLinkConfirm)}[/]")
                    .AddChoices(_t.Get(TranslationKeys.Yes), _t.Get(TranslationKeys.No)));

            return choice == _t.Get(TranslationKeys.Yes);
        }

        public string PromptDeathLinkMessage()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"[blue]{_t.Get(TranslationKeys.DeathLinkMessagePrompt)}[/]")
                    .AllowEmpty());
        }

        public string PromptChatMessage()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"[blue]{_t.Get(TranslationKeys.ChatMessagePrompt)}[/]")
                    .AllowEmpty());
        }

        public void ShowChatSent()
        {
            AnsiConsole.MarkupLine($"[green]{_t.Get(TranslationKeys.ChatSent)}[/]");
        }

        public void ShowChatCancelled()
        {
            AnsiConsole.MarkupLine($"[yellow]{_t.Get(TranslationKeys.ChatCancelled)}[/]");
        }

        public void ShowError(string errorMessage)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(errorMessage)}[/]");
        }

        public void WaitForKeyPress()
        {
            AnsiConsole.MarkupLine($"[grey]{_t.Get(TranslationKeys.PressKeyToContinue)}[/]");
            Console.ReadKey(true);
        }
    }
}
