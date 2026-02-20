using SFA.DAS.Payments.CollectionPeriod.Application.Mapping;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;

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

        public async Task UpdatePaymentsCollectionPeriodAsync(IEnumerable<SLDJobContextCollectionPeriodModel> collectionPeriods)
        {
            var mappedCollectionPeriods = _collectionPeriodMapper.MapCollectionPeriods(collectionPeriods);

            await _collectionPeriodRepository.UpdateCollectionPeriods(mappedCollectionPeriods);
        }
    }
}
