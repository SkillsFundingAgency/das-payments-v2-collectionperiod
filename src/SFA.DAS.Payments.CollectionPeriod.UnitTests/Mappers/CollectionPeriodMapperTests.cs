using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Mappers
{
    [TestFixture]
    internal class CollectionPeriodMapperTests
    {
        private CollectionPeriodMapper _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new CollectionPeriodMapper();
        }

        [Test]
        public void Map_ShouldMapAllProperties_WhenSourceIsValid()
        {
            var source = new CollectionPeriodModel
            {
                Id = 1,
                Period = 5,
                AcademicYear = 2324,
                ReferenceDataValidationDate = new DateTime(2024, 1, 15),
                CompletionDate = new DateTime(2024, 2, 28),
                Status = CollectionPeriodStatus.Open,
            };

            var result = _sut.MapToCollectionPeriodResponseModel(source);

            result.Should().NotBeNull();
            result.Id.Should().Be(source.Id);
            result.Period.Should().Be(source.Period);
            result.Status.Should().Be(source.Status.Value.ToString());
        }

        [Test]
        public void MapList_ShouldMapAllItems()
        {
            var sources = new List<CollectionPeriodModel>
            {
                new CollectionPeriodModel
                {
                    Id = 1,
                    Period = 1,
                    AcademicYear = 2324,
                    Status = CollectionPeriodStatus.Completed,
                },
                new CollectionPeriodModel
                {
                    Id = 2,
                    Period = 2,
                    AcademicYear = 2324,
                    Status = CollectionPeriodStatus.Closed,
                },
                new CollectionPeriodModel
                {
                    Id = 3,
                    Period = 3,
                    AcademicYear = 2324,
                    Status = CollectionPeriodStatus.Open,
                 }
            };

            var results = _sut.MapToCollectionPeriodsForCollectionYearResponseModel(sources, 2024, null);

            results.Periods.Should().HaveCount(3);
            results.Periods.Select(r => r.Period).Should().BeEquivalentTo(new[] { 1, 2, 3 });
            results.Periods.Select(r => r.Id).Should().BeEquivalentTo(new[] { 1, 2, 3 });
            results.Periods.Select(r => r.Status).Should().BeEquivalentTo(new[] { "Completed", "Closed", "Open" });
        }

        [Test]
        public void Map_CollectionYear_ShouldMapAllItems()
        {
            var openCollectionYears = new List<short> { 2023, 2024, 2025 };

            var results = _sut.MapToOpenCollectionYearResponseModel(openCollectionYears);

            results.Should().HaveCount(3);
            results.Select(r => r.Year).Should().BeEquivalentTo(new[] { 2023, 2024, 2025 });
            results.Select(r => r.Status).Should().BeEquivalentTo(new[] { "Open", "Open", "Open" });
        }
    }
}