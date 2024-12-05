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
        private readonly IUserServiceRepository _userServiceRepository;

        public LoginController(IGitLoginRepository gitLoginRepository, IUserServiceRepository userServiceRepository, ITokenService tokenService)
        {
            _gitLoginRepository = gitLoginRepository;
            _userServiceRepository = userServiceRepository;
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

            // Step 3: Call UserService to check or create the user
            var userResult = await _userServiceRepository.CheckOrCreateUserAsync(gitUserData);
            if (userResult.IsFailed)
            {
                // Log or use the detailed error messages in userResult.Errors
                return BadRequest(new
                {
                    Message = "Error checking or creating user.",
                    Details = userResult.Errors.Select(e => e.Message) // Return all error messages
                });
            }

            return Ok(userResult);
        }
    }
}
