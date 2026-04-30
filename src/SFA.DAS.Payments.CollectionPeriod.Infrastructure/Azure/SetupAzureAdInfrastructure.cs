using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure
{
    public interface ISetupAzureAdInfrastructure
    {
        ClientSecretCredential GetAzureAdConfig();
        Task<string> GetAzureAdToken(ClientSecretCredential clientSecret);
    }

    public class SetupAzureAdInfrastructure : ISetupAzureAdInfrastructure
    {
        private readonly ILogger<SetupAzureAdInfrastructure> _logger;
        private readonly IConfiguration _config;

        public SetupAzureAdInfrastructure(ILogger<SetupAzureAdInfrastructure> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }   

        public ClientSecretCredential GetAzureAdConfig()
        {
            var clientId = _config["ClientId"];
            var tenantId = _config["TenantId"];
            var clientSecret = _config["ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientSecret))
            {
                _logger.LogError("Missing configuration for Azure AD. Please ensure ClientId, TenantId, and ClientSecret are set.");
                throw new Exception("Missing configuration for Azure AD. Please ensure ClientId, TenantId, and ClientSecret are set.");
            }

            return new ClientSecretCredential(
                tenantId, clientId, clientSecret, new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                }
            );
        }

        public async Task<string> GetAzureAdToken(ClientSecretCredential credential)
        {
            var audience = _config["Audience"];

            if (string.IsNullOrEmpty(audience))
                throw new ArgumentException("Audience is missing");

            var tokenRequestContext = new TokenRequestContext([$"{audience}/.default"]);

            var token = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);

            return token.Token;
        }
    }
}
