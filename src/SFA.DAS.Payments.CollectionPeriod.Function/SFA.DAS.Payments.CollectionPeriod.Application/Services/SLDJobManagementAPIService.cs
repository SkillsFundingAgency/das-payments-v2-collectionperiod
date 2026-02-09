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
        public SLDJobManagementAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SLDJobManagementAPI");
        }

        public async Task<List<SLDJobManagementAPICollectionPeriod>> GetCollectionPeriods(string uptoDateStr)
        {
            var sldResponse = await _httpClient.GetAsync($"upto/{uptoDateStr}/ILR");

            if (sldResponse.IsSuccessStatusCode)
            {
                var result = await sldResponse.Content.ReadFromJsonAsync<List<SLDJobManagementAPICollectionPeriod>>();
                return result ?? new List<SLDJobManagementAPICollectionPeriod>();
            }

            return new List<SLDJobManagementAPICollectionPeriod>();
        }
    }
}
