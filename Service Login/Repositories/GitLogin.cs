using System.Text.Json;
using Service_Template.Models;

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
    
    public async Task<LoginResult> Login(string code)
    {
        string url = "https://github.com/login/oauth/access_token";
        string settings = "?client_id=" + _gitSecrets.Client + "&client_secret=" + _gitSecrets.Secret + "&code=" + code;
            
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Accepts", "application/json"),
        });
            
        var client = _httpClientFactory.CreateClient();

        try
        {
            var response = await client.PostAsync(url + settings, content);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            string token = ParseTokenResponse(responseData);
            
            if (string.IsNullOrEmpty(token))
            {
                return new LoginResult(errorMessage: "Login failed. Invalid token received");
            }

            return new LoginResult(accessToken: token);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            return new LoginResult(errorMessage: "An error occurred during login");
        }
    }


    public string ParseTokenResponse(string responseData)
    {
        var tokenData = System.Web.HttpUtility.ParseQueryString(responseData);
        
        if (string.IsNullOrEmpty(tokenData["access_token"]))
        {
            return null; // Return null if the token is not present
        }
        
        var jsonToken = JsonSerializer.Serialize(new
        {
            access_token = tokenData["access_token"],
            scope = tokenData["scope"],
            token_type = tokenData["token_type"]
        });
        
        return jsonToken;
    }
}