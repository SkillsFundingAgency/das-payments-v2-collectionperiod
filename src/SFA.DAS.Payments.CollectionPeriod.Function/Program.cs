using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Processors;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Validators;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
 .AddApplicationInsightsTelemetryWorkerService()
 .ConfigureFunctionsApplicationInsights();


builder.Services.AddDbContext<IPaymentsDataContext, PaymentsDataContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("PaymentsConnectionString"));
}); 

builder.Services.AddHttpClient<SLDJobManagementAPIService>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SLDJobManagementAPIEndpoint"));
});

builder.Services.AddScoped<IPaymentsDataContext, PaymentsDataContext>();
builder.Services.AddScoped<ICollectionPeriodRepository, CollectionPeriodRepository>();
builder.Services.AddScoped<ICollectionPeriodFunctionProcessor, CollectionPeriodFunctionProcessor>();
builder.Services.AddScoped<ICollectionPeriodMapper, CollectionPeriodMapper>();
builder.Services.AddScoped<ICollectionPeriodHttpTriggerInputValidator, CollectionPeriodHttpTriggerInputValidator>();
builder.Services.AddScoped<ISyncCollectionPeriodMapper, SyncCollectionPeriodMapper>();
builder.Services.AddScoped<ISyncCollectionPeriodsProcessor, SyncCollectionPeriodsFunctionProcessor>();
builder.Services.AddScoped<ISLDJobManagementAPIService, SLDJobManagementAPIService>();

builder.Services.AddHostedService<SetupMessagingInfrastructure>(
    serviceProvider => new SetupMessagingInfrastructure(
        serviceProvider.GetRequiredService<ILogger<SetupMessagingInfrastructure>>(),
        serviceProvider.GetRequiredService<IConfiguration>()
    )
);

builder.Build().Run();
