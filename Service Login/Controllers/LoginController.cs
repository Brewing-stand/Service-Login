using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Service_Template.Repositories;
using Service_Template.Settings;

namespace Service_Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IGitLogin _gitLogin;
        
        public LoginController(IGitLogin gitLogin)
        {
            _gitLogin = gitLogin;
        }
        
        // GET: api/Login/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> Login([FromRoute] string code)
        {
            var tokenResult = await _gitLogin.Login(code);

            if (tokenResult.IsFailed)
            {
                return Unauthorized();
            }
            
            // Retrieve GitHub user data using the access token
            Result<string> userDataResult = await _gitLogin.GetUserData(tokenResult.Value);
        
            // Check if userdata
            if (userDataResult.IsFailed)
            {
                return NotFound("Failed to retrieve user data");
            }
            
            SetAuthCookie("access_token",tokenResult.Value);

            return Ok(userDataResult.Value);
        }

        private void SetAuthCookie(string key, string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,       // Use HTTPS
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append(key, token, cookieOptions);
        }
    }
}
