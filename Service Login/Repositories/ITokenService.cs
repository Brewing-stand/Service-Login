namespace Service_Login.Repositories;

public interface ITokenService
{
    string GenerateJwt(string userId, string username);
}