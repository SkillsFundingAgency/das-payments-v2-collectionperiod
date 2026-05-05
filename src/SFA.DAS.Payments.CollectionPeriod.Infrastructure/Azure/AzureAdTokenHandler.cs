using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure
{
    public class AzureAdTokenHandler : DelegatingHandler
    {
        private readonly ClientSecretCredential _credential;
        private readonly IConfiguration _config;

        public AzureAdTokenHandler(ClientSecretCredential credential, IConfiguration config)
        {
            _credential = credential;
            _config = config;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var audience = _config["Audience"];

            if (string.IsNullOrEmpty(audience))
                throw new ArgumentException("Audience is missing");

            var tokenRequestContext = new TokenRequestContext([$"{audience}/.default"]);

            var token = await _credential.GetTokenAsync(tokenRequestContext, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
