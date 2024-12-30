namespace Service_Login.Models;

public class User
{
    public Guid id { get; set; }
    public long gitId { get; set; }
    public string username { get; set; }
    public string avatar { get; set; }
}