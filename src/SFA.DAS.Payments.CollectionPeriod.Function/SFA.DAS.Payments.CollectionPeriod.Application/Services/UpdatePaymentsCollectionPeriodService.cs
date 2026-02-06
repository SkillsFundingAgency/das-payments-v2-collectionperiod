using SFA.DAS.Payments.CollectionPeriod.Application.Mapping;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public interface IUpdatePaymentsCollectionPeriodService
    {
        public void UpdatePaymentsCollectionPeriod(List<SLDJobManagementAPICollectionPeriod> collectionPeriods);
        public void UpdatePaymentsCollectionPeriod(PeriodEndStoppedEvent periodEndStoppedEvent);
    }

    public class UpdatePaymentsCollectionPeriodService : IUpdatePaymentsCollectionPeriodService
    {
        public readonly ICollectionPeriodRepository _collectionPeriodRepository;
        public readonly ICollectionPeriodMapper _collectionPeriodMapper;

        public UpdatePaymentsCollectionPeriodService(ICollectionPeriodRepository collectionPeriodRepository, ICollectionPeriodMapper collectionPeriodMapper)
        {
            _collectionPeriodRepository = collectionPeriodRepository;
            _collectionPeriodMapper = collectionPeriodMapper;
        }

        public void UpdatePaymentsCollectionPeriod(List<SLDJobManagementAPICollectionPeriod> collectionPeriods)
        {
            var mappedCollectionPeriods = _collectionPeriodMapper.MapCollectionPeriods(collectionPeriods);

            _collectionPeriodRepository.UpdateCollectionPeriods(mappedCollectionPeriods);
        }

        public void UpdatePaymentsCollectionPeriod(PeriodEndStoppedEvent periodEndStoppedEvent)
        {
            _collectionPeriodRepository.UpdateCollectionPeriodOnPeriodEndStopped(periodEndStoppedEvent.CollectionPeriod.AcademicYear, periodEndStoppedEvent.CollectionPeriod.Period);
        }
    }
}
