using DeathWorm.Services;
using DeathWorm.Clients;
using DeathWorm.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DeathWorm.Views
{
    public class StatusView
    {
        private readonly MessageService _messageService;
        private readonly DeathDataService _deathDataService;
        private readonly ArchipelagoClientService _archipelagoClient;

        public StatusView(MessageService messageService, DeathDataService deathDataService, ArchipelagoClientService archipelagoClient)
        {
            _messageService = messageService;
            _deathDataService = deathDataService;
            _archipelagoClient = archipelagoClient;
        }

        public void Show(CancellationToken cancellationToken)
        {
            AnsiConsole.Clear();

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
                            if (key.KeyChar == '1')
                            {
                                return;
                            }
                        }

                        Thread.Sleep(500);
                    }
                });
        }

        private IRenderable CreateLayout()
        {
            var connectionStatus = _archipelagoClient.IsConnected
                ? "[green]Verbunden[/]"
                : "[red]Nicht verbunden[/]";

            var layout = new Table();
            layout.Border = TableBorder.None;
            layout.AddColumn(new TableColumn("").Width(50));
            layout.AddColumn(new TableColumn("").Width(50));
            layout.Title = new TableTitle($"[yellow]Status[/] | Verbindung: {connectionStatus}");
            layout.Caption = new TableTitle("[grey][[1]][/] Zurück zum Hauptmenü");

            layout.AddRow(CreateMessagesTable(), CreateDeathDataTable());

            return layout;
        }

        private Table CreateMessagesTable()
        {
            var table = new Table();
            table.AddColumn("Zeit");
            table.AddColumn("Nachricht");
            table.Title = new TableTitle("[yellow]Live Nachrichten[/]");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var messages = _messageService.GetMessages();

            if (messages.Count == 0)
            {
                table.AddRow("[grey]-[/]", "[grey]Keine Nachrichten vorhanden[/]");
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
            table.AddColumn("Spieler");
            table.AddColumn("Tode");
            table.AddColumn("Verteilung");
            table.Title = new TableTitle("[red]Death Counter[/]");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var deathData = _deathDataService.GetAllDeathData();

            if (deathData.Count == 0)
            {
                table.AddRow("[grey]-[/]", "[grey]-[/]", "[grey]Keine Daten vorhanden[/]");
            }
            else
            {
                var maxDeaths = deathData.Max(d => d.Timestamps.Count);
                const int barWidth = 20;

                foreach (var player in deathData.OrderByDescending(d => d.Timestamps.Count))
                {
                    var deathCount = player.Timestamps.Count;
                    var filledWidth = maxDeaths > 0 ? (int)Math.Round((double)deathCount / maxDeaths * barWidth) : 0;
                    var emptyWidth = barWidth - filledWidth;

                    var bar = $"[red]{new string('#', filledWidth)}[/][grey]{new string('-', emptyWidth)}[/]";

                    table.AddRow(
                        $"[blue]{player.PlayerName}[/]",
                        $"[red]{deathCount}[/]",
                        bar);
                }
            }

            return table;
        }
    }
}
