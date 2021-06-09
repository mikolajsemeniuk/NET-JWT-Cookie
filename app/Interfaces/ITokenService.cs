using System;
using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.Http;

namespace app.Interfaces
{
    public interface ITokenService
    {
        Task<Tuple<string, string, CookieOptions>> CreateJWTCookieAsync(User user);
        Task<string> CreateJWTTokenAsync(User user);
        Tuple<string, string, CookieOptions> RemoveJWTCookie();
    }
}