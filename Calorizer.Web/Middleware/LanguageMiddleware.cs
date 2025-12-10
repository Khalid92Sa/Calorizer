using Microsoft.AspNetCore.Http;

namespace Calorizer.Web.Middleware
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if language is already set in session
            var sessionLanguage = context.Session.GetString("Language");

            if (string.IsNullOrEmpty(sessionLanguage))
            {
                // Try to get from query string (e.g., ?lang=ar)
                var queryLanguage = context.Request.Query["lang"].ToString();

                if (!string.IsNullOrEmpty(queryLanguage) && (queryLanguage == "en" || queryLanguage == "ar"))
                {
                    context.Session.SetString("Language", queryLanguage);
                }
                else
                {
                    // Try to get from cookie
                    var cookieLanguage = context.Request.Cookies["Language"];

                    if (!string.IsNullOrEmpty(cookieLanguage) && (cookieLanguage == "en" || cookieLanguage == "ar"))
                    {
                        context.Session.SetString("Language", cookieLanguage);
                    }
                    else
                    {
                        // Try to get from Accept-Language header
                        var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
                        var language = acceptLanguage.StartsWith("ar") ? "ar" : "en";
                        context.Session.SetString("Language", language);
                    }
                }
            }

            await _next(context);
        }
    }
}