using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Options;
using Refit;
using Service_Login.DTOs;
using Service_Login.Models;
using Service_Login.Services;
using Service_Login.Settings;

namespace Service_Login.Repositories;


public class UserServiceRepository : IUserServiceRepository
{
    private readonly IUserServiceApi _userServiceApi;

    public UserServiceRepository(IUserServiceApi userServiceApi)
    {
        _userServiceApi = userServiceApi;
    }

    public async Task<Result<UserResponseDTO>> CheckOrCreateUserAsync(UserRequestDTO userRequestDto)
    {
        try
        {
            // Call the UserService API to check or create the user
            var user = await _userServiceApi.CheckOrCreateUserAsync(userRequestDto);
            return Result.Ok(user);
        }
        catch (ApiException ex)
        {
            return Result.Fail("An error occurred while calling the UserService API.")
                .WithError(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Fail("An unexpected error occurred while communicating with the UserService API.")
                .WithError(ex.Message);
        }
    }
}
