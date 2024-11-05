namespace Service_Template.Models;

public class LoginResult(string accessToken = null, string errorMessage = null)
{
    public string AccessToken { get; set; } = accessToken;
    public string ErrorMessage { get; set; } = errorMessage;
    public bool IsSuccess { get; set; } = string.IsNullOrEmpty(errorMessage);
}