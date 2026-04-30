using Azure.Core;
using Azure.Identity;
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
        [Test]
        public async Task CreateAadHttpClient_ShouldThrow_If_GetAzureAdConfig_Error()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<SldJobManagementApiService>>();
            _azureAdInfrastructureMock.Setup(x => x.GetAzureAdConfig()).Throws(new Exception("Azure AD config error"));
            _sut = new SldJobManagementApiService(_configuration, _azureAdInfrastructureMock.Object, _loggerMock.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _sut.CreateAadHttpClient());
            ex.Message.Should().Be("Azure AD config error");
        }

        [Test]
        public async Task CreateAadHttpClient_ShouldThrow_If_TokenCredential_Error()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<SldJobManagementApiService>>();
            _azureAdInfrastructureMock.Setup(x => x.GetAzureAdToken(It.IsAny<ClientSecretCredential>())).Throws(new Exception("Azure AD config error"));
            _sut = new SldJobManagementApiService(_configuration, _azureAdInfrastructureMock.Object, _loggerMock.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _sut.CreateAadHttpClient());
            ex.Message.Should().Be("Azure AD config error");
        }

        [Test]
        public async Task CreateAadHttpClient_ShouldThrow_If_SLDJobManagementAPIEndpoint_NotConfigured()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _sut = new SldJobManagementApiService(_configuration, _azureAdInfrastructureMock.Object, _loggerMock.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _sut.CreateAadHttpClient());
            ex.Message.Should().Be("SLD Job Management API endpoint is not configured.");
        }

        [Test]
        public async Task CreateAadHttpClient_Should_Return_HttpClient()
        {
            // Arrange
            var endpoint = "https://sldjobmanagementapiendpoint";
            var token = "test_token";
            _azureAdInfrastructureMock.Setup(x => x.GetAzureAdConfig()).Returns(new ClientSecretCredential("tenantId", "clientId", "clientSecret"));
            _azureAdInfrastructureMock.Setup(x => x.GetAzureAdToken(It.IsAny<ClientSecretCredential>())).ReturnsAsync(token);
            var inMemorySettings = new Dictionary<string, string>
            {
                { "TenantId", "tenantId" },
                { "ClientId", "clientId" },
                { "ClientSecret", "clientSecret" },
                { "Audience", "audience" },
                { "SLDJobManagementAPIEndpoint", endpoint }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _sut = new SldJobManagementApiService(_configuration, _azureAdInfrastructureMock.Object, _loggerMock.Object);

            // Act & Assert
            var httpClient = await _sut.CreateAadHttpClient();
            httpClient.Should().NotBeNull();
            httpClient.BaseAddress.Should().NotBeNull();
            httpClient.BaseAddress.ToString().Should().Be(endpoint +"/");
            httpClient.DefaultRequestHeaders.Authorization.Should().NotBeNull();
            httpClient.DefaultRequestHeaders.Authorization.Scheme.Should().Be("Bearer");
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

        }

    }
}