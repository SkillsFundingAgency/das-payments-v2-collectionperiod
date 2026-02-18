using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class OpenAcademicYears
{
    private readonly ILogger<OpenAcademicYears> _logger;
    private readonly ICollectionPeriodRepository _collectionPeriodRepository;

    public OpenAcademicYears(ILogger<OpenAcademicYears> logger, ICollectionPeriodRepository collectionPeriodRepository)
    {
        _logger = logger;
        _collectionPeriodRepository = collectionPeriodRepository;
    }

    [Function("AcademicYears")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("OpenAcademicYears HTTP trigger function triggered.");

        var collectionPeriods = await _collectionPeriodRepository.GetOpenCollectionPeriods();

        _logger.LogInformation("OpenAcademicYears HTTP trigger function processed.");

        return collectionPeriods.Any() ? new OkObjectResult(collectionPeriods) : new NoContentResult();
    }
}