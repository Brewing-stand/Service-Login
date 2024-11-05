using Service_Template.Models;

namespace Service_Template.Repositories;

public interface IGitLogin
{
    Task<LoginResult> Login(string code);
}