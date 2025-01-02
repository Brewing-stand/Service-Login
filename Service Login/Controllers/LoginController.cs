using Microsoft.AspNetCore.Mvc;
using Service_Login.Models;
using Service_Login.Repositories;

namespace Service_Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IGitLoginRepository _gitLoginRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public LoginController(IGitLoginRepository gitLoginRepository, IUserRepository userRepository, ITokenService tokenService)
        {
            _gitLoginRepository = gitLoginRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        // GET: api/Login/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> Login([FromRoute] string code)
        {
            // Step 1: Retrieve GitHub access token
            var tokenResult = await _gitLoginRepository.Login(code);
            if (tokenResult.IsFailed)
                return Unauthorized(tokenResult.Errors.FirstOrDefault());

            // Step 2: Retrieve GitHub user data
            var userDataResult = await _gitLoginRepository.GetUserData(tokenResult.Value);
            if (userDataResult.IsFailed)
                return NotFound(userDataResult.Errors.FirstOrDefault());

            var gitUserData = userDataResult.Value;

            // Step 3: Call check or create the user in the database
            var userData = new User();
            
            userData.gitId = gitUserData.id;
            userData.username = gitUserData.login;
            userData.avatar = gitUserData.avatar_url;
            
            var userResult = await _userRepository.CheckOrCreateUserAsync(userData);
            if (userResult.IsFailed)
            {
                return BadRequest(new
                {
                    Message = "Error checking or creating user.",
                    Details = userResult.Errors.Select(e => e.Message) // Detailed error messages
                });
            }
            
            // Step 4: Generate the JWT token
            var user = userResult.Value;
            var jwtToken = _tokenService.GenerateToken(user);

            // Step 5: Set the token in the cookie
            Response.Cookies.Append("jwt", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,  // Make sure the connection is HTTPS in production
                SameSite = SameSiteMode.None, // Required for cross-origin cookies
                Expires = DateTime.UtcNow.AddDays(1),
            });
            
            // Step 6: Return user data and JWT
            return Ok(new
            {
                User = new
                {
                    user.id,
                    user.username,
                    user.avatar
                },
            });
        }
    }
}
