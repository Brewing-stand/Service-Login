using Microsoft.AspNetCore.Mvc;
using Service_Login.DTOs;
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
            var userRequestDto = new UserRequestDTO();
            
            userRequestDto.GitId = gitUserData.id;
            userRequestDto.Username = gitUserData.login;
            userRequestDto.Avatar = gitUserData.avatar_url;
            
            var userResult = await _userServiceRepository.CheckOrCreateUserAsync(userRequestDto);
            if (userResult.IsFailed)
            {
                return BadRequest(new
                {
                    Message = "Error checking or creating user.",
                    Details = userResult.Errors.Select(e => e.Message) // Detailed error messages
                });
            }

            var userResultData = userResult.Value;

            // Step 4: Generate the JWT token
            var user = new User();

            user.Id = userResultData.Id;
            user.GitId = userResultData.GitId;
            user.Username = userResultData.Username;
            user.Avatar = userResultData.Avatar;
            
            var jwtToken = _tokenService.GenerateToken(user); // Assuming `GenerateToken` accepts user data and returns a JWT token

            // Step 5: Return user data and JWT
            return Ok(new
            {
                User = user,  // Return user data
                Token = jwtToken          // Return JWT token
            });
        }
    }
}
