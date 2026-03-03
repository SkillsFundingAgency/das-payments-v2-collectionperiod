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
        //ToDo - WIP

        //private CollectionPeriodMapper _sut;

        //[SetUp]
        //public void SetUp()
        //{
        //    _sut = new CollectionPeriodMapper();
        //}

        //[Test]
        //public void Map_ShouldMapAllProperties_WhenSourceIsValid()
        //{
        //    // Arrange
        //    var source = new CollectionPeriodModel
        //    {
        //        Id = 1,
        //        Period = 5,
        //        AcademicYear = 2324,
        //        ReferenceDataValidationDate = new DateTime(2024, 1, 15),
        //        CompletionDate = new DateTime(2024, 2, 28),
        //        Status = CollectionPeriodStatus.Open,
        //        CalendarMonth = 2,
        //        CalendarYear = 2023
        //    };

        //    // Act
        //    var result = _sut.MapToCollectionPeriodResponseModel(source);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Period.Should().Be(source.Period);
        //    result.CalendarYear.Should().Be(source.CalendarYear);
        //    result.CalendarMonth.Should().Be(source.CalendarMonth);
        //    result.Status.Should().Be(source.Status.Value.ToString());
        //}

        //[Test]
        //public void Map_ShouldThrowArgumentNullException_WhenSourceIsNull()
        //{
        //    // Act
        //    Action act = () => _sut.Map(null);

        //    // Assert
        //    act.Should().Throw<ArgumentNullException>();
        //}

        //[Test]
        //public void MapList_ShouldMapAllItems_WhenSourceListIsValid()
        //{
        //    // Arrange
        //    var sources = new List<CollectionPeriodModel>
        //    {
        //        new CollectionPeriodModel { Id = 1, Period = 1, AcademicYear = 2324, OpenDate = new DateTime(2023, 8, 1), CloseDate = new DateTime(2023, 9, 5) },
        //        new CollectionPeriodModel { Id = 2, Period = 2, AcademicYear = 2324, OpenDate = new DateTime(2023, 9, 1), CloseDate = new DateTime(2023, 10, 5) },
        //        new CollectionPeriodModel { Id = 3, Period = 3, AcademicYear = 2324, OpenDate = new DateTime(2023, 10, 1), CloseDate = new DateTime(2023, 11, 5) }
        //    };

        //    // Act
        //    var results = _sut.Map(sources);

        //    // Assert
        //    results.Should().HaveCount(3);
        //    results.Select(r => r.Period).Should().BeEquivalentTo(new[] { 1, 2, 3 });
        //}

        //[Test]
        //public void Map_ShouldHandleNullableDates_WhenDatesAreNull()
        //{
        //    // Arrange
        //    var source = new CollectionPeriodModel
        //    {
        //        Id = 1,
        //        Period = 1,
        //        AcademicYear = 2324,
        //        ReferenceDataValidationDate = null,
        //        CompletionDate = null,
        //        SignOffDate = null
        //    };

        //    // Act
        //    var result = _sut.Map(source);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.ReferenceDataValidationDate.Should().BeNull();
        //    result.CompletionDate.Should().BeNull();
        //    result.SignOffDate.Should().BeNull();
        //}

        //[Test]
        //public void MapList_ShouldReturnEmptyList_WhenSourceListIsEmpty()
        //{
        //    // Arrange
        //    var sources = new List<CollectionPeriodModel>();

        //    // Act
        //    var results = _sut.Map(sources);

        //    // Assert
        //    results.Should().BeEmpty();
        //}
    }
}