namespace Calorizer.Business.Interfaces
{
    public interface ILocalizationProvider
    {
        string GetValue(string key, string language = "en");
        Dictionary<string, string> GetAllTranslations(string language = "en");
    }
}