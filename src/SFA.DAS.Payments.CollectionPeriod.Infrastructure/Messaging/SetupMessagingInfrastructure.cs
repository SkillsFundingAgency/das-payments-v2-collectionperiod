using System.Net;
using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Messaging.ServiceBus.Administration;

namespace SFA.DAS.Payments.CollectionPeriod.Infrastructure.Messaging
{
    public class SetupMessagingInfrastructure : IHostedService
    {
        private readonly ILogger<SetupMessagingInfrastructure> _logger;
        private readonly IConfiguration _configuration;

        public SetupMessagingInfrastructure(ILogger<SetupMessagingInfrastructure> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InitialiseCollectionPeriodQueue(ServiceBusAdministrationClient adminClient, string queueName)
        {
            try
            {
                if (await adminClient.QueueExistsAsync(queueName, CancellationToken.None))
                {
                    _logger.LogInformation($"Queue '{queueName}' already exists, skipping queue creation.");
                    return;
                }

                var options = new CreateQueueOptions(queueName)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                    DeadLetteringOnMessageExpiration = true,
                    LockDuration = TimeSpan.FromMinutes(5),
                    MaxDeliveryCount = 50,
                    MaxSizeInMegabytes = 5120
                };

                await adminClient.CreateQueueAsync(options, CancellationToken.None);

                _logger.LogInformation($"Queue '{queueName}' created.");
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
            {
                _logger.LogInformation($"Queue '{queueName}' already exists: {ex.Message}. Another instance likely created it.");
            }
            catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.Conflict)
            {
                _logger.LogInformation($"Queue '{queueName}' already exists (409): {ex.Message}. Another instance likely created it.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ensuring queue '{queueName}': {ex.Message}.", ex);
                throw;
            }


        }

        public async Task InitialiseCollectionPeriodSubscription(ServiceBusAdministrationClient adminClient, string topicName, string subscriptionName, string queueName)
        {
            try
            {
                if (await adminClient.SubscriptionExistsAsync(topicName, subscriptionName, CancellationToken.None))
                {
                    _logger.LogInformation($"Subscription '{subscriptionName}' on topic '{topicName}' already exists, skipping subscription creation.");
                    return;
                }
                var options = new CreateSubscriptionOptions(topicName, subscriptionName)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                    DeadLetteringOnMessageExpiration = true,
                    LockDuration = TimeSpan.FromMinutes(5),
                    MaxDeliveryCount = 50,
                    ForwardTo = queueName
                };

                await adminClient.CreateSubscriptionAsync(options, CancellationToken.None);

                _logger.LogInformation($"Subscription '{subscriptionName}' on topic '{topicName}' created.");
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
            {
                _logger.LogInformation($"Subscription '{subscriptionName}' on topic '{topicName}' already exists: {ex.Message}. Another instance likely created it.");
            }
            catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.Conflict)
            {
                _logger.LogInformation($"Subscription '{subscriptionName}' on topic '{topicName}' already exists (409): {ex.Message}. Another instance likely created it.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ensuring subscription '{subscriptionName}' on topic '{topicName}': {ex.Message}.", ex);
                throw;
            }
        }

        public async Task CreatePeriodEndStoppedEventFilter(ServiceBusAdministrationClient adminClient, string topicName, string subscriptionName)
        {
            try
            {
                if (!await adminClient.SubscriptionExistsAsync(topicName, subscriptionName, CancellationToken.None))
                {
                    _logger.LogWarning($"Subscription '{subscriptionName}' on topic '{topicName}' does not exist. Cannot create filter.");
                    return;
                }

                var ruleName = "PeriodEndStoppedEvent";

                if (await adminClient.RuleExistsAsync(topicName, subscriptionName, ruleName, CancellationToken.None))
                {
                    _logger.LogInformation($"Rule '{ruleName}' already exists on subscription '{subscriptionName}', skipping rule creation.");
                    return;
                }

                var ruleOptions = new CreateRuleOptions(ruleName)
                {
                    Filter = new SqlRuleFilter("[NServiceBus.EnclosedMessageTypes] LIKE '%SFA.DAS.Payments.PeriodEnd.Messages.Events.PeriodEndStoppedEvent%'")
                };

                await adminClient.CreateRuleAsync(topicName, subscriptionName, ruleOptions, CancellationToken.None);

                _logger.LogInformation($"Rule '{ruleName}' with filter '{ruleOptions.Filter}' created on subscription '{subscriptionName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating rule on subscription '{subscriptionName}': {ex.Message}.", ex);
                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CollectionPeriod Function App - Start creating messaging infrastructure.");

            var serviceBusNamespace = _configuration["ServiceBusNamespace"];
            var queueName = _configuration["CollectionPeriodQueueName"];
            var topicName = _configuration["PaymentsTopicName"];
            var subscriptionName = _configuration["CollectionPeriodSubscriptionName"];

            if (string.IsNullOrEmpty(serviceBusNamespace) || 
                string.IsNullOrEmpty(queueName) ||
                string.IsNullOrEmpty(topicName) ||
                string.IsNullOrEmpty(subscriptionName))
            {
                _logger.LogWarning("One or more required configuration values are missing. Skipping messaging infrastructure setup.");
                return;
            }

            var administrationClient = new ServiceBusAdministrationClient(serviceBusNamespace, new DefaultAzureCredential());

            await InitialiseCollectionPeriodQueue(administrationClient, queueName);
            await InitialiseCollectionPeriodSubscription(administrationClient, topicName, subscriptionName, queueName);
            await CreatePeriodEndStoppedEventFilter(administrationClient, topicName, subscriptionName);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CollectionPeriod Function App - Finished creating messaging infrastructure.");
            return Task.CompletedTask;
        }
    }
}