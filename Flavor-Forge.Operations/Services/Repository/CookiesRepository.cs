/*
 * This repository defines cookie handling services
 */

using Flavor_Forge.Operations.Services.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Flavor_Forge.Operations.Services.Repository
{
    public class CookiesRepository : ICookiesServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookiesRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Sets cookie of key to value
        public void SetCookie(string key, string value)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
                // No expiry
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
        }

        // Retrieves cookie value with key
        public string? GetCookie(string key)
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[key];
        }

        // Deletes cookie
        public void DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
