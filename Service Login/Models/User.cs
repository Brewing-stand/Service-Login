﻿namespace Service_Login.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid GitId { get; set; }
    public string Username { get; set; }
}