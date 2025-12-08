using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Calorizer.Business.Services
{
    public class LanguageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentLanguage()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Language") ?? "en";
        }

        public void SetCurrentLanguage(string language)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("Language", language);
        }
    }
}