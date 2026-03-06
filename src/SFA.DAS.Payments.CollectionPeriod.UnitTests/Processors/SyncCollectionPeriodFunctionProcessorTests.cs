using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Processors;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Processors
{
    [TestFixture]
    public class SyncCollectionPeriodFunctionProcessorTests
    {
        private Mock<ICollectionPeriodRepository> _repositoryMock;
        private Mock<ICollectionPeriodRepository> _mockRepository;
        private Mock<ILogger<SyncCollectionPeriodsFunctionProcessor>> _mockLogger;
        private Mock<ISyncCollectionPeriodMapper> _mockMapper;
        private SyncCollectionPeriodsFunctionProcessor _sut;
        private Mock<ISLDJobManagementAPIService> _mockSLDAPI;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ICollectionPeriodRepository>();
            _mockMapper = new Mock<ISyncCollectionPeriodMapper>();
            _mockSLDAPI = new Mock<ISLDJobManagementAPIService>();
            _mockLogger = new Mock<ILogger<SyncCollectionPeriodsFunctionProcessor>>();

            _sut = new SyncCollectionPeriodsFunctionProcessor(_mockSLDAPI.Object, _mockMapper.Object, _repositoryMock.Object, _mockLogger.Object);
        }

        [Test]
        public async Task UpdateCollectionPeriod_ShouldCallRepositoryWithCorrectMappedCollectionPeriod()
        {
            var period = new SLDJobContextCollectionPeriodModel
            {
                PeriodNumber = 1,
                CollectionYear = 2425,
                StartDateTimeUtc = new DateTime(2024, 8, 1),
                EndDateTimeUtc = new DateTime(2024, 8, 31),
                IsOpen = true
            };

            _mockSLDAPI.Setup(s => s.GetCollectionPeriods(DateTime.Today.ToString("yyyy-MM-dd")))
                .ReturnsAsync(new[] { period });

            _repositoryMock.Setup(r => r.UpdateCollectionPeriods(It.IsAny<IEnumerable<CollectionPeriodModel>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockMapper.Setup(m => m.MapToPaymentsDBCollectionPeriods(It.IsAny<IEnumerable<SLDJobContextCollectionPeriodModel>>()))
                .Returns(new[]
                {
                    new CollectionPeriodModel
                    {
                        AcademicYear = 2425,
                        Period = 1,
                        Status = CollectionPeriodStatus.Open
                    }
                });

            await _sut.Process();

            _repositoryMock.Verify(r => r.UpdateCollectionPeriods(It.Is<IEnumerable<CollectionPeriodModel>>(p =>
                p.Count() == 1 &&
                p.First().AcademicYear == 2425 &&
                p.First().Period == 1 &&
                p.First().Status == CollectionPeriodStatus.Open
            )), Times.Once);
        }

        [Test]
        public async Task UpdateCollectionPeriod_UpdatesMultipleCollectionPeriod()
        {
            var periods = new[]
            {
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 2,
                    CollectionYear = 2425,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(-10),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(4),
                    IsOpen = true
                },
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 3,
                    CollectionYear = 2425,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(1),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(10),
                    IsOpen = false
                },
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 4,
                    CollectionYear = 2425,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(-10),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(-1),
                    IsOpen = false
                }
            };

            _mockSLDAPI.Setup(s => s.GetCollectionPeriods(DateTime.Today.ToString("yyyy-MM-dd")))
                .ReturnsAsync(periods);

            _repositoryMock.Setup(r => r.UpdateCollectionPeriods(It.IsAny<IEnumerable<CollectionPeriodModel>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

             _mockMapper.Setup(m => m.MapToPaymentsDBCollectionPeriods(It.IsAny<IEnumerable<SLDJobContextCollectionPeriodModel>>()))
                .Returns(new[]
                {
                    new CollectionPeriodModel
                    {
                        AcademicYear = 2425,
                        Period = 2,
                        Status = CollectionPeriodStatus.Open
                    },
                    new CollectionPeriodModel
                    {
                        AcademicYear = 2425,
                        Period = 3,
                        Status = CollectionPeriodStatus.NotStarted
                    },
                    new CollectionPeriodModel
                    {
                        AcademicYear = 2425,
                        Period = 4,
                        Status = CollectionPeriodStatus.Closed
                    }
                });

            await _sut.Process();

            _repositoryMock.Verify(r => r.UpdateCollectionPeriods(It.Is<IEnumerable<CollectionPeriodModel>>(p =>
                p.Count() == 3 &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 2 && cp.Status == CollectionPeriodStatus.Open) &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 3 && cp.Status == CollectionPeriodStatus.NotStarted) &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 4 && cp.Status == CollectionPeriodStatus.Closed)
            )), Times.Once);
        }
    }
}   
