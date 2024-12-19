using Refit;

namespace Service_Login.Services;

public interface IGitHubAuthApi
{
    // Endpoint for retrieving the access token (OAuth)
    [Post("/login/oauth/access_token")]
    Task<string> GetAccessTokenAsync(
        [AliasAs("client_id")] string clientId,
        [AliasAs("client_secret")] string clientSecret,
        [AliasAs("code")] string code,
        [AliasAs("Accept")] string accept
    );
}