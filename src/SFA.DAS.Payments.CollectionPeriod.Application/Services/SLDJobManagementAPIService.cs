using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using System.Net.Http.Json;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public interface ISldJobManagementApiService
    {
        Task<IEnumerable<SLDJobContextCollectionPeriodModel>> GetCollectionPeriods(short fromCollectionYear);
    }

    public class SldJobManagementApiService : ISldJobManagementApiService
    {
        private readonly ILogger<SldJobManagementApiService> _logger;
        private readonly HttpClient _httpClient;


        public SldJobManagementApiService(ILogger<SldJobManagementApiService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SLDJobContextCollectionPeriodModel>> GetCollectionPeriods(short fromCollectionYear)
        {           
            try
            {
                var sldResponse = await _httpClient.GetAsync($"returnperiods/?fromCollectionYear={fromCollectionYear}");

                if (sldResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"API call was successful. Retrieved collection periods for from CollectionYear: {fromCollectionYear}");
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
    }
}
