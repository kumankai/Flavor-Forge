/*
 * This service is for handling cookies
 */

using Microsoft.EntityFrameworkCore;

namespace Flavor_Forge.Operations.Services.Service
{
    public interface ICookiesServices
    {
        void SetCookie(string key, string value);
        string? GetCookie(string key);
        void DeleteCookie(string key);
    }
}
