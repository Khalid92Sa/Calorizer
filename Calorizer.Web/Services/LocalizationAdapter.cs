using Calorizer.Business.Interfaces;

namespace Calorizer.Web.Services
{
    public class LocalizationAdapter : ILocalizationProvider
    {
        private readonly ILocalizationService _localizationService;

        public LocalizationAdapter(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public string GetValue(string key, string language = "en")
        {
            return _localizationService.GetValue(key, language);
        }

        public Dictionary<string, string> GetAllTranslations(string language = "en")
        {
            return _localizationService.GetAllTranslations(language);
        }
    }
}