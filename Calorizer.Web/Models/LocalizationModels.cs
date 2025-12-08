namespace Calorizer.Web.Models
{
    public class LocalizationEntry
    {
        public string ValueEn { get; set; } = string.Empty;
        public string ValueAr { get; set; } = string.Empty;
    }

    public class LocalizationData
    {
        public Dictionary<string, LocalizationEntry> Translations { get; set; } = new();
    }
}