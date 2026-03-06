using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class TestSLDAPIHttpTrigger
{
    private readonly ILogger<TestSLDAPIHttpTrigger> _logger;
    private readonly ISLDJobManagementAPIService sLDJobManagementAPIService;

    public TestSLDAPIHttpTrigger(ILogger<TestSLDAPIHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("TestSLDAPIHttpTrigger")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("TestSLDAPIHttpTrigger function processed a request.");

        var result = await sLDJobManagementAPIService.GetCollectionPeriods(DateTime.Now.ToString("yyyy-MM-dd"));

        return new OkObjectResult(result);
    }
}