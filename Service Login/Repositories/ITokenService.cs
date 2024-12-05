using Service_Login.Models;

namespace Service_Login.Repositories;

public interface ITokenService
{
    string GenerateToken(User user);
}