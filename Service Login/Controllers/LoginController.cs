using Microsoft.AspNetCore.Mvc;
using Service_Login.Repositories;

namespace Service_Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IGitLoginRepository _gitLoginRepository;
        private readonly ITokenService _tokenService;

        public LoginController(IGitLoginRepository gitLoginRepository)
        {
            _gitLoginRepository = gitLoginRepository;
        }

        // GET: api/Login/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> Login([FromRoute] string code)
        {
            // Step 1: Retrieve GitHub access token
            var tokenResult = await _gitLoginRepository.Login(code);
            if (tokenResult.IsFailed)
                return Unauthorized("GitHub authentication failed");

            // Step 2: Retrieve GitHub user data
            var userDataResult = await _gitLoginRepository.GetUserData(tokenResult.Value);
            if (userDataResult.IsFailed)
                return NotFound("Failed to retrieve GitHub user data");

            // Step 3: Return GitHub user data
            return Ok(userDataResult.Value); // Return the user data as response
        }
    }
}
