using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Processors;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Validators;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class CollectionPeriodHttpTrigger
{
    private readonly ILogger<CollectionPeriodHttpTrigger> _logger;
    private readonly ICollectionPeriodFunctionProcessor _processor;
    private readonly ICollectionPeriodHttpTriggerInputValidator _validator;

    public CollectionPeriodHttpTrigger(ILogger<CollectionPeriodHttpTrigger> logger, ICollectionPeriodFunctionProcessor processor, ICollectionPeriodHttpTriggerInputValidator validator)
    {
        _logger = logger;
        _processor = processor;
        _validator = validator;
    }

    [Function("CollectionYears")]
    public async Task<IActionResult> HandleCollectionYearsRoute([HttpTrigger(AuthorizationLevel.Function, "get", Route = "collectionYear")] HttpRequest req)
    {
        _logger.LogInformation("CollectionYear HTTP trigger function triggered - CollectionYear");

        var responseCollectionYears = await _processor.ProcessCollectionYear();

        return responseCollectionYears == null
        ? new NoContentResult()
        : new OkObjectResult(responseCollectionYears);
    }

    [Function("CollectionYear")]
    public async Task<IActionResult> HandleCollectionYearRoute([HttpTrigger(AuthorizationLevel.Function, "get", Route = "collectionYear/{collectionYear}")] HttpRequest req, short collectionYear)
    {
        _logger.LogInformation("CollectionYears HTTP trigger function triggered - CollectionYear");

        CollectionPeriodStatus? status = null;

        try
        {
            _validator.ValidateCollectionYear(collectionYear);
            status = _validator.ValidateStatus(req.Query["status"]);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }

        var responsePeriods = await _processor.ProcessCollectionYear(collectionYear, status);

        return responsePeriods == null
            ? new NoContentResult()
            : new OkObjectResult(responsePeriods);
    }

    [Function("CollectionPeriod")]
    public async Task<IActionResult> HandleCollectionPeriod([HttpTrigger(AuthorizationLevel.Function, "get", Route = "collectionYear/{collectionYear?}/collectionPeriod/{period?}")] HttpRequest req, short? collectionYear, short? period)
    {
        _logger.LogInformation("CollectionYears HTTP trigger function triggered - CollectionPeriod");

        try
        {
            _validator.ValidateCollectionYearAndCollectionPeriod(collectionYear, period);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }

        var responsePeriod = await _processor.ProcessCollectionPeriod(collectionYear.Value, period.Value);

        return responsePeriod == null
            ? new NoContentResult()
            : new OkObjectResult(responsePeriod);
    }

    [Function("CollectionPeriodStatuses")]
    public async Task<IActionResult> CollectionPeriodStatuses([HttpTrigger(AuthorizationLevel.Function, "get", Route = "collectionPeriodStatuses")] HttpRequest req)
    {
        _logger.LogInformation("CollectionYears HTTP trigger function triggered - CollectionPeriodStatuses");

        return new OkObjectResult(new CollectionPeriodStatusesResponseModel().Status);
    }
}