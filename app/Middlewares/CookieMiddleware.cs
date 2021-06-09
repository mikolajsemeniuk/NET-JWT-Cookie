using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace app.Middlewares
{
    public class CookieMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CookieName = "app-cookie";

        public CookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var cookie = context.Request.Cookies[CookieName];

            if (cookie != null)
                if (!context.Request.Headers.ContainsKey("Authorization"))
                    context.Request.Headers.Append("Authorization", "Bearer " + cookie);

            await _next.Invoke(context);
        }
    }
}