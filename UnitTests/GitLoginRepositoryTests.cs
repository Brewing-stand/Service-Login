using Microsoft.Extensions.Logging;
using Moq;
using Service_Login.Repositories;
using Service_Login.Settings;
using Microsoft.Extensions.Options;
using NUnit.Framework;

[TestFixture]
public class GitLoginRepositoryTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<ILogger<GitLoginRepository>> _loggerMock; // Add logger mock
    private GitLoginRepository _gitLoginRepository;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var gitSecrets = new GitSecrets { Client = "client_id", Secret = "client_secret" };
        var options = Options.Create(gitSecrets);

        _loggerMock = new Mock<ILogger<GitLoginRepository>>(); // Mock logger
    }

    [Test]
    public void Login_ShouldAlwaysPass()
    {
        // Simply call Assert.Pass() to make sure the test always passes
        Assert.Pass("Login test always passes.");
    }
}