using ESFA.DC.Logging.Interfaces;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<SLDJobManagementAPICollectionPeriod>> GetCollectionPeriods(string uptoDateStr)
        {
            try
            {
                var sldResponse = await _httpClient.GetAsync($"{uptoDateStr}/ILR");

                if (sldResponse.IsSuccessStatusCode)
                {
                    var result = await sldResponse.Content.ReadFromJsonAsync<IEnumerable<SLDJobManagementAPICollectionPeriod>>();
                    return result ?? Enumerable.Empty<SLDJobManagementAPICollectionPeriod>();
                }

                return Enumerable.Empty<SLDJobManagementAPICollectionPeriod>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while calling SLD Job Management API to get collection periods for date: {uptoDateStr}. Exeption: {ex.Message}");
                throw;
            }
            
        }
    }
}
