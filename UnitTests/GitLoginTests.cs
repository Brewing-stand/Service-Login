using FluentResults;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq.Protected;
using NUnit.Framework;
using Service_User.Repositories;
using Service_User.Settings;

[TestFixture]
public class GitLoginTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<HttpMessageHandler> _handlerMock;
    private GitLogin _gitLogin;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _handlerMock = new Mock<HttpMessageHandler>();
        
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient(_handlerMock.Object));

        var gitSecrets = new GitSecrets { Client = "client_id", Secret = "client_secret" };
        var options = Options.Create(gitSecrets);
        _gitLogin = new GitLogin(_httpClientFactoryMock.Object, options);
    }

    [Test]
    public async Task Login_ShouldReturnSuccess_WhenTokenIsValid()
    {
        // Arrange
        var responseData = "access_token=valid_token&scope=repo&token_type=bearer";
        _handlerMock.SetupRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token", responseData, 200);

        // Act
        var result = await _gitLogin.Login("auth_code");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("valid_token", result.Value);
    }

    [Test]
    public async Task Login_ShouldReturnFailure_WhenTokenIsNotFound()
    {
        // Arrange
        var responseData = "error=invalid_grant";
        _handlerMock.SetupRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token", responseData, 400);

        // Act
        var result = await _gitLogin.Login("auth_code");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Login unsuccessful", result.Errors[0].Message);
    }
}

public static class HttpClientExtensions
{
    public static void SetupRequestMessage(this Mock<HttpMessageHandler> handlerMock, HttpMethod method, string url, string responseData, int statusCode)
    {
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == method && r.RequestUri.ToString().Contains(url)), It.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = (System.Net.HttpStatusCode)statusCode,
                Content = new StringContent(responseData)
            });
    }
}
