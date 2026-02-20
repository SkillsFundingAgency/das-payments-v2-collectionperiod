using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class AllCollectionPeriods
{
    private readonly ILogger<AllCollectionPeriods> _logger;
    private readonly ICollectionPeriodRepository _collectionPeriodRepository;

    public AllCollectionPeriods(ILogger<AllCollectionPeriods> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns the collection periods for all academic years, regardless of their status.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function("AllCollectionPeriods")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("AllCollectionPeriods HTTP trigger function triggered.");

        var collectionPeriods = await _collectionPeriodRepository.GetAllCollectionPeriods();

        _logger.LogInformation("AllCollectionPeriods HTTP trigger function processed.");

        return collectionPeriods.Any() ? new OkObjectResult(collectionPeriods) : new NoContentResult();
    }
}