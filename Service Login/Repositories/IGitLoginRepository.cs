using FluentResults;
using Service_Login.Models;

namespace Service_Login.Repositories;

public interface IGitLoginRepository
{
    public Task<Result<string>> Login(string code);
    public Task<Result<GitUserData>> GetUserData(string rawToken);

}