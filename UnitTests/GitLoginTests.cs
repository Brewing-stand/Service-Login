using Moq;
using System.Net.Http;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Service_User.Repositories;
using Service_User.Settings;

[TestFixture]
public class GitLoginTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private GitLogin _gitLogin;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var gitSecrets = new GitSecrets { Client = "client_id", Secret = "client_secret" };
        var options = Options.Create(gitSecrets);

        _gitLogin = new GitLogin(_httpClientFactoryMock.Object, options);
    }

    [Test]
    public void Login_ShouldBeImplemented()
    {
        // Placeholder for actual test logic
        Assert.Pass("Login test not implemented yet.");
    }
}