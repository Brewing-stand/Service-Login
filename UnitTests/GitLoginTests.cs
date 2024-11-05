using Microsoft.Extensions.Options;
using Moq.Protected;

namespace UnitTests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Service_Template.Models;
using Service_Template.Repositories;
using Service_Template.Settings;

[TestFixture]
    public class GitLoginTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private GitLogin _gitLogin;
        private GitSecrets _gitSecrets;

        [SetUp]
        public void Setup()
        {
            // Mock HttpClient
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Mock GitSecrets
            _gitSecrets = new GitSecrets
            {
                Client = "test_client_id",
                Secret = "test_client_secret"
            };

            // Mock IOptions<GitSecrets>
            var gitSecretsOptions = new Mock<IOptions<GitSecrets>>();
            gitSecretsOptions.Setup(opt => opt.Value).Returns(_gitSecrets);

            // Create instance of GitLogin
            _gitLogin = new GitLogin(_httpClientFactoryMock.Object, gitSecretsOptions.Object);
        }

        [Test]
        public void ParseTokenResponse_ValidResponse_ReturnsTokenJson()
        {
            // Arrange
            var responseData = "access_token=valid_access_token&scope=user&token_type=bearer";

            // Act
            var token = _gitLogin.ParseTokenResponse(responseData);

            // Assert
            Assert.IsNotNull(token);
            StringAssert.Contains("valid_access_token", token);
        }

        [Test]
        public void ParseTokenResponse_InvalidResponse_ReturnsNull()
        {
            // Arrange
            var responseData = "";

            // Act
            var token = _gitLogin.ParseTokenResponse(responseData);

            // Assert
            Assert.IsNull(token);
        }
    }