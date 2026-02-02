using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class CollectionPeriodsByAcademicYear
{
    private readonly ILogger<CollectionPeriodsByAcademicYear> _logger;
    private readonly ICollectionPeriodRepository _collectionPeriodRepository;

    public CollectionPeriodsByAcademicYear(ILogger<CollectionPeriodsByAcademicYear> logger, ICollectionPeriodRepository collectionPeriodRepository)
    {
        _logger = logger;
        _collectionPeriodRepository = collectionPeriodRepository;
    }

    [Function("CollectionPeriodsByAcademicYear")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        short.TryParse(req.Query["academicYear"], out var academicYear);

        _logger.LogInformation("CollectionPeriodsByAcademicYear HTTP trigger function for academic year: {academicYear}.", academicYear);

        var collectionPeriods = await _collectionPeriodRepository.GetCollectionPeriodByAcademicYear(academicYear);

        _logger.LogInformation("CollectionPeriodsByAcademicYear processed for academic year: {academicYear}.", academicYear);
        return new OkObjectResult(collectionPeriods);
    }
}