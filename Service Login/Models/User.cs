namespace Service_Login.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Primary Key
    public string GitHubId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}