using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Processors;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.UnitTests.Processors
{
    [TestFixture]
    public class CollectionPeriodFunctionProcessorTests
    {
        private Mock<ICollectionPeriodRepository> _mockRepository;
        private Mock<ILogger<CollectionPeriodFunctionProcessor>> _mockLogger;
        private Mock<ICollectionPeriodMapper> _mockMapper;
        private CollectionPeriodFunctionProcessor _sut;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ICollectionPeriodRepository>();
            _mockLogger = new Mock<ILogger<CollectionPeriodFunctionProcessor>>();
            _mockMapper = new Mock<ICollectionPeriodMapper>();
            _sut = new CollectionPeriodFunctionProcessor(_mockRepository.Object, _mockLogger.Object, _mockMapper.Object);
        }

        #region ProcessOpenCollectionYears

        [Test]
        public async Task ProcessOpenCollectionYears_ShouldReturnMappedResult_WhenOpenCollectionYearsExist()
        {
            var openYears = new List<short> { 2023, 2024 };
            var expectedResponse = new List<CollectionYearResponseModel>
             {
                new CollectionYearResponseModel { Year = 2023, Status = "Open" },
                new CollectionYearResponseModel { Year = 2024, Status = "Open" }
            };

            _mockRepository.Setup(r => r.OpenCollectionYears()).ReturnsAsync(openYears);
            _mockMapper.Setup(m => m.MapToOpenCollectionYearResponseModel(openYears)).Returns(expectedResponse);

            var result = await _sut.ProcessCollectionYear();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);

            _mockRepository.Verify(r => r.OpenCollectionYears(), Times.Once);
            _mockMapper.Verify(m => m.MapToOpenCollectionYearResponseModel(openYears), Times.Once);
        }

        [Test]
        public async Task ProcessOpenCollectionYears_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            _mockRepository.Setup(r => r.OpenCollectionYears()).ReturnsAsync((IEnumerable<short>)null);

            var result = await _sut.ProcessCollectionYear();

            result.Should().BeNull();
            _mockMapper.Verify(m => m.MapToOpenCollectionYearResponseModel(It.IsAny<IEnumerable<short>>()), Times.Never);
        }

        [Test]
        public async Task ProcessOpenCollectionYears_ShouldReturnNull_WhenRepositoryReturnsEmptyCollection()
        {
            _mockRepository.Setup(r => r.OpenCollectionYears()).ReturnsAsync(Enumerable.Empty<short>());

            var result = await _sut.ProcessCollectionYear();

            result.Should().BeNull();
            _mockMapper.Verify(m => m.MapToOpenCollectionYearResponseModel(It.IsAny<IEnumerable<short>>()), Times.Never);
        }

        #endregion

        #region ProcessCollectionYear

        [Test]
        public async Task ProcessCollectionYear_ShouldReturnMappedResult_WhenCollectionPeriodsExist()
        {
            short collectionYear = 2324;
            CollectionPeriodStatus? status = CollectionPeriodStatus.Open;

            var collectionPeriods = new List<CollectionPeriodModel>
            {
                new CollectionPeriodModel { 
                    Id = 1, Period = 1, 
                    AcademicYear = 2324, 
                    Status = CollectionPeriodStatus.Open, 
                    CalendarMonth = 8, 
                    CalendarYear = 2023 
                },
                new CollectionPeriodModel { 
                    Id = 2, 
                    Period = 2, 
                    AcademicYear = 2425, 
                    Status = CollectionPeriodStatus.Open, 
                    CalendarMonth = 9, 
                    CalendarYear = 2025 
                },
                new CollectionPeriodModel { 
                    Id = 3, 
                    Period = 3, 
                    AcademicYear = 2324, 
                    Status = CollectionPeriodStatus.Closed, 
                    CalendarMonth = 9, 
                    CalendarYear = 2023 
                }
            };

            var expectedResponse = new CollectionPeriodsForCollectionYearResponseModel
            {
                Year = collectionYear,
                Status = "Open",
                Periods = new List<CollectionPeriodResponseModel>
                {
                    new CollectionPeriodResponseModel { 
                        Id = 1, 
                        Period = 1, 
                        CalendarMonth = 8, 
                        CalendarYear = 2023, 
                        Status = "Open" 
                    },
                    new CollectionPeriodResponseModel { 
                        Id = 2, 
                        Period = 2, 
                        CalendarMonth = 9, 
                        CalendarYear = 2025, 
                        Status = "Open" 
                    }
                }
            };

            _mockRepository.Setup(r => r.CollectionYear(collectionYear, status)).ReturnsAsync(collectionPeriods);
            _mockMapper.Setup(m => m.MapToCollectionPeriodsForCollectionYearResponseModel(collectionPeriods, collectionYear, status)).Returns(expectedResponse);

            var result = await _sut.ProcessCollectionYear(collectionYear, status);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            _mockRepository.Verify(r => r.CollectionYear(collectionYear, status), Times.Once);
            _mockMapper.Verify(m => m.MapToCollectionPeriodsForCollectionYearResponseModel(collectionPeriods, collectionYear, status), Times.Once);
        }

        [Test]
        public async Task ProcessCollectionYear_ShouldReturnMappedResult_WhenStatusIsNull()
        {
            short collectionYear = 2324;
            CollectionPeriodStatus? status = null;

            var collectionPeriods = new List<CollectionPeriodModel>
            {
                new CollectionPeriodModel { 
                    Id = 1, 
                    Period = 1, 
                    AcademicYear = 2324, 
                    Status = CollectionPeriodStatus.Open 
                },
                new CollectionPeriodModel { 
                    Id = 2, 
                    Period = 2, 
                    AcademicYear = 2324, 
                    Status = CollectionPeriodStatus.Closed 
                }
            };

            var expectedResponse = new CollectionPeriodsForCollectionYearResponseModel
            {
                Year = collectionYear,
                Periods = new List<CollectionPeriodResponseModel>
                {
                    new CollectionPeriodResponseModel { 
                        Id = 1, 
                        Period = 1, 
                        Status = "Open" 
                    },
                    new CollectionPeriodResponseModel { 
                        Id = 2, 
                        Period = 2, 
                        Status = "Closed" 
                    }
                }
            };

            _mockRepository.Setup(r => r.CollectionYear(collectionYear, status)).ReturnsAsync(collectionPeriods);
            _mockMapper.Setup(m => m.MapToCollectionPeriodsForCollectionYearResponseModel(collectionPeriods, collectionYear, status)).Returns(expectedResponse);

            var result = await _sut.ProcessCollectionYear(collectionYear, status);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        #endregion

        #region ProcessCollectionPeriod

        [Test]
        public async Task ProcessCollectionPeriod_ShouldReturnMappedResult_WhenCollectionPeriodExists()
        {
            // Arrange
            short collectionYear = 2324;
            short period = 5;

            var collectionPeriod = new CollectionPeriodModel
            {
                Id = 1,
                Period = 5,
                AcademicYear = 2324,
                Status = CollectionPeriodStatus.Open,
                CalendarMonth = 12,
                CalendarYear = 2023
            };

            var expectedResponse = new CollectionPeriodResponseModel
            {
                Id = 1,
                Period = 5,
                CalendarMonth = 12,
                CalendarYear = 2023,
                Status = "Open"
            };

            _mockRepository.Setup(r => r.CollectionPeriodForCollectionYear(collectionYear, period)).ReturnsAsync(collectionPeriod);
            _mockMapper.Setup(m => m.MapToCollectionPeriodResponseModel(collectionPeriod)).Returns(expectedResponse);

            // Act
            var result = await _sut.ProcessCollectionPeriod(collectionYear, period);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            _mockRepository.Verify(r => r.CollectionPeriodForCollectionYear(collectionYear, period), Times.Once);
            _mockMapper.Verify(m => m.MapToCollectionPeriodResponseModel(collectionPeriod), Times.Once);
        }

        [Test]
        public async Task ProcessCollectionPeriod_ShouldNotCallMapper_WhenCollectionPeriodIsNull()
        {
            // Arrange
            short collectionYear = 2324;
            short period = 99;

            _mockRepository.Setup(r => r.CollectionPeriodForCollectionYear(collectionYear, period)).ReturnsAsync((CollectionPeriodModel)null);

            // Act
            await _sut.ProcessCollectionPeriod(collectionYear, period);

            // Assert
            _mockMapper.Verify(m => m.MapToCollectionPeriodResponseModel(It.IsAny<CollectionPeriodModel>()), Times.Never);
        }

        #endregion
    }
}
