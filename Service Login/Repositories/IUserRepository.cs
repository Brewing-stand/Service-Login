using FluentResults;
using Service_Login.Models;

namespace Service_Login.Repositories;

public interface IUserRepository
{
    Task<Result<User>> CheckOrCreateUserAsync(User user);
}