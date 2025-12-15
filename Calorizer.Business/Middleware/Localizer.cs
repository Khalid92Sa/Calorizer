using Calorizer.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Calorizer.Business.Services
{
    public class Localizer
    {
        private readonly ILocalizationProvider _localizationProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Localizer>? _logger;

        public Localizer(
            ILocalizationProvider localizationProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<Localizer>? logger = null)
        {
            _localizationProvider = localizationProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Simple indexer to get translation
        public string this[string key]
        {
            get
            {
                var language = GetCurrentLanguage();
                _logger?.LogDebug($"Localizer indexer called: Key={key}, Language={language}");

                var value = _localizationProvider.GetValue(key, language);

                _logger?.LogDebug($"Localizer result: Key={key}, Value={value}");

                // If value is null, empty, or same as key, return the key itself
                return string.IsNullOrEmpty(value) ? key : value;
            }
        }

        // Get current language from session
        private string GetCurrentLanguage()
        {
            try
            {
                var language = _httpContextAccessor.HttpContext?.Session.GetString("Language") ?? "en";
                _logger?.LogDebug($"Current language from session: {language}");
                return language;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Could not access session, defaulting to 'en'");
                return "en";
            }
        }

        // Optional: Get all translations for current language
        public Dictionary<string, string> GetAll()
        {
            var language = GetCurrentLanguage();
            return _localizationProvider.GetAllTranslations(language);
        }

        // Optional: Get translation with fallback
        public string Get(string key, string fallback = "")
        {
            var value = this[key];
            return string.IsNullOrEmpty(value) || value == key ? fallback : value;
        }
    }
}