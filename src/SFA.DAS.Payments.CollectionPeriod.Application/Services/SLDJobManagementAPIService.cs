using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using System.Net.Http.Json;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public interface ISldJobManagementApiService
    {
        Task<IEnumerable<SLDJobContextCollectionPeriodModel>> GetCollectionPeriods(short fromCollectionYear);
        HttpClient CreateAadHttpClient();
    }

    public class SldJobManagementApiService : ISldJobManagementApiService
    {
        private IConfiguration _config;
        private ISetupAzureAdInfrastructure _azureAdInfrastructure;
        private readonly ILogger<SldJobManagementApiService> _logger;

        public SldJobManagementApiService(IConfiguration config, ISetupAzureAdInfrastructure azureAdInfrastructure, ILogger<SldJobManagementApiService> logger)
        {
            _config = config;
            _azureAdInfrastructure = azureAdInfrastructure;
            _logger = logger;
        }

        public async Task<IEnumerable<SLDJobContextCollectionPeriodModel>> GetCollectionPeriods(short fromCollectionYear)
        {           
            try
            {
                var httpClient = CreateAadHttpClient();

                var sldResponse = await httpClient.GetAsync($"/api/returnperiods/?fromCollectionYear={fromCollectionYear}/");

                if (sldResponse.IsSuccessStatusCode)
                {
                    var result = await sldResponse.Content.ReadFromJsonAsync<IEnumerable<SLDJobContextCollectionPeriodModel>>();
                    return result ?? Enumerable.Empty<SLDJobContextCollectionPeriodModel>();
                }

                return Enumerable.Empty<SLDJobContextCollectionPeriodModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while calling SLD Job Context API to get collection periods for fromCollectionYear: {fromCollectionYear}. Exception: {ex.Message}");
                throw;
            }
        }

        public HttpClient CreateAadHttpClient()
        {
            var azureAdConfig = _azureAdInfrastructure.GetAzureAdConfig();

            //TODO: Add call to get token using azureAdConfig.

            //Return HttpClient with token in header for authentication to SLD Job Context API.

            // Add call 
            throw new NotImplementedException();
        }
    }
}
