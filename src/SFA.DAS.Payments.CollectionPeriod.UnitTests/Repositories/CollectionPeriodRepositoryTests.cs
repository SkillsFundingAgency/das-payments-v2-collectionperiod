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

        [Test]
        public async Task GetCurrentCollectionYear_ReturnsOldestYearWithFuturePeriod()
        {
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Closed, EndDateTime = DateTime.Today.AddDays(-30) },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open, EndDateTime = DateTime.Today.AddDays(10) },
                new CollectionPeriodModel { AcademicYear = 2526, Period = 1, Status = CollectionPeriodStatus.NotStarted, EndDateTime = DateTime.Today.AddDays(400) },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetCurrentCollectionYear();

            Assert.That(result, Is.EqualTo((short)2425));
        }

        [Test]
        public async Task GetCurrentCollectionYear_IncludesOlderYearWithStaleOpenStatus()
        {
            // AcademicYear 2324 has no period with a future EndDateTime, but still has a period
            // stuck as Open - it should still be picked up so the next SLD sync can correct it
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open, EndDateTime = DateTime.Today.AddDays(-30) },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.NotStarted, EndDateTime = DateTime.Today.AddDays(10) },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetCurrentCollectionYear();

            Assert.That(result, Is.EqualTo((short)2324));
        }

        [Test]
        public async Task GetCurrentCollectionYear_ExcludesYearsWithNoFuturePeriodsAndNoStaleStatus()
        {
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Closed, EndDateTime = DateTime.Today.AddDays(-30) },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Completed, EndDateTime = DateTime.Today.AddDays(-1) },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetCurrentCollectionYear();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task UpdateCollectionPeriods_UpdatesStatusOfPeriods()
        {
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.NotStarted },
                new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Closed },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var updatedPeriods = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Closed },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Open },
            };

            await sut.UpdateCollectionPeriods(updatedPeriods);

            var result = _mockContext.CollectionPeriod.ToList();

            Assert.That(result.First(cp => cp.AcademicYear == 2425 && cp.Period == 1).Status, Is.EqualTo(CollectionPeriodStatus.Closed));
            Assert.That(result.First(cp => cp.AcademicYear == 2425 && cp.Period == 2).Status, Is.EqualTo(CollectionPeriodStatus.Open));
            Assert.That(result.First(cp => cp.AcademicYear == 2324 && cp.Period == 1).Status, Is.EqualTo(CollectionPeriodStatus.Closed));
        }

        [Test]
        public async Task UpdateCollectionPeriods_UpdatesStartAndEndDateTimeOfExistingPeriods()
        {
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open, StartDateTime = DateTime.Today.AddMonths(-1), EndDateTime = DateTime.Today.AddDays(-30) },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var newStart = DateTime.Today.AddDays(1);
            var newEnd = DateTime.Today.AddDays(28);

            var updatedPeriods = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.NotStarted, StartDateTime = newStart, EndDateTime = newEnd },
            };

            await sut.UpdateCollectionPeriods(updatedPeriods);

            var result = _mockContext.CollectionPeriod.First(cp => cp.AcademicYear == 2425 && cp.Period == 1);

            Assert.That(result.Status, Is.EqualTo(CollectionPeriodStatus.NotStarted));
            Assert.That(result.StartDateTime, Is.EqualTo(newStart));
            Assert.That(result.EndDateTime, Is.EqualTo(newEnd));
        }

        [Test]
        public async Task UpdateCollectionPeriods_AddNewPeriods()
        {
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open }
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var updatedPeriods = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Closed },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Open }
            };

            await sut.UpdateCollectionPeriods(updatedPeriods);

            var result = _mockContext.CollectionPeriod.ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First(cp => cp.AcademicYear == 2425 && cp.Period == 1).Status, Is.EqualTo(CollectionPeriodStatus.Closed));
            Assert.That(result.First(cp => cp.AcademicYear == 2425 && cp.Period == 2).Status, Is.EqualTo(CollectionPeriodStatus.Open));
        }

        [Test]
        public async Task UpdateCollectionPeriodSetCompleted_SetsStatusToCompleted()
        {
            var mockData = new[]
            {
                new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Open }
            };
            _mockContext.CollectionPeriod.AddRange(mockData);

            await _mockContext.SaveChangesAsync();

            await sut.UpdateCollectionPeriodSetCompleted(2425, 1);

            var result = _mockContext.CollectionPeriod.ToList();

            Assert.That(result.First(cp => cp.AcademicYear == 2425 && cp.Period == 1).Status, Is.EqualTo(CollectionPeriodStatus.Completed));
            Assert.That(result.First(cp => cp.AcademicYear == 2425 && cp.Period == 2).Status, Is.EqualTo(CollectionPeriodStatus.Open));
        }
    }
}
