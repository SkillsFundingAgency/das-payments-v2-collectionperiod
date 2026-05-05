using Azure.Core;
using Azure.Identity;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure;
using System.Net;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Infrastructure
{
    [TestFixture]
    public class AzureAdTokenHandlerTests
    {
        private Mock<ClientSecretCredential> _credentialMock;
        private IConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _credentialMock = new Mock<ClientSecretCredential>();

            var settings = new Dictionary<string, string>
            {
                { "Audience", "api://test" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Test]
        public async Task Handler_Should_Add_Bearer_Token()
        {
            // Arrange
            var token = new AccessToken("test_token", DateTimeOffset.UtcNow.AddHours(1));

            _credentialMock
                .Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            var handler = new AzureAdTokenHandler(_credentialMock.Object, _config)
            {
                InnerHandler = new TestHandler()
            };

            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("https://test.com");

            // Assert
            response.RequestMessage.Headers.Authorization.Should().NotBeNull();
            response.RequestMessage.Headers.Authorization.Scheme.Should().Be("Bearer");
            response.RequestMessage.Headers.Authorization.Parameter.Should().Be("test_token");

            _credentialMock.Verify(
                x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public void Handler_Should_Throw_When_Audience_Is_Missing()
        {
            // Arrange
            var settings = new Dictionary<string, string>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var handler = new AzureAdTokenHandler(_credentialMock.Object, config)
            {
                InnerHandler = new TestHandler()
            };

            var client = new HttpClient(handler);

            // Act
            Func<Task> act = async () => await client.GetAsync("https://test.com");

            // Assert
            var ex = act.Should().ThrowAsync<ArgumentException>().Result;
            ex.Which.Message.Should().Be("Audience is missing");

            _credentialMock.Verify(
                x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Handler_Should_Throw_When_Audience_Is_Empty()
        {
            // Arrange
            var settings = new Dictionary<string, string>
            {
                { "Audience", "" } 
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var handler = new AzureAdTokenHandler(_credentialMock.Object, config)
            {
                InnerHandler = new TestHandler()
            };

            var client = new HttpClient(handler);

            // Act
            Func<Task> act = async () => await client.GetAsync("https://test.com");

            // Assert
            var ex = act.Should().ThrowAsync<ArgumentException>().Result;
            ex.Which.Message.Should().Be("Audience is missing");

            // ✅ Ensure token is NEVER requested
            _credentialMock.Verify(
                x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }

    public class TestHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request
            });
        }
    }
}
