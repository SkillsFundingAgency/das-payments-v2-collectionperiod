using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Processors
{
    public interface ISyncCollectionPeriodsProcessor
    {
        Task Process();
    }

    public class SyncCollectionPeriodsProcessor : ISyncCollectionPeriodsProcessor
    {
        private readonly SLDJobManagementAPIService _sldJobManagementAPIService;
        private readonly UpdatePaymentsCollectionPeriodService _updatePaymentsCollectionPeriodService;

        public SyncCollectionPeriodsProcessor(SLDJobManagementAPIService sldJobManagementAPIService, UpdatePaymentsCollectionPeriodService updatePaymentsCollectionPeriodService)
        {
            _sldJobManagementAPIService = sldJobManagementAPIService;
            _updatePaymentsCollectionPeriodService = updatePaymentsCollectionPeriodService;
        }   

        public async Task Process()
        {
            var collectionPeriods = await _sldJobManagementAPIService.GetCollectionPeriods(DateTime.Today.ToString("yyyy-MM-dd"));

            if (collectionPeriods != null && collectionPeriods.Any())
            {
                await _updatePaymentsCollectionPeriodService.UpdatePaymentsCollectionPeriodAsync(collectionPeriods);
            }
        }
    }
}
