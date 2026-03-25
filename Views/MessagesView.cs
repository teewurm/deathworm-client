using DeathWorm.Services;
using DeathWorm.Clients;
using DeathWorm.Services;
using Spectre.Console;

namespace DeathWorm.Views
{
    public class MessagesView
    {
        private readonly MessageService _messageService;
        private readonly ArchipelagoClientService _archipelagoClient;

        public MessagesView(MessageService messageService, ArchipelagoClientService archipelagoClient)
        {
            _messageService = messageService;
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

        private Table CreateLayout()
        {
            var connectionStatus = _archipelagoClient.IsConnected
                ? "[green]Verbunden[/]"
                : "[red]Nicht verbunden[/]";

            var table = new Table();
            table.AddColumn("Zeit");
            table.AddColumn("Nachricht");
            table.Title = new TableTitle($"[yellow]Live Nachrichten[/] | Status: {connectionStatus}");
            table.Caption = new TableTitle("[grey][[1]][/] Zurück zum Hauptmenü");
            table.Border = TableBorder.Rounded;

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
    }
}
