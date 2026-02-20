using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Processors;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class SyncCollectionPeriodsTimerTrigger
{
    private readonly ILogger _logger;
    private readonly ISyncCollectionPeriodsProcessor _syncCollectionPeriodsProcessor;

    public SyncCollectionPeriodsTimerTrigger(ILoggerFactory loggerFactory, ISyncCollectionPeriodsProcessor syncCollectionPeriodsProcessor)
    {
        _logger = loggerFactory.CreateLogger<SyncCollectionPeriodsTimerTrigger>();
        _syncCollectionPeriodsProcessor = syncCollectionPeriodsProcessor;
    }

    /// <summary>
    /// Timer trigger function to get collection periods from SLD Job Context API and update payments collection period in the database.
    /// Schedule is configured via app settings key 'SyncCollectionPeriodsSchedule'.
    /// </summary>
    /// <param name="sldAPIQueryTimer"></param>
    /// <returns></returns>
    [Function("SyncCollectionPeriods")]
    public async Task Run([TimerTrigger("%SyncCollectionPeriodsSchedule%")] TimerInfo sldAPIQueryTimer)
    {
        _logger.LogInformation("SyncCollectionPeriods Timer trigger function executed at: {executionTime}", DateTime.Now);

        await _syncCollectionPeriodsProcessor.Process();

        _logger.LogInformation("SyncCollectionPeriods Timer trigger function completed at: {completionTime}", DateTime.Now);
    }
}