namespace Service_Login.Models;

public class User
{
    public Guid Id { get; set; }
    public long GitId { get; set; }
    public string Username { get; set; }
    public string Avatar { get; set; }
}