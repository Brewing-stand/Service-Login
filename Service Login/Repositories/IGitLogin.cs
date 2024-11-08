﻿using FluentResults;

namespace Service_Template.Repositories;

public interface IGitLogin
{
    public Task<Result<string>> Login(string code);
    public Task<Result<string>> GetUserData(string rawToken);

}