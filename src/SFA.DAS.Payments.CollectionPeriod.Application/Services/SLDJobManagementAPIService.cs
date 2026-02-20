using ESFA.DC.Logging.Interfaces;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using System.Net.Http.Json;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public class SLDJobManagementAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public SLDJobManagementAPIService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<SLDJobContextCollectionPeriodModel>> GetCollectionPeriods(string uptoDateStr)
        {           
            try
            {
                var sldResponse = await _httpClient.GetAsync($"{uptoDateStr}/ILR");

                if (sldResponse.IsSuccessStatusCode)
                {
                    var result = await sldResponse.Content.ReadFromJsonAsync<IEnumerable<SLDJobContextCollectionPeriodModel>>();
                    return result ?? Enumerable.Empty<SLDJobContextCollectionPeriodModel>();
                }

                return Enumerable.Empty<SLDJobContextCollectionPeriodModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while calling SLD Job Conext API to get collection periods for date: {uptoDateStr}. Exeption: {ex.Message}");
                throw;
            }
            
        }
    }
}
