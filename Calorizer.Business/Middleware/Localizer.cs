using Calorizer.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Calorizer.Business.Services
{
    public class Localizer
    {
        private readonly ILocalizationProvider _localizationProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Localizer(
            ILocalizationProvider localizationProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _localizationProvider = localizationProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        // Simple indexer to get translation
        public string this[string key]
        {
            get
            {
                var language = GetCurrentLanguage();
                return _localizationProvider.GetValue(key, language);
            }
        }

        // Get current language from session
        private string GetCurrentLanguage()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Language") ?? "en";
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