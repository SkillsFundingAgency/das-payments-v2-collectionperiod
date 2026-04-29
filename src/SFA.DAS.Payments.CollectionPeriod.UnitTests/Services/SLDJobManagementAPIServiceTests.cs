using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Services
{
    [TestFixture]
    public class SLDJobManagementAPIServiceTests
    {
        private IConfiguration _configuration;
        private ISldJobManagementApiService _sut;
        private Mock<ILogger<SldJobManagementApiService>> _loggerMock;
        private Mock<ISetupAzureAdInfrastructure> _azureAdInfrastructureMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SldJobManagementApiService>>();
            _azureAdInfrastructureMock = new Mock<ISetupAzureAdInfrastructure>();

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
            _sut = new SldJobManagementApiService(_configuration, _azureAdInfrastructureMock.Object, _loggerMock.Object);
        }
    }
}