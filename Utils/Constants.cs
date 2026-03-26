namespace DeathWorm.Utils
{
    public static class Constants
    {
        public static readonly string SettingsFile = "settings.json";
        public static readonly string DeathDataFile = "deathdata.json";
        public static readonly string OtherDeathDataFolder = "other_death_data";
    }

    public static class TranslationKeys
    {
        // Hauptmenü
        public const string EditSettings = "menu.edit_settings";
        public const string Connect = "menu.connect";
        public const string ShowStatus = "menu.show_status";
        public const string SendDeathLink = "menu.send_deathlink";
        public const string SendChat = "menu.send_chat";
        public const string Exit = "menu.exit";

        // Einstellungsmenü
        public const string Server = "settings.server";
        public const string Port = "settings.port";
        public const string UserName = "settings.username";
        public const string GameName = "settings.gamename";
        public const string Language = "settings.language";
        public const string Back = "menu.back";

        // Bestätigung
        public const string Yes = "confirm.yes";
        public const string No = "confirm.no";

        // Titel
        public const string MainMenuTitle = "menu.main_title";
        public const string SettingsMenuTitle = "menu.settings_title";
        public const string SettingsTableTitle = "settings.table_title";
        public const string SettingsTableValue = "settings.table_value";

        // Verbindung
        public const string Connecting = "connection.connecting";
        public const string ConnectionSuccess = "connection.success";
        public const string ConnectionFailed = "connection.failed";
        public const string AlreadyConnected = "connection.already_connected";
        public const string NotConnected = "connection.not_connected";
        public const string Connected = "status.connected";
        public const string Disconnected = "status.disconnected";

        // DeathLink
        public const string DeathLinkConfirm = "deathlink.confirm";
        public const string DeathLinkSent = "deathlink.sent";
        public const string DeathLinkCancelled = "deathlink.cancelled";
        public const string DeathLinkMessagePrompt = "deathlink.message_prompt";

        // Chat
        public const string ChatMessagePrompt = "chat.message_prompt";
        public const string ChatSent = "chat.sent";
        public const string ChatCancelled = "chat.cancelled";

        // Allgemein
        public const string PressKeyToContinue = "general.press_key";
        public const string NoMessages = "general.no_messages";
        public const string NoData = "general.no_data";
        public const string Time = "general.time";
        public const string Message = "general.message";

        // Status View
        public const string StatusTitle = "status.title";
        public const string LiveMessages = "status.live_messages";
        public const string DeathCounter = "status.death_counter";
        public const string Player = "status.player";
        public const string Deaths = "status.deaths";
        public const string Distribution = "status.distribution";
        public const string Connection = "status.connection";
        public const string BackToMainMenu = "status.back_to_main";

        // Death Data
        public const string ClearDeathData = "deathdata.clear";
        public const string ClearDeathDataConfirm = "deathdata.clear_confirm";
        public const string ClearDeathDataSuccess = "deathdata.clear_success";
        public const string ClearDeathDataCancelled = "deathdata.clear_cancelled";
        public const string MergeDeathData = "deathdata.merge";
        public const string MergeDeathDataPrompt = "deathdata.merge_prompt";
        public const string MergeDeathDataSuccess = "deathdata.merge_success";
        public const string MergeDeathDataCancelled = "deathdata.merge_cancelled";
        public const string MergeDeathDataError = "deathdata.merge_error";
        public const string MergeDeathDataFileNotFound = "deathdata.merge_file_not_found";
    }

    public static class Translations
    {
        public static readonly HashSet<string> SupportedLanguages = ["de", "en"];

        public static readonly Dictionary<string, Dictionary<string, string>> Texts = new()
        {
            // Hauptmenü
            [TranslationKeys.EditSettings] = new() { ["de"] = "Einstellungen bearbeiten", ["en"] = "Edit Settings" },
            [TranslationKeys.Connect] = new() { ["de"] = "Verbinden", ["en"] = "Connect" },
            [TranslationKeys.ShowStatus] = new() { ["de"] = "Status anzeigen", ["en"] = "Show Status" },
            [TranslationKeys.SendDeathLink] = new() { ["de"] = "Death Link senden", ["en"] = "Send Death Link" },
            [TranslationKeys.SendChat] = new() { ["de"] = "Chat senden", ["en"] = "Send Chat" },
            [TranslationKeys.Exit] = new() { ["de"] = "Beenden", ["en"] = "Exit" },

            // Einstellungsmenü
            [TranslationKeys.Server] = new() { ["de"] = "Server", ["en"] = "Server" },
            [TranslationKeys.Port] = new() { ["de"] = "Port", ["en"] = "Port" },
            [TranslationKeys.UserName] = new() { ["de"] = "Benutzername", ["en"] = "Username" },
            [TranslationKeys.GameName] = new() { ["de"] = "Spielname", ["en"] = "Game Name" },
            [TranslationKeys.Language] = new() { ["de"] = "Sprache", ["en"] = "Language" },
            [TranslationKeys.Back] = new() { ["de"] = "Zurück", ["en"] = "Back" },

            // Bestätigung
            [TranslationKeys.Yes] = new() { ["de"] = "Ja", ["en"] = "Yes" },
            [TranslationKeys.No] = new() { ["de"] = "Nein", ["en"] = "No" },

            // Titel
            [TranslationKeys.MainMenuTitle] = new() { ["de"] = "Hauptmenü", ["en"] = "Main Menu" },
            [TranslationKeys.SettingsMenuTitle] = new() { ["de"] = "Welche Einstellung möchtest du ändern?", ["en"] = "Which setting do you want to change?" },
            [TranslationKeys.SettingsTableTitle] = new() { ["de"] = "Einstellung", ["en"] = "Setting" },
            [TranslationKeys.SettingsTableValue] = new() { ["de"] = "Wert", ["en"] = "Value" },

            // Verbindung
            [TranslationKeys.Connecting] = new() { ["de"] = "Verbinde mit {0}:{1}...", ["en"] = "Connecting to {0}:{1}..." },
            [TranslationKeys.ConnectionSuccess] = new() { ["de"] = "Erfolgreich verbunden!", ["en"] = "Successfully connected!" },
            [TranslationKeys.ConnectionFailed] = new() { ["de"] = "Verbindung fehlgeschlagen!", ["en"] = "Connection failed!" },
            [TranslationKeys.AlreadyConnected] = new() { ["de"] = "Bereits verbunden", ["en"] = "Already connected" },
            [TranslationKeys.NotConnected] = new() { ["de"] = "Nicht verbunden. Bitte zuerst verbinden.", ["en"] = "Not connected. Please connect first." },
            [TranslationKeys.Connected] = new() { ["de"] = "Verbunden", ["en"] = "Connected" },
            [TranslationKeys.Disconnected] = new() { ["de"] = "Nicht verbunden", ["en"] = "Disconnected" },

            // DeathLink
            [TranslationKeys.DeathLinkConfirm] = new() { ["de"] = "Möchtest du wirklich einen Death Link senden?", ["en"] = "Do you really want to send a Death Link?" },
            [TranslationKeys.DeathLinkSent] = new() { ["de"] = "Death Link gesendet!", ["en"] = "Death Link sent!" },
            [TranslationKeys.DeathLinkCancelled] = new() { ["de"] = "Death Link abgebrochen.", ["en"] = "Death Link cancelled." },
            [TranslationKeys.DeathLinkMessagePrompt] = new() { ["de"] = "Nachricht (leer für zufällige Nachricht):", ["en"] = "Message (empty for random message):" },

            // Chat
            [TranslationKeys.ChatMessagePrompt] = new() { ["de"] = "Chat-Nachricht (leer zum Abbrechen):", ["en"] = "Chat message (empty to cancel):" },
            [TranslationKeys.ChatSent] = new() { ["de"] = "Chat-Nachricht gesendet!", ["en"] = "Chat message sent!" },
            [TranslationKeys.ChatCancelled] = new() { ["de"] = "Chat abgebrochen.", ["en"] = "Chat cancelled." },

            // Allgemein
            [TranslationKeys.PressKeyToContinue] = new() { ["de"] = "Drücke eine Taste um fortzufahren...", ["en"] = "Press any key to continue..." },
            [TranslationKeys.NoMessages] = new() { ["de"] = "Keine Nachrichten vorhanden", ["en"] = "No messages available" },
            [TranslationKeys.NoData] = new() { ["de"] = "Keine Daten vorhanden", ["en"] = "No data available" },
            [TranslationKeys.Time] = new() { ["de"] = "Zeit", ["en"] = "Time" },
            [TranslationKeys.Message] = new() { ["de"] = "Nachricht", ["en"] = "Message" },

            // Status View
            [TranslationKeys.StatusTitle] = new() { ["de"] = "Status", ["en"] = "Status" },
            [TranslationKeys.LiveMessages] = new() { ["de"] = "Live Nachrichten", ["en"] = "Live Messages" },
            [TranslationKeys.DeathCounter] = new() { ["de"] = "Death Counter", ["en"] = "Death Counter" },
            [TranslationKeys.Player] = new() { ["de"] = "Spieler", ["en"] = "Player" },
            [TranslationKeys.Deaths] = new() { ["de"] = "Tode", ["en"] = "Deaths" },
            [TranslationKeys.Distribution] = new() { ["de"] = "Verteilung", ["en"] = "Distribution" },
            [TranslationKeys.Connection] = new() { ["de"] = "Verbindung", ["en"] = "Connection" },
            [TranslationKeys.BackToMainMenu] = new() { ["de"] = "Zurück zum Hauptmenü", ["en"] = "Back to Main Menu" },

            // Death Data
            [TranslationKeys.ClearDeathData] = new() { ["de"] = "Death-Daten löschen", ["en"] = "Clear Death Data" },
            [TranslationKeys.ClearDeathDataConfirm] = new() { ["de"] = "Möchtest du wirklich alle Death-Daten löschen?", ["en"] = "Do you really want to clear all death data?" },
            [TranslationKeys.ClearDeathDataSuccess] = new() { ["de"] = "Death-Daten wurden gelöscht!", ["en"] = "Death data has been cleared!" },
            [TranslationKeys.ClearDeathDataCancelled] = new() { ["de"] = "Löschen abgebrochen.", ["en"] = "Clear cancelled." },
            [TranslationKeys.MergeDeathData] = new() { ["de"] = "Death-Daten zusammenführen", ["en"] = "Merge Death Data" },
            [TranslationKeys.MergeDeathDataPrompt] = new() { ["de"] = "Möchtest du die Dateien aus folgendem Ordner zusammenführen?\n{0}", ["en"] = "Do you want to merge the files from the following folder?\n{0}" },
            [TranslationKeys.MergeDeathDataSuccess] = new() { ["de"] = "Death-Daten wurden zusammengeführt! ({0} Datei(en))", ["en"] = "Death data has been merged! ({0} file(s))" },
            [TranslationKeys.MergeDeathDataCancelled] = new() { ["de"] = "Zusammenführen abgebrochen.", ["en"] = "Merge cancelled." },
            [TranslationKeys.MergeDeathDataError] = new() { ["de"] = "Fehler beim Lesen der Datei: {0}", ["en"] = "Error reading file: {0}" },
            [TranslationKeys.MergeDeathDataFileNotFound] = new() { ["de"] = "Keine JSON-Dateien im Ordner gefunden:\n{0}", ["en"] = "No JSON files found in folder:\n{0}" }
        };
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
