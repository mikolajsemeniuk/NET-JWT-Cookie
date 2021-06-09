using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using app.Interfaces;
using app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace app.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private UserManager<User> _userManager;
        private const string CookieName = "app-cookie";

        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }

        // should be private only for demonstration purpose
        public async Task<string> CreateJWTTokenAsync(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Tuple<string, string, CookieOptions>> CreateJWTCookieAsync(User user)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddHours(1);
            options.HttpOnly = true;
            options.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Secure = true;

            return Tuple.Create(
                CookieName,
                await CreateJWTTokenAsync(user),
                options);
        }

        public Tuple<string, string, CookieOptions> RemoveJWTCookie()
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddSeconds(1);
            options.HttpOnly = true;
            options.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Secure = true;

            return Tuple.Create(
                CookieName,
                "",
                options);
        }
    }
}