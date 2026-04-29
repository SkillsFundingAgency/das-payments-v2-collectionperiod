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
    }
}