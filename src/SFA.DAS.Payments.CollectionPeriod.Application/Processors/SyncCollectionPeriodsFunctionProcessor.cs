using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Processors
{
    public interface ISyncCollectionPeriodsProcessor
    {
        Task Process();
    }

    public class SyncCollectionPeriodsFunctionProcessor : ISyncCollectionPeriodsProcessor
    {
        private readonly ISLDJobManagementAPIService _sldJobManagementAPIService;
        private readonly ISyncCollectionPeriodMapper _mapper;
        private readonly ILogger<SyncCollectionPeriodsFunctionProcessor> _logger;
        private readonly ICollectionPeriodRepository _collectionPeriodRepository;

        public SyncCollectionPeriodsFunctionProcessor(ISLDJobManagementAPIService sldJobManagementAPIService, ISyncCollectionPeriodMapper syncCollectionPeriodMapper, ICollectionPeriodRepository collectionPeriodRepository, ILogger<SyncCollectionPeriodsFunctionProcessor> logger)
        {
            _sldJobManagementAPIService = sldJobManagementAPIService;
            _mapper = syncCollectionPeriodMapper;
            _collectionPeriodRepository = collectionPeriodRepository;
            _logger = logger;
        }   

        public async Task Process()
        {
            var queryCollectionYear = _collectionPeriodRepository.GetCurrentCollectionYear();

            var collectionPeriods = await _sldJobManagementAPIService.GetCollectionPeriods(queryCollectionYear);

            if (collectionPeriods != null && collectionPeriods.Any())
            {
                var mappedCollectionPeriods = _mapper.MapToPaymentsDBCollectionPeriods(collectionPeriods);

                _logger.LogInformation("Processed SyncCollectionPeriods");

                await _collectionPeriodRepository.UpdateCollectionPeriods(mappedCollectionPeriods);
            }
        }
    }
}
