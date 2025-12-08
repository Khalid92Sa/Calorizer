namespace Calorizer.Business.Interfaces
{
    public interface ILocalizationService
    {
        string GetValue(string key, string language = "en");
        Dictionary<string, string> GetAllTranslations(string language = "en");
        void ReloadTranslations();
    }
}