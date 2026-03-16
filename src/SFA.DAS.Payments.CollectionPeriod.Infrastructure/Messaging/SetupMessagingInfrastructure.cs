using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SFA.DAS.Payments.CollectionPeriod.Infrastructure.ServiceBus
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

        public async Task InitialiseCollectionPeriodQueue(string serviceBusConnectionString, string queueName)
        {
            try
            {
                var adminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);

                if (await adminClient.QueueExistsAsync(queueName, CancellationToken.None).ConfigureAwait(false))
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

                await adminClient.CreateQueueAsync(options, CancellationToken.None).ConfigureAwait(false);

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

        public async Task InitialiseCollectionPeriodSubscription(string serviceBusConnectionString, string topicName, string subscriptionName, string queueName)
        {
            try
            {
                var adminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);
                if (await adminClient.SubscriptionExistsAsync(topicName, subscriptionName, CancellationToken.None).ConfigureAwait(false))
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

                await adminClient.CreateSubscriptionAsync(options, CancellationToken.None).ConfigureAwait(false);

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

        public async Task CreatePeriodEndStoppedEventFilter(string topicName, string subscriptionName, string serviceBusConnectionString)
        {
            try
            {
                var adminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);

                if (!await adminClient.SubscriptionExistsAsync(topicName, subscriptionName, CancellationToken.None).ConfigureAwait(false))
                {
                    _logger.LogWarning($"Subscription '{subscriptionName}' on topic '{topicName}' does not exist. Cannot create filter.");
                    return;
                }

                var ruleName = "PeriodEndStoppedEvent";

                if (await adminClient.RuleExistsAsync(topicName, subscriptionName, ruleName, CancellationToken.None).ConfigureAwait(false))
                {
                    _logger.LogInformation($"Rule '{ruleName}' already exists on subscription '{subscriptionName}', skipping rule creation.");
                    return;
                }

                var ruleOptions = new CreateRuleOptions(ruleName)
                {
                    Filter = new SqlRuleFilter("[NServiceBus.EnclosedMessageTypes] LIKE '%SFA.DAS.Payments.PeriodEnd.Messages.Events.PeriodEndStoppedEvent%'")
                };

                await adminClient.CreateRuleAsync(topicName, subscriptionName, ruleOptions, CancellationToken.None).ConfigureAwait(false);

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
            var serviceBusConnectionString = _configuration.GetConnectionString("ServiceBusConnectionString");
            var queueName = _configuration.GetConnectionString("CollectionPeriodQueueName");
            var topicName = _configuration.GetConnectionString("PaymentsTopicName");
            var subscriptionName = _configuration.GetConnectionString("CollectionPeriodSubscriptionName");

            await InitialiseCollectionPeriodQueue(serviceBusConnectionString, queueName);
            await InitialiseCollectionPeriodSubscription(serviceBusConnectionString, topicName, subscriptionName, queueName);
            await CreatePeriodEndStoppedEventFilter(topicName, subscriptionName, serviceBusConnectionString);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}