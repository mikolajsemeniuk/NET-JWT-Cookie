using System.Threading.Tasks;
using app.Interfaces;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _service;

        public AuthController(ITokenService service)
        {
            _service = service;
        }

        [HttpGet("token")]
        public async Task<ActionResult<string>> GetToken() => 
            Ok(new 
            {
                Token = await _service.CreateJWTTokenAsync(new User 
                { 
                    Id = 1,
                    UserName = "adam",
                    Email = "adam@mock.com"
                })
            });

        [HttpGet("cookie")]
        public async Task<ActionResult> GetCookie()
        {
            var cookieResult = await _service.CreateJWTCookieAsync(new User 
            { 
                Id = 1,
                UserName = "adam",
                Email = "adam@mock.com"
            });
            Response.Cookies.Append(cookieResult.Item1, cookieResult.Item2, cookieResult.Item3);
            return Ok(new { Message = "you were authenticated" });
        }

        [HttpGet("remove1")]
        public ActionResult RemoveCookie1()
        {
            var cookieResult = _service.RemoveJWTCookie();
            Response.Cookies.Append(cookieResult.Item1, cookieResult.Item2, cookieResult.Item3);
            return Ok(new { Message = "you were successfully logged out" });
        }

        [HttpGet("remove2")]
        public ActionResult RemoveCookie2()
        {
            Response.Cookies.Delete("app-cookie");
            return Ok(new { Message = "you were successfully logged out" });
        }

        [Authorize]
        [HttpGet("auth")]
        public string Authorized() => "only authenticated users can see this content";

        [AllowAnonymous]
        [HttpGet("unauth")]
        public string UnAuthorized() => "anyone can see this content";
    }
}
