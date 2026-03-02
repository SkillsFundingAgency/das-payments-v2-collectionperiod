using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Repositories
{
    [TestFixture]
    public class CollectionPeriodRepositoryTests
    {
        private IPaymentsDataContext _mockContext;
        private CollectionPeriodRepository sut;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();

            var contextBuilder = new DbContextOptionsBuilder<PaymentsDataContext>()
                .UseInMemoryDatabase(databaseName: dbName).Options;

            _mockContext = new PaymentsDataContext(contextBuilder);

            sut = new CollectionPeriodRepository(_mockContext, new Mock<ILogger<CollectionPeriodRepository>>().Object);
        }

        [Test]
        public async Task OpenCollectionYears_ReturnsDistinctOpenYears()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2223, Period = 1, Status = CollectionPeriodStatus.Closed },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.OpenCollectionYears();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Contains((short)2425), Is.True);
            Assert.That(result.Contains((short)2324), Is.True);
        }

        [Test]
        public async Task CollectionYear_ReturnsPeriodsForGivenYear()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.CollectionYear(2425, null);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.AcademicYear == 2425), Is.True);
            Assert.That(result.Any(p => p.Period == 1 && p.Status == CollectionPeriodStatus.Open), Is.True);
            Assert.That(result.Any(p => p.Period == 2 && p.Status == CollectionPeriodStatus.Closed), Is.True);
        }

        [Test]
        public async Task CollectionYear_ReturnsPeriodsForGivenYearAndStatus_Open()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.CollectionYear(2425, CollectionPeriodStatus.Open);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.AcademicYear == 2425), Is.True);
            Assert.That(result.Any(p => p.Period == 1 && p.Status == CollectionPeriodStatus.Open), Is.True);
        }

        [Test]
        public async Task CollectionYear_ReturnsPeriodsForGivenYearAndStatus_NotStarted()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.NotStarted },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.CollectionYear(2425, CollectionPeriodStatus.NotStarted);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.AcademicYear == 2425), Is.True);
            Assert.That(result.Any(p => p.Period == 1 && p.Status == CollectionPeriodStatus.NotStarted), Is.True);
        }

        [Test]
        public async Task CollectionYear_ReturnsPeriodsForGivenYearAndStatus_Closed()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.NotStarted },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.CollectionYear(2425, CollectionPeriodStatus.Closed);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.AcademicYear == 2425), Is.True);
            Assert.That(result.Any(p => p.Period == 2 && p.Status == CollectionPeriodStatus.Closed), Is.True);
        }

        [Test]
        public async Task CollectionYear_ReturnsPeriodsForGivenYearAndStatus_Completed()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.NotStarted },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Completed },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.CollectionYear(2324, CollectionPeriodStatus.Completed);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.AcademicYear == 2324), Is.True);
            Assert.That(result.Any(p => p.Period == 1 && p.Status == CollectionPeriodStatus.Completed), Is.True);
        }
    }
}
