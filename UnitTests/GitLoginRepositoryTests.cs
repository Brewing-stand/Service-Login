using Microsoft.Extensions.Options;
using Moq;
using Service_Login.Repositories;
using Service_Login.Settings;

[TestFixture]
public class GitLoginRepositoryTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private GitLoginRepository _gitLoginRepository;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var gitSecrets = new GitSecrets { Client = "client_id", Secret = "client_secret" };
        var options = Options.Create(gitSecrets);

        _gitLoginRepository = new GitLoginRepository(_httpClientFactoryMock.Object, options);
    }

    [Test]
    public void Login_ShouldBeImplemented()
    {
        // Placeholder for actual test logic
        Assert.Pass("Login test not implemented yet.");
    }
}