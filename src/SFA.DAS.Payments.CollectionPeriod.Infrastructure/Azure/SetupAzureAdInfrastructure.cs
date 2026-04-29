using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.CollectionPeriod.Infrastructure.Azure
{
    public interface ISetupAzureAdInfrastructure
    {
        ClientSecretCredential GetAzureAdConfig();
        string GetAzureAdToken(ClientSecretCredential clientSecret);
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
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                }
            );
        }

        public string GetAzureAdToken(ClientSecretCredential clientSecret)
        {
            throw new NotImplementedException();
        }
    }
}
