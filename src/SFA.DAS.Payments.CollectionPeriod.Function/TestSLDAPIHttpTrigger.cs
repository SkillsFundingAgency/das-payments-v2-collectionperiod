using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class TestSLDAPIHttpTrigger
{
    private readonly ILogger<TestSLDAPIHttpTrigger> _logger;
    private readonly SLDJobManagementAPIService _sLDJobManagementAPIService;

    public TestSLDAPIHttpTrigger(ILogger<TestSLDAPIHttpTrigger> logger, SLDJobManagementAPIService sLDJobManagementAPIService)
    {
        _logger = logger;
        _sLDJobManagementAPIService = sLDJobManagementAPIService;
    }

    /// <summary>
    /// This is a test function to verify connectivity to the SLD Job Management API. 
    /// Make sure to remove after integration is tested and working as expected.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function("TestSLDAPIHttpTrigger")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("TestSLDAPIHttpTrigger function processed a request.");

        var result = await _sLDJobManagementAPIService.GetCollectionPeriods(2526);

        return new OkObjectResult(result);
    }
}
