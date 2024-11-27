﻿using Microsoft.EntityFrameworkCore;

namespace Flavor_Forge.Operations.Services
{
    public interface ICookiesServices
    {
        void SetCookie(string key, string value);
        string? GetCookie(string key);
        void DeleteCookie(string key);
    }
}
