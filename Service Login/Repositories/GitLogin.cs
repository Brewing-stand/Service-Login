using System.Net.Http.Headers;
using System.Text.Json;
using FluentResults;

namespace Service_Template.Repositories;
using Microsoft.Extensions.Options;
using Service_Template.Settings;

public class GitLogin : IGitLogin
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GitSecrets _gitSecrets;

    public GitLogin(IHttpClientFactory httpClientFactory, IOptions<GitSecrets> gitSecret)
    {
        _httpClientFactory = httpClientFactory;
            
        _gitSecrets = gitSecret.Value;
    }
    
    public async Task<Result<string>> Login(string code)
    {
        // Login into GitHub and retrieve access token
        var tokenResult  = await RetrieveGit(code);

        if (tokenResult.IsFailed)
        {
            return Result.Fail("Login failed");
        }
        
        string accessToken = ExtractEAccessToken(tokenResult.Value)!;

        if (string.IsNullOrEmpty(accessToken))
        {
            return Result.Fail("Login failed");
        }

        return Result.Ok(accessToken);
    }

    public async Task<Result<string>> RetrieveGit(string code)
    {
        // Setup request
        string url = "https://github.com/login/oauth/access_token";
        string settings = "?client_id=" + _gitSecrets.Client + "&client_secret=" + _gitSecrets.Secret + "&code=" + code;
            
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Accepts", "application/json"),
        });
            
        var client = _httpClientFactory.CreateClient();

        // Request token from git
        try
        {
            var response = await client.PostAsync(url + settings, content);
            response.EnsureSuccessStatusCode();
            string responseData = await response.Content.ReadAsStringAsync();

            return Result.Ok(responseData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Fail("Something went wrong during login");
        }
    }

    public bool HasToken(string responseData)
    {
        var tokenData = System.Web.HttpUtility.ParseQueryString(responseData);
        
        return !string.IsNullOrEmpty(tokenData["access_token"]);
    }
    
    public async Task<Result<string>> GetUserData(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return Result.Fail("Access token not found in the response");
        }
        
        const string url = "https://api.github.com/user";
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Brewing stand");

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string userData = await response.Content.ReadAsStringAsync();

            return Result.Ok(userData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Fail("Failed to retrieve user data");
        }
    }

    private static string? ExtractEAccessToken(string rawToken)
    {
        // Extract the actual access token value
        var accessToken = rawToken.Split('&')
            .FirstOrDefault(param => param.StartsWith("access_token="))
            ?.Replace("access_token=", "");
        return accessToken;
    }
}