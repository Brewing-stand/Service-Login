using Refit;
using Service_Login.Models;
using System.Threading.Tasks;
using Service_Login.DTOs;

namespace Service_Login.Services;

public interface IUserServiceApi
{
    // Define the POST method for checking or creating a user
    [Post("/api/User/LoginOrRegisterGit")]
    Task<UserResponseDTO> CheckOrCreateUserAsync([Body] UserRequestDTO userRequestDto);
}

