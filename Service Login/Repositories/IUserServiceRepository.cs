using FluentResults;
using Service_Login.Models;

namespace Service_Login.Repositories;

public interface IUserServiceRepository
{
    Task<Result<User>> CheckOrCreateUserAsync(GitUserData gitUserData);
}