using FluentResults;
using Microsoft.EntityFrameworkCore;
using Service_Login.Context;
using Service_Login.Models;

namespace Service_Login.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> CheckOrCreateUserAsync(User user)
    {
        // Check if the user exists
        var getResult = await CheckUserExistsAsync(user.gitId);

        if (getResult.IsFailed || getResult.Value == null)
        {
            // Create the user if it doesn't exist
            return await CreateUserAsync(user);
        }

        // User exists, return the existing user
        return Result.Ok(getResult.Value);
    }

    private async Task<Result<User>> CheckUserExistsAsync(long gitId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.gitId == gitId);
            return user != null ? Result.Ok(user) : Result.Fail("User not found.");
        }
        catch (Exception ex)
        {
            return Result.Fail(new List<string> { ex.Message });
        }
    }

    private async Task<Result<User>> CreateUserAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Result.Ok(user);
        }
        catch (Exception ex)
        {
            return Result.Fail<User>($"Error creating user: {ex.Message}"); 
        }
    }
}