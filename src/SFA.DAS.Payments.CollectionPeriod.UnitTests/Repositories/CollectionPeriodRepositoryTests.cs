using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
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
        public async Task GetOpenCollectionPeriods_ReturnsOpenPeriods()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetOpenCollectionPeriods();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(cp => cp.Status == CollectionPeriodStatus.Open), Is.True);
        }

        [Test]
        public async Task GetAllCollectionPeriods_ReturnsAllPeriods()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, Status = CollectionPeriodStatus.Open },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, Status = CollectionPeriodStatus.Closed },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, Status = CollectionPeriodStatus.Open },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetAllCollectionPeriods();

            Assert.That(result.Count(), Is.EqualTo(3));
        }
    }
}
