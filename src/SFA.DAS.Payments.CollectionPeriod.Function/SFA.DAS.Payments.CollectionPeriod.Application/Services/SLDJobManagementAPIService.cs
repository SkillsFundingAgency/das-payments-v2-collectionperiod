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
    public interface ISLDJobManagementAPIService
    {
        Task<List<SLDJobManagementAPICollectionPeriod>> GetCollectionPeriods();
    }

    public class SLDJobManagementAPIService : ISLDJobManagementAPIService
    {
        private readonly HttpClient _httpClient;
        public SLDJobManagementAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SLDJobManagementAPI");
        }

        public async Task<List<SLDJobManagementAPICollectionPeriod>> GetCollectionPeriods()
        {
            var sldResponse = await _httpClient.GetAsync("");

            if (sldResponse.IsSuccessStatusCode)
            {
                var result = await sldResponse.Content.ReadFromJsonAsync<List<SLDJobManagementAPICollectionPeriod>>();
                return result ?? new List<SLDJobManagementAPICollectionPeriod>();
            }

            return new List<SLDJobManagementAPICollectionPeriod>();
        }
    }
}
