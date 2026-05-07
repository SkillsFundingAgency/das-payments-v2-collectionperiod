using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using System.Net;
using System.Net.Http.Json;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Services
{
    [TestFixture]
    public class SLDJobManagementAPIServiceTests
    {
        private IConfiguration _configuration;
        private ISLDJobManagementAPIService _sut;
        private Mock<ILogger<SLDJobManagementAPIService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SLDJobManagementAPIService>>();
        }

        [Test]
        public async Task GetCollectionPeriods_ShouldReturn_Data_WhenApiReturnsSuccess()
        {
            // Arrange
            var expected = new List<SLDJobContextCollectionPeriodModel>
            {
                new SLDJobContextCollectionPeriodModel()
            };

            var handler = new MockHttpMessageHandler(expected, HttpStatusCode.OK);

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://test/api/")
            };

            var sut = new SLDJobManagementAPIService(httpClient, _loggerMock.Object);

            // Act
            var result = await sut.GetCollectionPeriods(2425);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Test]
        public async Task GetCollectionPeriods_ShouldReturnEmpty_WhenApiFails()
        {
            // Arrange
            var handler = new MockHttpMessageHandler(null, HttpStatusCode.BadRequest);

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://test/api/")
            };

            var sut = new SLDJobManagementAPIService(httpClient, _loggerMock.Object);

            // Act
            var result = await sut.GetCollectionPeriods(2425);

            // Assert
            result.Should().BeEmpty();
        }

    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly object _response;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(object response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = new HttpResponseMessage
            {
                StatusCode = _statusCode
            };

            if (_response != null)
            {
                message.Content = JsonContent.Create(_response);
            }

            return Task.FromResult(message);
        }
    }
}