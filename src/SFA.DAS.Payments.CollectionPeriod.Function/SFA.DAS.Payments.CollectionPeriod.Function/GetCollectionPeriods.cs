using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class GetCollectionPeriods
{
    private readonly ILogger _logger;
    private readonly ISLDJobManagementAPIService _sldJobManagementAPIService;
    private readonly IUpdatePaymentsCollectionPeriodService _updatePaymentsCollectionPeriodService;

    public GetCollectionPeriods(ILoggerFactory loggerFactory, ISLDJobManagementAPIService sldJobManagementAPIService, IUpdatePaymentsCollectionPeriodService updatePaymentsCollectionPeriodService)
    {
        _logger = loggerFactory.CreateLogger<GetCollectionPeriods>();
        _sldJobManagementAPIService = sldJobManagementAPIService;
        _updatePaymentsCollectionPeriodService = updatePaymentsCollectionPeriodService;
    }

    [Function("GetCollectionPeriods")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("GetCollectionPeriods Timer trigger function executed at: {executionTime}", DateTime.Now);

        var collectionPeriods = await _sldJobManagementAPIService.GetCollectionPeriods();

        if (collectionPeriods != null)
        {
            _updatePaymentsCollectionPeriodService.UpdatePaymentsCollectionPeriod(collectionPeriods);
        }
    }
}