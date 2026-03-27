using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Mappers
{
    public interface ISyncCollectionPeriodMapper
    {
        IEnumerable<CollectionPeriodModel> MapToPaymentsDBCollectionPeriods(IEnumerable<SLDJobContextCollectionPeriodModel> collectionPeriods);
    }

    public class SyncCollectionPeriodMapper : ISyncCollectionPeriodMapper
    {

        public IEnumerable<CollectionPeriodModel> MapToPaymentsDBCollectionPeriods(IEnumerable<SLDJobContextCollectionPeriodModel> collectionPeriods)
        {
            return collectionPeriods.Select(cp => new CollectionPeriodModel
            {
                AcademicYear = cp.CollectionYear,
                Period = cp.PeriodNumber,
                Status = MapCollectionPeriodStaus(cp)
            });
        }

        private CollectionPeriodStatus? MapCollectionPeriodStaus(SLDJobContextCollectionPeriodModel cp)
        {
            switch (cp.IsOpen)
            {
                case false when DateTime.UtcNow < cp.StartDateTimeUtc:
                    return CollectionPeriodStatus.NotStarted;
                case false when DateTime.UtcNow > cp.StartDateTimeUtc:
                    return CollectionPeriodStatus.Closed;
                case true:
                    return CollectionPeriodStatus.Open;
                default:
                    return null;
            }
        }

    }
}
