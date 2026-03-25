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
        public const string Exit = "Beenden";

        // Einstellungsmenü
        public const string Server = "Server";
        public const string Port = "Port";
        public const string UserName = "Benutzername";
        public const string GameName = "Spielname";
        public const string Back = "Zurück";
    }
}
