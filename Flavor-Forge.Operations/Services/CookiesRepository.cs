using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Flavor_Forge.Operations.Services
{
    public class CookiesRepository : ICookiesServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookiesRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
        
        public string? GetCookie(string key)
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[key];
        }

        public void DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
