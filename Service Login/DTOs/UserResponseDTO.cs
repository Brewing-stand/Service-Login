namespace Service_Login.DTOs;

public class UserResponseDTO
{
    public Guid Id { get; set; }
    public long GitId { get; set; }
    public string Username { get; set; }
    public string Avatar { get; set; }
}