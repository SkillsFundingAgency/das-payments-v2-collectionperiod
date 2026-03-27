using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Validators;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Validators;
internal class CollectionPeriodHttpTriggerInputValidatorTests
{
    private CollectionPeriodHttpTriggerInputValidator _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new CollectionPeriodHttpTriggerInputValidator();
    }

    [Test]
    public void ValidateCollectionPeriod_Does_Not_Throw_Argument_Exception_When_Collection_Period_Is_Valid()
    {
        short validCollectionPeriod = 1;
        Action act = () => _sut.ValidateCollectionPeriod(validCollectionPeriod);

        act.Should().NotThrow<ArgumentException>(); 
    }

    [Test]
    public void ValidateCollectionPeriod_Throws_Argument_Exception_When_Collection_Period_Is_Invalid()
    {
        short invalidCollectionPeriod = 0;
        Action act = () => _sut.ValidateCollectionPeriod(invalidCollectionPeriod);


        act.Should().Throw<ArgumentException>()
            .WithMessage("Collection period must be between 1 and 14.");
    }

    [TestCase(null)]
    [TestCase("")]
    public void ValidateStatus_Returns_Null_When_Status_Is_Null_Or_Empty(string nullOrEmptyStatus)
    {
        _sut.ValidateStatus(nullOrEmptyStatus).Should().BeNull();
    }

    [TestCase("Open", CollectionPeriodStatus.Open)]
    [TestCase("Closed", CollectionPeriodStatus.Closed)]
    [TestCase("NotStarted", CollectionPeriodStatus.NotStarted)]
    [TestCase("Completed", CollectionPeriodStatus.Completed)]
    public void ValidateStatus_Returns_Parsed_Status_For_Valid_Collection_Period_Status(string validStatus,
        CollectionPeriodStatus expectedStatus)
    {
        _sut.ValidateStatus(validStatus).Should().Be(expectedStatus);
    }

    [Test]
    public void ValidateStatus_Throws_An_Argument_Exception_When_Status_Is_Invalid()
    {
        var invalidStatus = "example";
        Action act = () => _sut.ValidateStatus(invalidStatus);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid status value: example. Valid values are Open, Closed, Not Started, Completed.");
    }

    [Test]
    public void
        ValidateCollectionYearAndCollectionPeriod_Throws_An_Argument_Exception_When_CollectionYear_Has_A_Value_But_Period_Does_Not()
    {
        short collectionYear = 2526;
        short? period = null;
        var act = () => _sut.ValidateCollectionYearAndCollectionPeriod(collectionYear, period);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Collection Period is required when Year is specified.");
    }

    [Test]
    public void
        ValidateCollectionYearAndCollectionPeriod_Throws_An_Argument_Exception_When_CollectionYear_Does_Not_Have_A_Value_But_Period_Does()
    {
        short? collectionYear = null;
        short period = 1;
        Action act = () => _sut.ValidateCollectionYearAndCollectionPeriod(collectionYear, period);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Collection Year is required when Period is specified.");
    }

    [Test]
    public void
        ValidateCollectionYearAndCollectionPeriod_Does_Not_Throw_An_Argument_Exception_When_Both_CollectionYear_And_Period_Have_Valid_Values()
    {
        short collectionYear = 2526;
        short period = 1;
        Action act = () => _sut.ValidateCollectionYearAndCollectionPeriod(collectionYear, period);
        act.Should().NotThrow<ArgumentException>();
    }
}