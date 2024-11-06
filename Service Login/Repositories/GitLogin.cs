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
        var responseData = await RetrieveGit(code);

        if (!HasToken(responseData.Value))
        {
            return Result.Fail("Login unsuccessful");
        }

        return Result.Ok(responseData.Value);
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
}