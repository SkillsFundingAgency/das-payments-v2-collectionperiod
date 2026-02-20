using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Mapping
{
    public interface ICollectionPeriodMapper
    {
        IEnumerable<CollectionPeriodModel> MapCollectionPeriods(IEnumerable<SLDJobContextCollectionPeriodModel> collectionPeriods);
    }

    public class CollectionPeriodMapper : ICollectionPeriodMapper
    {
        public IEnumerable<CollectionPeriodModel> MapCollectionPeriods(IEnumerable<SLDJobContextCollectionPeriodModel> collectionPeriods)
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
                case false when DateTime.UtcNow > cp.EndDateTimeUtc:
                    return CollectionPeriodStatus.Completed;
                case true:
                    return CollectionPeriodStatus.Open;
                default:
                    return null;
            }
        }
    }
}
