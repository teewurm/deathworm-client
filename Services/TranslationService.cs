using DeathWorm.Repositories;
using DeathWorm.Utils;

namespace DeathWorm.Services
{
    public class TranslationService
    {
        private readonly SettingsRepository _settingsRepository;
        private string _currentLanguage;

        public string CurrentLanguage => _currentLanguage;

        public TranslationService(SettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            var settings = _settingsRepository.Load();
            _currentLanguage = Translations.SupportedLanguages.Contains(settings.Language) 
                ? settings.Language 
                : "en";
        }

        public void SetLanguage(string languageCode)
        {
            if (Translations.SupportedLanguages.Contains(languageCode))
            {
                _currentLanguage = languageCode;
                var settings = _settingsRepository.Load();
                settings.Language = languageCode;
                _settingsRepository.Save(settings);
            }
        }

        public string Get(string key)
        {
            if (Translations.Texts.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(_currentLanguage, out var text))
                {
                    return text;
                }

                // Fallback auf Englisch
                if (translations.TryGetValue("en", out var fallback))
                {
                    return fallback;
                }
            }

            return key;
        }

        public string Get(string key, params object[] args)
        {
            var text = Get(key);
            return string.Format(text, args);
        }
    }
}
