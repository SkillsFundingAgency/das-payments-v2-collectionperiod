using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Tests.Repositories
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
        public async Task GetCollectionPeriodByAcademicYear_ReturnsOpenPeriods()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 1, IsOpen = true },
                 new CollectionPeriodModel { AcademicYear = 2425, Period = 2, IsOpen = false },
                 new CollectionPeriodModel { AcademicYear = 2324, Period = 1, IsOpen = true },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetCollectionPeriodByAcademicYear(2425);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.All(cp => cp.AcademicYear == 2425 && cp.IsOpen == true), Is.True);
        }

        [Test]
        public async Task GetCollectionPeriodByAcademicYear_ReturnsEmpty_WhenNoMatches()
        {
            var mockData = new[]{
                 new CollectionPeriodModel { AcademicYear = 2223, Period =1, IsOpen = true },
            };

            _mockContext.CollectionPeriod.AddRange(mockData);
            await _mockContext.SaveChangesAsync();

            var result = await sut.GetCollectionPeriodByAcademicYear(2425);

            Assert.That(result, Is.Empty);
        }
    }
}
