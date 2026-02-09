using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class GetCollectionPeriods
{
    private readonly ILogger _logger;
    private readonly SLDJobManagementAPIService _sldJobManagementAPIService;
    private readonly UpdatePaymentsCollectionPeriodService _updatePaymentsCollectionPeriodService;

    public GetCollectionPeriods(ILoggerFactory loggerFactory, SLDJobManagementAPIService sldJobManagementAPIService, UpdatePaymentsCollectionPeriodService updatePaymentsCollectionPeriodService)
    {
        _logger = loggerFactory.CreateLogger<GetCollectionPeriods>();
        _sldJobManagementAPIService = sldJobManagementAPIService;
        _updatePaymentsCollectionPeriodService = updatePaymentsCollectionPeriodService;
    }

    /// <summary>
    /// Timer trigger function to get collection periods from SLD Job Management API and update payments collection period in the database.
    /// Schedule is configured via app settings key 'GetCollectionPeriodsSchedule'.
    /// </summary>
    /// <param name="sldAPIQueryTimer"></param>
    /// <returns></returns>
    [Function("GetCollectionPeriods")]
    public async Task Run([TimerTrigger("%GetCollectionPeriodsSchedule%")] TimerInfo sldAPIQueryTimer)
    {
        _logger.LogInformation("GetCollectionPeriods Timer trigger function executed at: {executionTime}", DateTime.Now);

        var collectionPeriods = await _sldJobManagementAPIService.GetCollectionPeriods(DateTime.Today.ToString("yyyy-MM-dd"));

        if (collectionPeriods != null)
        {
            await _updatePaymentsCollectionPeriodService.UpdatePaymentsCollectionPeriodAsync(collectionPeriods);
        }
    }
}