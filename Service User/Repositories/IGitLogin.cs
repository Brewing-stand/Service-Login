using FluentResults;

namespace Service_User.Repositories;

public interface IGitLogin
{
    public Task<Result<string>> Login(string code);
    public Task<Result<string>> GetUserData(string rawToken);

}