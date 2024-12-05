using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Options;
using Refit;
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

        // Check or create a user by calling the UserService API
        public async Task<Result<User>> CheckOrCreateUserAsync(GitUserData gitUserData)
        {
            try
            {
                // Call the UserService API to check or create the user
                var user = await _userServiceApi.CheckOrCreateUserAsync(gitUserData);
                return Result.Ok(user);
            }
            catch (ApiException ex)
            {
                // Extract request details
                var requestDetails = JsonSerializer.Serialize(gitUserData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Extract response content
                var responseContent = !string.IsNullOrEmpty(ex.Content) ? ex.Content : "No response content";

                // Log and return error details
                return Result.Fail("Error calling UserService API.")
                    .WithError($"Request Data: {requestDetails}")
                    .WithError($"Status Code: {ex.StatusCode}")
                    .WithError($"Error Message: {responseContent}");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors and return a failure
                return Result.Fail("An unexpected error occurred while communicating with the UserService API.")
                    .WithError(ex.Message);
            }
        }
    }
