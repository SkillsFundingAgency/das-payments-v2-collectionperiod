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
        public List<CollectionPeriodModel> MapCollectionPeriods(List<SLDJobManagementAPICollectionPeriod> collectionPeriods);
    }

    public class CollectionPeriodMapper : ICollectionPeriodMapper
    {
        public List<CollectionPeriodModel> MapCollectionPeriods(List<SLDJobManagementAPICollectionPeriod> collectionPeriods)
        {
            return collectionPeriods.Select(cp => new CollectionPeriodModel
            {
                AcademicYear = cp.CollectionYear,
                Period = cp.PeriodNumber,
                IsOpen = cp.IsOpen
            }).ToList();
        }
    }
}
