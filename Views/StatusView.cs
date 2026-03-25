using DeathWorm.Services;
using DeathWorm.Clients;
using DeathWorm.Utils;
using DeathWorm.ViewModels;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DeathWorm.Views
{
    public enum StatusViewAction
    {
        None,
        Back,
        SendDeathLink,
        SendChat
    }

    public class StatusView
    {
        private readonly MessageService _messageService;
        private readonly DeathDataService _deathDataService;
        private readonly ArchipelagoClientService _archipelagoClient;
        private readonly MainView _mainView;
        private readonly MainViewModel _viewModel;
        private readonly TranslationService _t;

        public StatusView(
            MessageService messageService, 
            DeathDataService deathDataService, 
            ArchipelagoClientService archipelagoClient,
            MainView mainView,
            MainViewModel viewModel,
            TranslationService translationService)
        {
            _messageService = messageService;
            _deathDataService = deathDataService;
            _archipelagoClient = archipelagoClient;
            _mainView = mainView;
            _viewModel = viewModel;
            _t = translationService;
        }

        public void Show(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var action = ShowLiveStatus(cancellationToken);

                switch (action)
                {
                    case StatusViewAction.Back:
                        return;

                    case StatusViewAction.SendDeathLink:
                        SendDeathLink();
                        break;

                    case StatusViewAction.SendChat:
                        SendChat();
                        break;
                }
            }
        }

        private StatusViewAction ShowLiveStatus(CancellationToken cancellationToken)
        {
            AnsiConsole.Clear();
            var action = StatusViewAction.None;

            AnsiConsole.Live(CreateLayout())
                .Start(ctx =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ctx.Refresh();
                        ctx.UpdateTarget(CreateLayout());

                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true);
                            switch (key.KeyChar)
                            {
                                case '1':
                                    action = StatusViewAction.Back;
                                    return;
                                case '2':
                                    action = StatusViewAction.SendDeathLink;
                                    return;
                                case '3':
                                    action = StatusViewAction.SendChat;
                                    return;
                            }
                        }

                        Thread.Sleep(500);
                    }
                });

            return action;
        }

        private void SendDeathLink()
        {
            AnsiConsole.Clear();

            if (!_mainView.ConfirmDeathLink())
            {
                _mainView.ShowDeathLinkCancelled();
                _mainView.WaitForKeyPress();
                return;
            }

            var message = _mainView.PromptDeathLinkMessage();
            var result = _viewModel.SendDeathLink(message);

            if (result.Success)
            {
                _mainView.ShowDeathLinkSent();
            }
            else
            {
                _mainView.ShowError(result.ErrorMessage ?? "Unknown error");
            }

            _mainView.WaitForKeyPress();
        }

        private void SendChat()
        {
            AnsiConsole.Clear();

            var message = _mainView.PromptChatMessage();

            if (string.IsNullOrWhiteSpace(message))
            {
                _mainView.ShowChatCancelled();
                _mainView.WaitForKeyPress();
                return;
            }

            var result = _viewModel.Say(message);

            if (result.Success)
            {
                _mainView.ShowChatSent();
            }
            else
            {
                _mainView.ShowError(result.ErrorMessage ?? "Unknown error");
            }

            _mainView.WaitForKeyPress();
        }

        private IRenderable CreateLayout()
        {
            var connectionStatus = _archipelagoClient.IsConnected
                ? $"[green]{_t.Get(TranslationKeys.Connected)}[/]"
                : $"[red]{_t.Get(TranslationKeys.Disconnected)}[/]";

            var layout = new Table();
            layout.Border = TableBorder.None;
            layout.AddColumn(new TableColumn("").Width(50));
            layout.AddColumn(new TableColumn("").Width(50));
            layout.Title = new TableTitle($"[yellow]{_t.Get(TranslationKeys.StatusTitle)}[/] | {_t.Get(TranslationKeys.Connection)}: {connectionStatus}");
            layout.Caption = new TableTitle($"[grey][[1]][/] {_t.Get(TranslationKeys.BackToMainMenu)}  [grey][[2]][/] {_t.Get(TranslationKeys.SendDeathLink)}  [grey][[3]][/] {_t.Get(TranslationKeys.SendChat)}");

            layout.AddRow(CreateMessagesTable(), CreateDeathDataTable());

            return layout;
        }

        private Table CreateMessagesTable()
        {
            var table = new Table();
            table.AddColumn(_t.Get(TranslationKeys.Time));
            table.AddColumn(_t.Get(TranslationKeys.Message));
            table.Title = new TableTitle($"[yellow]{_t.Get(TranslationKeys.LiveMessages)}[/]");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var messages = _messageService.GetMessages();

            if (messages.Count == 0)
            {
                table.AddRow("[grey]-[/]", $"[grey]{_t.Get(TranslationKeys.NoMessages)}[/]");
            }
            else
            {
                foreach (var message in messages)
                {
                    table.AddRow(
                        $"[grey]{message.Timestamp:HH:mm:ss}[/]",
                        message.Text);
                }
            }

            return table;
        }

        private Table CreateDeathDataTable()
        {
            var table = new Table();
            table.AddColumn(_t.Get(TranslationKeys.Player));
            table.AddColumn(_t.Get(TranslationKeys.Deaths));
            table.AddColumn(_t.Get(TranslationKeys.Distribution));
            table.Title = new TableTitle($"[red]{_t.Get(TranslationKeys.DeathCounter)}[/]");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var deathData = _deathDataService.GetAllDeathData();

            if (deathData.Count == 0)
            {
                table.AddRow("[grey]-[/]", "[grey]-[/]", $"[grey]{_t.Get(TranslationKeys.NoData)}[/]");
            }
            else
            {
                var maxDeaths = deathData.Max(d => d.Timestamps.Count);
                const int barWidth = 20;

                foreach (var player in deathData.OrderByDescending(d => d.Timestamps.Count))
                {
                    var deathCount = player.Timestamps.Count;
                    var percentage = maxDeaths > 0 ? (double)deathCount / maxDeaths * 100 : 0;
                    var filledWidth = maxDeaths > 0 ? (int)Math.Round((double)deathCount / maxDeaths * barWidth) : 0;
                    var emptyWidth = barWidth - filledWidth;

                    var barColor = percentage switch
                    {
                        >= 66 => "red",
                        >= 33 => "yellow",
                        _ => "green"
                    };

                    var bar = $"[{barColor}]{new string('#', filledWidth)}[/][grey]{new string('-', emptyWidth)}[/]";

                    table.AddRow(
                        $"[blue]{Markup.Escape(player.PlayerName)}[/]",
                        $"[{barColor}]{deathCount}[/]",
                        bar);
                }
            }

            return table;
        }
    }
}
