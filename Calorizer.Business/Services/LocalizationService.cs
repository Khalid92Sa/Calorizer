using System.Text.Json;
using Calorizer.Business.Interfaces;
using Calorizer.Business.Models;
using Microsoft.Extensions.Logging;

namespace Calorizer.Business.Services
{
    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, LocalizationEntry> _translations;
        private readonly string _jsonFilePath;
        private readonly ILogger<LocalizationService>? _logger;

        public LocalizationService(string jsonFilePath, ILogger<LocalizationService>? logger = null)
        {
            _logger = logger;
            _jsonFilePath = jsonFilePath;
            _translations = new Dictionary<string, LocalizationEntry>();
            LoadTranslations();
        }

        private void LoadTranslations()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    var json = File.ReadAllText(_jsonFilePath);

                    // Try to deserialize directly as the dictionary
                    var tempTranslations = JsonSerializer.Deserialize<Dictionary<string, LocalizationEntry>>(json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            ReadCommentHandling = JsonCommentHandling.Skip,
                            AllowTrailingCommas = true
                        });

                    if (tempTranslations != null)
                    {
                        _translations = tempTranslations;
                        _logger?.LogInformation($"Loaded {_translations.Count} translations from {_jsonFilePath}");

                        // Debug: Log first few keys
                        foreach (var key in _translations.Keys.Take(3))
                        {
                            _logger?.LogInformation($"Sample key: {key}, ValueEn: {_translations[key].ValueEn}, ValueAr: {_translations[key].ValueAr}");
                        }
                    }
                    else
                    {
                        _logger?.LogWarning($"Failed to deserialize translations from {_jsonFilePath}");
                    }
                }
                else
                {
                    _logger?.LogWarning($"Localization file not found: {_jsonFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading translations");
                _translations = new Dictionary<string, LocalizationEntry>();
            }
        }

        public string GetValue(string key, string language = "en")
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger?.LogWarning("GetValue called with empty key");
                return string.Empty;
            }

            if (_translations.TryGetValue(key, out var entry))
            {
                var value = language.ToLower() switch
                {
                    "ar" => entry.ValueAr,
                    "en" => entry.ValueEn,
                    _ => entry.ValueEn
                };

                _logger?.LogDebug($"GetValue: Key={key}, Language={language}, Value={value}");
                return value;
            }

            _logger?.LogWarning($"Translation key not found: {key}, Language: {language}");
            return key; // Return the key itself if not found
        }

        public Dictionary<string, string> GetAllTranslations(string language = "en")
        {
            var result = new Dictionary<string, string>();

            foreach (var kvp in _translations)
            {
                result[kvp.Key] = language.ToLower() switch
                {
                    "ar" => kvp.Value.ValueAr,
                    "en" => kvp.Value.ValueEn,
                    _ => kvp.Value.ValueEn
                };
            }

            return result;
        }

        public void ReloadTranslations()
        {
            LoadTranslations();
        }
    }
}