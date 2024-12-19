namespace Service_Login.Services;

using Refit;
using System.Threading.Tasks;
using Service_Login.Models;

public interface IGitHubServiceApi
{
    // Endpoint for getting user data
    [Get("/user")]
    [Headers("User-Agent: Brewing Stand")] // Add User-Agent header to the method
    Task<GitUserData> GetUserDataAsync([Header("Authorization")] string authorizationHeader);
}