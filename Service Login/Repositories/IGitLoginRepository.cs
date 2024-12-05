using FluentResults;

namespace Service_Login.Repositories;

public interface IGitLoginRepository
{
    public Task<Result<string>> Login(string code);
    public Task<Result<string>> GetUserData(string rawToken);

}