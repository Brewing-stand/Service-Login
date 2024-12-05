using System.Net.Http.Headers;
using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Options;
using Service_Login.Models;
using Service_Login.Settings;

namespace Service_Login.Repositories
{
    public class GitLoginRepository : IGitLoginRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GitSecrets _gitSecrets;

        public GitLoginRepository(IHttpClientFactory httpClientFactory, IOptions<GitSecrets> gitSecrets)
        {
            _httpClientFactory = httpClientFactory;
            _gitSecrets = gitSecrets.Value;
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
            const string url = "https://github.com/login/oauth/access_token";

            var parameters = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _gitSecrets.Client),
                new KeyValuePair<string, string>("client_secret", _gitSecrets.Secret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("Accept", "application/json")
            });

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync(url, parameters);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var accessToken = ExtractAccessToken(responseData);

                if (string.IsNullOrEmpty(accessToken))
                    return Result.Fail("Access token not found");

                return Result.Ok(accessToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Result.Fail("Failed to retrieve access token: " + ex);
            }
        }

        public async Task<Result<GitUserData>> GetUserData(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return Result.Fail<GitUserData>("Access token is required");

            const string url = "https://api.github.com/user";
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Brewing Stand");

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var userData = await response.Content.ReadAsStringAsync();

                // Assuming you want to deserialize the response into a GitUserData object
                var gitUserData = DeserializeGitUserData(userData);

                return Result.Ok(gitUserData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Result.Fail<GitUserData>("Failed to retrieve user data: " + ex);
            }
        }

        private static string? ExtractAccessToken(string responseData)
        {
            // Parse the query string response and extract the access token
            var tokenData = System.Web.HttpUtility.ParseQueryString(responseData);
            return tokenData["access_token"];
        }
        
        private static GitUserData DeserializeGitUserData(string userData)
        {
            throw new NotImplementedException();
        }
    }
}
