using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Configuration;
using SFA.DAS.Payments.CollectionPeriod.Application.Handlers;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Processors;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.CollectionPeriod.Application.Validators;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Messaging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
 .AddApplicationInsightsTelemetryWorkerService()
 .ConfigureFunctionsApplicationInsights();

builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Services
    .AddOptions<CollectionPeriodServiceConfiguration>()
    .Bind(builder.Configuration)
    .ValidateOnStart();

builder.Services.AddDbContext<IPaymentsDataContext, PaymentsDataContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("PaymentsConnectionString"));
});

builder.Services.AddDbContext<IPeriodEndDataContext, PeriodEndDataContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("PaymentsConnectionString"));
});

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var tenantId = configuration["Values:TenantId"];
    var clientId = configuration["Values:ClientId"];
    var clientSecret = configuration["Values:ClientSecret"];

    if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        throw new Exception("Azure AD configuration missing");

    return new ClientSecretCredential(tenantId, clientId, clientSecret);
});

builder.Services.AddTransient<AzureAdTokenHandler>();

builder.Services.AddHttpClient<ISldJobManagementApiService, SldJobManagementApiService>((sp, client) =>
{
    var endpoint = Environment.GetEnvironmentVariable("SLDJobManagementAPIEndpoint");

    if (string.IsNullOrEmpty(endpoint))
        throw new Exception("SLDJobManagementAPIEndpoint is missing");

    client.BaseAddress = new Uri(endpoint);
})
.AddHttpMessageHandler<AzureAdTokenHandler>();



builder.Services.AddScoped<IPaymentsDataContext, PaymentsDataContext>();
builder.Services.AddScoped<ICollectionPeriodRepository, CollectionPeriodRepository>();
builder.Services.AddScoped<ICollectionPeriodFunctionProcessor, CollectionPeriodFunctionProcessor>();
builder.Services.AddScoped<ICollectionPeriodMapper, CollectionPeriodMapper>();
builder.Services.AddScoped<ICollectionPeriodHttpTriggerInputValidator, CollectionPeriodHttpTriggerInputValidator>();
builder.Services.AddScoped<ISyncCollectionPeriodMapper, SyncCollectionPeriodMapper>();
builder.Services.AddScoped<ISyncCollectionPeriodsProcessor, SyncCollectionPeriodsFunctionProcessor>();
builder.Services.AddScoped<IPeriodEndStoppedEventHandler, PeriodEndStoppedEventHandler>();
builder.Services.AddScoped<IPeriodEndRepository, PeriodEndRepository>();


builder.Services.AddHostedService<SetupMessagingInfrastructure>();


builder.Build().Run();
