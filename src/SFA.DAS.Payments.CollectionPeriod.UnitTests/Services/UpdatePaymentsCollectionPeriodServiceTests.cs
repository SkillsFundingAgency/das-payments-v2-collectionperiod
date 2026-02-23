using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Mapping;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Services
{
    [TestFixture]
    public class UpdatePaymentsCollectionPeriodServiceTests
    {
        private Mock<ICollectionPeriodRepository> _repositoryMock;
        private UpdatePaymentsCollectionPeriodService _service;
        private CollectionPeriodMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ICollectionPeriodRepository>();
            _mapper = new CollectionPeriodMapper();
            _service = new UpdatePaymentsCollectionPeriodService(_repositoryMock.Object, _mapper);
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

            await _service.UpdatePaymentsCollectionPeriod([period]);

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
                    IsOpen = false
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
                },
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 1,
                    CollectionYear = 2425,
                    StartDateTimeUtc = new DateTime(2024, 8, 1),
                    EndDateTimeUtc = new DateTime(2024, 8, 31),
                    IsOpen = true
                }
            };

            await _service.UpdatePaymentsCollectionPeriod(periods);

            _repositoryMock.Verify(r => r.UpdateCollectionPeriods(It.Is<IEnumerable<CollectionPeriodModel>>(p =>
                p.Count() == 4 &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 1 && cp.Status == CollectionPeriodStatus.Open) &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 2 && cp.Status == CollectionPeriodStatus.Closed) &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 3 && cp.Status == CollectionPeriodStatus.Closed) &&
                p.Any(cp => cp.AcademicYear == 2425 && cp.Period == 4 && cp.Status == CollectionPeriodStatus.Closed)
            )), Times.Once);
        }
    }
}   
