using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Mappers
{
    [TestFixture]
    public class SyncCollectionPeriodsMapperTests
    {
        private ISyncCollectionPeriodMapper _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new SyncCollectionPeriodMapper();
        }

        [Test]
        public void Map_ShouldReturnCollectionPeriod_WhenValidInput()
        {
            var dto = new SLDJobContextCollectionPeriodModel
            {
                PeriodNumber = 1,
                CollectionYear = 2425,
                StartDateTimeUtc = DateTime.UtcNow.AddDays(-25),
                EndDateTimeUtc = DateTime.UtcNow.AddDays(5),
                IsOpen = false
            };

            var result = _mockMapper.MapToPaymentsDBCollectionPeriods([dto]);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.PeriodNumber, result.First().Period);
            Assert.AreEqual(dto.CollectionYear, result.First().AcademicYear);
            Assert.AreEqual(CollectionPeriodStatus.Closed, result.First().Status);
        }

        [Test]
        public void Map_ShouldReturnClosedStatus_WhenIsOpenIsFalse()
        {
            var dto = new SLDJobContextCollectionPeriodModel
            {
                PeriodNumber = 1,
                CollectionYear = 2526,
                StartDateTimeUtc = DateTime.Now.AddDays(-30),
                EndDateTimeUtc = DateTime.Now.AddDays(-1),
                IsOpen = false
            };

            var result = _mockMapper.MapToPaymentsDBCollectionPeriods([dto]);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.PeriodNumber, result.First().Period);
            Assert.AreEqual(dto.CollectionYear, result.First().AcademicYear);
            Assert.AreEqual(CollectionPeriodStatus.Closed, result.First().Status);
        }

        [Test]
        public void Map_ShouldReturnOpenStatus_WhenIsOpenIsTrue()
        {
            var dto = new SLDJobContextCollectionPeriodModel
            {
                PeriodNumber = 2,
                CollectionYear = 2526,
                StartDateTimeUtc = DateTime.UtcNow.AddDays(-10),
                EndDateTimeUtc = DateTime.UtcNow.AddDays(4),
                IsOpen = true
            };

            var result = _mockMapper.MapToPaymentsDBCollectionPeriods([dto]);

            Assert.IsNotNull(result);
            Assert.AreEqual(CollectionPeriodStatus.Open, result.First().Status);
        }


        [Test]
        public void Map_ShouldReturnNotStartedStatus_WhenIsOpenIsFalseAndStartDateInFuture()
        {
            var dto = new SLDJobContextCollectionPeriodModel
            {
                PeriodNumber = 3,
                CollectionYear = 2526,
                StartDateTimeUtc = DateTime.UtcNow.AddDays(1),
                EndDateTimeUtc = DateTime.UtcNow.AddDays(10),
                IsOpen = false
            };

            var result = _mockMapper.MapToPaymentsDBCollectionPeriods([dto]);

            Assert.IsNotNull(result);
            Assert.AreEqual(CollectionPeriodStatus.NotStarted, result.First().Status);
        }

        //Not Testable here
        //[Test]
        //public void Map_ShouldReturnCompletedStatus_WhenIsOpenIsFalseAndEndDateInPast()
        //{
        //    var dto = new SLDJobContextCollectionPeriodModel
        //    {
        //        PeriodNumber = 4,
        //        CollectionYear = 2425,
        //        StartDateTimeUtc = DateTime.UtcNow.AddDays(-10),
        //        EndDateTimeUtc = DateTime.UtcNow.AddDays(-1),
        //        IsOpen = false
        //    };

        //    var result = _mockMapper.MapToPaymentsCollectionPeriods([dto]);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(CollectionPeriodStatus.Completed, result.First().Status);
        //}

        [Test]
        public void Map_ShouldHandleMultipleInputs()
        {
            var dtos = new[]
            {
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 2,
                    CollectionYear = 2526,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(-10),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(4),
                    IsOpen = false
                },
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 3,
                    CollectionYear = 2526,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(1),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(10),
                    IsOpen = false
                },
                new SLDJobContextCollectionPeriodModel
                {
                    PeriodNumber = 1,
                    CollectionYear = 2526,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(-2),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(28),
                    IsOpen = true
                }
            };


            var result = _mockMapper.MapToPaymentsDBCollectionPeriods(dtos).ToList();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.AreEqual(CollectionPeriodStatus.Closed, result[0].Status);
            Assert.AreEqual(CollectionPeriodStatus.NotStarted, result[1].Status);
            Assert.AreEqual(CollectionPeriodStatus.Open, result[2].Status);
        }

        [Test]
        public void Map_ShouldHandleEmptyInput()
        {
            var result = _mockMapper.MapToPaymentsDBCollectionPeriods(Array.Empty<SLDJobContextCollectionPeriodModel>());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
