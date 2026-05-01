using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public interface ISldJobManagementApiService
    {
        Task<IEnumerable<SLDJobContextCollectionPeriodModel>> GetCollectionPeriods(short fromCollectionYear);
        Task<HttpClient> CreateAadHttpClient();
    }

    public class SldJobManagementApiService : ISldJobManagementApiService
    {
        private readonly IConfiguration _config;
        private readonly ISetupAzureAdInfrastructure _azureAdInfrastructure;
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
                var httpClient = await CreateAadHttpClient();

                var sldResponse = await httpClient.GetAsync($"returnperiods/?fromCollectionYear={fromCollectionYear}");

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

        public async Task<HttpClient> CreateAadHttpClient()
        {
            try
            {
                var credential = _azureAdInfrastructure.GetAzureAdConfig();
                var token = await _azureAdInfrastructure.GetAzureAdToken(credential);

                var endpoint = _config["SLDJobManagementAPIEndpoint"];
                
                if(string.IsNullOrEmpty(endpoint))
                {
                    throw new Exception("SLD Job Management API endpoint is not configured.");
                }

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(endpoint)
                };

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                return httpClient;

            } 
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating Azure AD authenticated HttpClient. Exception: {ex.Message}");
                throw;
            }

        }
    }
}
