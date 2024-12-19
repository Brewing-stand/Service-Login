using FluentResults;
using Service_Login.DTOs;
using Service_Login.Models;

namespace Service_Login.Repositories;

public interface IUserServiceRepository
{
    Task<Result<UserResponseDTO>> CheckOrCreateUserAsync(UserRequestDTO userRequestDto);
}