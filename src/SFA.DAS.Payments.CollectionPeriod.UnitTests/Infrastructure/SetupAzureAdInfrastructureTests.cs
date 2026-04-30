using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Infrastructure
{
    [TestFixture]
    public class SetupAzureAdInfrastructureTests
    {
        private IConfiguration _configuration;
        private ISetupAzureAdInfrastructure _sut;
        private Mock<ILogger<SetupAzureAdInfrastructure>> _loggerMock;


        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SetupAzureAdInfrastructure>>();

            // Set up in-memory configuration for testing
            var inMemorySettings = new Dictionary<string, string>
            {
                { "TenantId", "tenantId" },
                { "ClientId", "clientId" },
                { "ClientSecret", "clientSecret" },
                { "Audience", "audience" },
                { "Scope", "api://test-api/.default"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _sut = new SetupAzureAdInfrastructure(_loggerMock.Object, _configuration);
        }

        [Test]
        public void CreateAadHttpClient_ShouldThrow_If_ClientSecret_Config_Missing()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            //Act
            _sut = new SetupAzureAdInfrastructure(_loggerMock.Object, _configuration);

            //Assert
            var ex = Assert.Throws<Exception>(() => _sut.GetAzureAdConfig());
            Assert.That(ex.Message, Does.Contain("Missing configuration for Azure AD"));
            // Assert: LogError should be called
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Missing configuration for Azure AD")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Test]
        public void GetAzureAdConfig_ReturnsCredential_WhenConfigIsValid()
        {
            var credential = _sut.GetAzureAdConfig();

            Assert.IsNotNull(credential);
        }

        [Test]
        public void GetAzureAdToken_ShouldThrow_WhenScopeMissing()
        {
            var config = new ConfigurationBuilder().Build();
            var sut = new SetupAzureAdInfrastructure(_loggerMock.Object, config);

            var fakeCredential = new FakeTokenCredential("token");

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.GetAzureAdToken(fakeCredential));
        }

        [Test]
        public void GetAzureAdToken_ShouldThrow_WhenCredentialFails()
        {
            var mockCredential = new Mock<TokenCredential>();

            mockCredential
                .Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Auth failed"));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _sut.GetAzureAdToken(mockCredential.Object));
        }

        [Test]
        public async Task GetAzureAdToken_ReturnsToken_WhenCredentialIsValid()
        {
            var fakeCredential = new FakeTokenCredential("test-token");

            var result = await _sut.GetAzureAdToken(fakeCredential);

            Assert.AreEqual("test-token", result);
        }
                
    }

    public class FakeTokenCredential : TokenCredential
    {
        private readonly AccessToken _token;

        public FakeTokenCredential(string token)
        {
            _token = new AccessToken(token, DateTimeOffset.UtcNow.AddHours(1));
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => _token;

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new ValueTask<AccessToken>(_token);
    }
}