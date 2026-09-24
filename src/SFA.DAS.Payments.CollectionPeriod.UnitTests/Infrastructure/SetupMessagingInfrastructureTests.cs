using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Messaging;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Infrastructure
{
    [TestFixture]
    public class SetupMessagingInfrastructureTests
    {
        private SetupMessagingInfrastructure _sut;
        private Mock<ILogger<SetupMessagingInfrastructure>> _loggerMock;
        private IConfiguration _configuration;
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SetupMessagingInfrastructure>>();
        }


        [Test]
        public async Task StartAsync_ShouldLogWarning_IfConfigValuesMissing()
        {
            // Arrange: missing required config values
            var inMemorySettings = new Dictionary<string, string>
            {
                // Intentionally leave out required connection strings and values
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _sut = new SetupMessagingInfrastructure(_loggerMock.Object, _configuration);

            // Act
            await _sut.StartAsync(CancellationToken.None);

            // Assert: LogWarning should be called
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("One or more required configuration values are missing")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
