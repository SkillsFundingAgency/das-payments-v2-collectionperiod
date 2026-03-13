using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Handlers;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class PeriodEndStoppedServiceBusTrigger
{
    private readonly ILogger<PeriodEndStoppedServiceBusTrigger> _logger;
    private readonly IPeriodEndStoppedEventHandler _periodEndStoppedEventHandler;

    public PeriodEndStoppedServiceBusTrigger(ILogger<PeriodEndStoppedServiceBusTrigger> logger, IPeriodEndStoppedEventHandler periodEndStoppedEventHandler)
    {
        _logger = logger;
        _periodEndStoppedEventHandler = periodEndStoppedEventHandler;
    }

    [Function(nameof(PeriodEndStoppedServiceBusTrigger))]
    public async Task Run(
        [ServiceBusTrigger("%QueueName%", Connection = "%ServiceBusConnectionString%")]
        ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("PeriodEndStoppedServiceBusTrigger function executed at: {executionTime}", DateTime.Now);

        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        var msg = JsonSerializer.Deserialize<PeriodEndStoppedEvent>(message.Body.ToString());

        await _periodEndStoppedEventHandler.Handle(msg);

        await messageActions.CompleteMessageAsync(message);
    }
}