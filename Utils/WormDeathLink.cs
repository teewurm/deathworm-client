using Archipelago.MultiClient.Net.Converters;
using Newtonsoft.Json.Linq;

namespace DeathWorm.Utils
{
    public class WormDeathLink : IEquatable<WormDeathLink>
    {
        //
        // Zusammenfassung:
        //     The Timestamp of the created DeathLink object
        public DateTime Timestamp { get; internal set; }

        //
        // Zusammenfassung:
        //     The name of the player who sent the DeathLink
        public string Source { get; }

        //
        // Zusammenfassung:
        //     The full text to print for players receiving the DeathLink. Can be null
        public string Cause { get; }

        //
        // Zusammenfassung:
        //     A DeathLink object that gets sent and received via bounce packets.
        //
        // Parameter:
        //   sourcePlayer:
        //     Name of the player sending the DeathLink
        //
        //   cause:
        //     Optional reason for the DeathLink. Since this is optional it should generally
        //     include a name as if this entire text is what will be displayed
        public WormDeathLink(string sourcePlayer, string cause = null)
        {
            Timestamp = DateTime.UtcNow;
            Source = sourcePlayer;
            Cause = cause;
        }

        internal static bool TryParse(Dictionary<string, JToken> data, out WormDeathLink deathLink)
        {
            try
            {
                if (!data.TryGetValue("time", out var value) || !data.TryGetValue("source", out var value2))
                {
                    deathLink = null;
                    return false;
                }

                string cause = null;
                if (data.TryGetValue("cause", out var value3))
                {
                    cause = value3.ToString();
                }

                deathLink = new WormDeathLink(value2.ToString(), cause)
                {
                    Timestamp = UnixTimeConverter.UnixTimeStampToDateTime(value.ToObject<double>())
                };
                return true;
            }
            catch
            {
                deathLink = null;
                return false;
            }
        }

        public bool Equals(WormDeathLink other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if ((object)this == other)
            {
                return true;
            }

            if (Source == other.Source && Timestamp.Date.Equals(other.Timestamp.Date) && Timestamp.Hour == other.Timestamp.Hour && Timestamp.Minute == other.Timestamp.Minute)
            {
                return Timestamp.Second == other.Timestamp.Second;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((WormDeathLink)obj);
        }

        public override int GetHashCode()
        {
            return (Timestamp.GetHashCode() * 397) ^ ((Source != null) ? Source.GetHashCode() : 0);
        }

        public static bool operator ==(WormDeathLink lhs, WormDeathLink rhs)
        {
            return lhs?.Equals(rhs) ?? ((object)rhs == null);
        }

        public static bool operator !=(WormDeathLink lhs, WormDeathLink rhs)
        {
            return !(lhs == rhs);
        }
    }
}
