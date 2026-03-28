using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Packets;
using DeathWorm.Extensions;
using Spectre.Console;

namespace DeathWorm.Services
{
    public record Message(DateTime Timestamp, string Text);

    public class MessageService
    {
        private readonly List<Message> _messages = new();
        private readonly object _messagesLock = new();
        private const int MaxMessages = 10;

        public void AddMessage(string text)
        {
            lock (_messagesLock)
            {
                _messages.Add(new Message(DateTime.Now, text));
                if (_messages.Count > MaxMessages)
                    _messages.RemoveAt(0);
            }
        }

        public void AddMessage(ArchipelagoPacketBase packet, Archipelago.MultiClient.Net.Helpers.IPlayerHelper players, Archipelago.MultiClient.Net.Helpers.IReceivedItemsHelper items)
        {
            if (packet.TryGetDeathLink(out var deathLink))
            {
                AddMessage($"[red]DeathLink von {Markup.Escape(deathLink.Source)}[/] [blue]{Markup.Escape(deathLink.Cause ?? "")}[/]");
                return;
            }

            switch (packet)
            {
                case ConnectedPacket connectedPacket:
                    AddMessage($"[green]Verbunden mit Slot {connectedPacket.Slot}[/]");
                    break;

                case RoomUpdatePacket roomUpdatePacket:
                    if (roomUpdatePacket.Players != null && roomUpdatePacket.Players.Length > 0)
                    {
                        AddMessage($"[yellow]Raum aktualisiert - {roomUpdatePacket.Players.Length} Spieler[/]");
                    }
                    break;

                case ItemPrintJsonPacket itemPrintJsonPacket:
                    
                    //Item name can't be found when sending item to other player
                    //(only when player finds their own items it shows the name)
                    var itemName = items.GetItemName(itemPrintJsonPacket.Item.Item) ?? "Unknonw Item";
                    var receivingPlayer = players.GetPlayerName(itemPrintJsonPacket.ReceivingPlayer) ?? "Unkown Player";
                    var sendingPlayer = players.GetPlayerName(itemPrintJsonPacket.Item.Player) ?? "Unkown Player";

                    AddMessage($"[red]{receivingPlayer}[/] [white]received[/] [blue]{itemName}[/] [white]from[/] [red]{sendingPlayer}[/]");

                    break;

                case PrintJsonPacket printJsonPacket:
                    var text = string.Join("", printJsonPacket.Data.Select(d => d.Text));
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        AddMessage($"[white]{Markup.Escape(text)}[/]");
                    }
                    break;

                case ReceivedItemsPacket receivedItemsPacket:
                    AddMessage($"[cyan]Items erhalten: {receivedItemsPacket.Items.Length} Item(s)[/]");
                    break;

                case LocationInfoPacket locationInfoPacket:
                    AddMessage($"[magenta]Location Info: {locationInfoPacket.Locations.Length} Location(s)[/]");
                    break;

                case DataPackagePacket:
                    AddMessage("[grey]Datenpaket empfangen[/]");
                    break;

                case RoomInfoPacket roomInfoPacket:
                    AddMessage($"[yellow]Raum Info - Seed: {Markup.Escape(roomInfoPacket.SeedName ?? "")}[/]");
                    break;

                case InvalidPacketPacket invalidPacket:
                    AddMessage($"[red]Ungültiges Packet: {Markup.Escape(invalidPacket.ErrorText ?? "")}[/]");
                    break;

                case ConnectionRefusedPacket connectionRefusedPacket:
                    var errors = string.Join(", ", connectionRefusedPacket.Errors.Select(e => Markup.Escape(e.ToString())));
                    AddMessage($"[red]Verbindung abgelehnt: {errors}[/]");
                    break;

                // Ignorierte Packets (zu häufig oder unwichtig)
                case BouncedPacket:
                case RetrievedPacket:
                case SetReplyPacket:
                    break;

                default:
                    // Unbekannte Packets optional loggen
                    AddMessage($"[grey]Packet: {packet.PacketType}[/]");
                    break;
            }
        }

        public List<Message> GetMessages()
        {
            lock (_messagesLock)
            {
                return _messages.ToList();
            }
        }
    }
}
