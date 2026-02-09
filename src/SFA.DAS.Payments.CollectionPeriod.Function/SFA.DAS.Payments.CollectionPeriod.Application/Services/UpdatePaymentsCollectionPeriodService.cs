using SFA.DAS.Payments.CollectionPeriod.Application.Mapping;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public class UpdatePaymentsCollectionPeriodService
    {
        public readonly ICollectionPeriodRepository _collectionPeriodRepository;
        public readonly ICollectionPeriodMapper _collectionPeriodMapper;

        public UpdatePaymentsCollectionPeriodService(ICollectionPeriodRepository collectionPeriodRepository, ICollectionPeriodMapper collectionPeriodMapper)
        {
            _collectionPeriodRepository = collectionPeriodRepository;
            _collectionPeriodMapper = collectionPeriodMapper;
        }

        public async Task UpdatePaymentsCollectionPeriodAsync(List<SLDJobManagementAPICollectionPeriod> collectionPeriods)
        {
            var mappedCollectionPeriods = _collectionPeriodMapper.MapCollectionPeriods(collectionPeriods);

            await _collectionPeriodRepository.UpdateCollectionPeriods(mappedCollectionPeriods);
        }
    }
}
