using System.Net.Http.Headers;
using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Options;
using Refit;
using Service_Login.Models;
using Service_Login.Services;
using Service_Login.Settings;

namespace Service_Login.Repositories
{
    public class GitLoginRepository : IGitLoginRepository
    {
        private readonly GitSecrets _gitSecrets;
        private readonly IGitHubAuthApi _gitHubAuthApi; // For OAuth access token
        private readonly IGitHubServiceApi _gitHubApi;  

        public GitLoginRepository(IOptions<GitSecrets> gitSecrets, IGitHubAuthApi gitHubAuthApi, IGitHubServiceApi gitHubApi)
        {
            _gitSecrets = gitSecrets.Value;
            _gitHubAuthApi = gitHubAuthApi;
            _gitHubApi = gitHubApi;
        }

        public async Task<Result<string>> Login(string code)
        {
            var tokenResult = await RetrieveAccessToken(code);

            if (tokenResult.IsFailed || string.IsNullOrEmpty(tokenResult.Value))
                return Result.Fail("Login failed");

            return Result.Ok(tokenResult.Value);
        }

        private async Task<Result<string>> RetrieveAccessToken(string code)
        {
            try
            {
                // Use Refit to retrieve the access token from github.com
                var response = await _gitHubAuthApi.GetAccessTokenAsync(
                    _gitSecrets.Client,
                    _gitSecrets.Secret,
                    code,
                    "application/json"
                );
                
                // Parse the query string and extract the access_token
                var accessToken = ExtractAccessToken(response);

                if (string.IsNullOrEmpty(accessToken))
                    return Result.Fail("Access token not found");

                return Result.Ok(accessToken);
            }
            catch (ApiException ex)
            {
                return Result.Fail($"Failed to retrieve access token: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result<GitUserData>> GetUserData(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return Result.Fail("Access token is required");

            try
            {
                // Call the GitHub API to get user data from api.github.com
                var userData = await _gitHubApi.GetUserDataAsync($"Bearer {accessToken}");
                
                return Result.Ok(userData);  // Return raw JSON response
            }
            catch (ApiException ex)
            {
                return Result.Fail($"Failed to retrieve user data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"An unexpected error occurred: {ex.Message}");
            }
        }

        private static string? ExtractAccessToken(string responseData)
        {
            // Parse the query string response and extract the access token
            var tokenData = System.Web.HttpUtility.ParseQueryString(responseData);
            return tokenData["access_token"];
        }
    }
}
