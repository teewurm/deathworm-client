using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Packets;
using DeathWorm.Utils;

namespace DeathWorm.Extensions
{
    public static class ArchipelagoPacketExtensions
    {
        public static bool TryGetDeathLink(this ArchipelagoPacketBase packet, out WormDeathLink deathLink)
        {
            deathLink = new WormDeathLink("Nobody");

            if (packet is BouncedPacket bouncedPacket && 
                bouncedPacket.Tags.Contains("DeathLink") && 
                WormDeathLink.TryParse(bouncedPacket.Data, out var parsedDeathLink))
            {
                deathLink = parsedDeathLink;
                return true;
            }

            return false;
        }
    }
}
