using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Mappers
{
    public interface ICollectionPeriodMapper
    {

        public CollectionPeriodsForCollectionYearResponseModel MapToCollectionPeriodsForCollectionYearResponseModel(IEnumerable<CollectionPeriodModel> openCollectionPeriods, short year, CollectionPeriodStatus? status);

        public IEnumerable<CollectionYearResponseModel> MapToOpenCollectionYearResponseModel(IEnumerable<short> openCollectionYears);

        public CollectionPeriodResponseModel MapToCollectionPeriodResponseModel(CollectionPeriodModel collectionPeriod);
    }

    public class CollectionPeriodMapper : ICollectionPeriodMapper
    {
        // Response for /collectionyear/{year}
        public CollectionPeriodsForCollectionYearResponseModel MapToCollectionPeriodsForCollectionYearResponseModel(IEnumerable<CollectionPeriodModel> collectionPeriods, short year, CollectionPeriodStatus? status)
        {
            var mappedPeriods = collectionPeriods.Select(period => new CollectionPeriodResponseModel
            {
                Id = period.Id,
                Period = period.Period,
                CalendarMonth = period.CalendarMonth,
                CalendarYear = period.CalendarYear,
                Status = period.Status.HasValue ? period.Status.Value.ToString() : ""
            });

            return new CollectionPeriodsForCollectionYearResponseModel
            {
                Year = year,
                Status = status.HasValue ? status.Value.ToString() : "",
                Periods = mappedPeriods
            };
        }

        // Response for /collectionyear
        public IEnumerable<CollectionYearResponseModel> MapToOpenCollectionYearResponseModel(IEnumerable<short> openCollectionYears)
        {
            return openCollectionYears.Select(year => new CollectionYearResponseModel
            {
                Year = year,
                Status = "Open"
            });
        }

        // Response for collectionyear/{year}/collectionperiod/{id}
        public CollectionPeriodResponseModel MapToCollectionPeriodResponseModel(CollectionPeriodModel collectionPeriod)
        {
            return new CollectionPeriodResponseModel
            {
                Id = collectionPeriod.Id,
                Period = collectionPeriod.Period,
                CalendarMonth = collectionPeriod.CalendarMonth,
                CalendarYear = collectionPeriod.CalendarYear,
                Status = collectionPeriod.Status.HasValue ? collectionPeriod.Status.Value.ToString() : ""
            };
        }
    }
}
