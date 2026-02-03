using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
 .AddApplicationInsightsTelemetryWorkerService()
 .ConfigureFunctionsApplicationInsights();


builder.Services.AddDbContext<IPaymentsDataContext, PaymentsDataContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("PaymentsConnectionString"));
});

builder.Services.AddScoped<IPaymentsDataContext, PaymentsDataContext>();
builder.Services.AddScoped<ICollectionPeriodRepository, CollectionPeriodRepository>();
  
builder.Build().Run();
