namespace DeathWorm.Utils
{
    public static class Constants
    {
        public static readonly string SettingsFile = "settings.json";
        public static readonly string DeathDataFile = "deathdata.json";
    }

    public static class MenuChoices
    {
        // Hauptmenü
        public const string EditSettings = "Einstellungen bearbeiten";
        public const string Connect = "Verbinden";
        public const string ShowStatus = "Status anzeigen";
        public const string SendDeathLink = "Death Link senden";
        public const string SendChat = "Chat senden";
        public const string Exit = "Beenden";

        // Einstellungsmenü
        public const string Server = "Server";
        public const string Port = "Port";
        public const string UserName = "Benutzername";
        public const string GameName = "Spielname";
        public const string Back = "Zurück";

        // Bestätigung
        public const string Yes = "Ja";
        public const string No = "Nein";
    }

    public static class DeathLinkMessages
    {
        private static readonly string[] Messages =
        [
            "ist gestorben",
            "hat das Zeitliche gesegnet",
            "wurde vom Schicksal ereilt",
            "hat den Löffel abgegeben",
            "ist über den Jordan gegangen",
            "hat ins Gras gebissen",
            "wurde aus dem Leben gerissen",
            "hat die Radieschen von unten betrachtet",
            "ist nicht mehr unter uns",
            "hat den ultimativen Ragequit gemacht",
            "wurde vom RNG zerstört",
            "hat vergessen zu heilen",
            "dachte, Fall-Schaden wäre optional",
            "hat die Falle nicht gesehen",
            "wurde von einem Creeper überrascht"
        ];

        private static readonly Random _random = new();

        public static string GetRandomMessage()
        {
            return Messages[_random.Next(Messages.Length)];
        }
    }
}
