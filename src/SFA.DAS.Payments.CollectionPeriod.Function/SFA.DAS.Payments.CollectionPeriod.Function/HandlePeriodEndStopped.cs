using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.CollectionPeriod.Function;

public class HandlePeriodEndStopped
{
    private readonly ILogger<HandlePeriodEndStopped> _logger;

    public HandlePeriodEndStopped(ILogger<HandlePeriodEndStopped> logger)
    {
        _logger = logger;
    }

    [Function(nameof(HandlePeriodEndStopped))]
    public async Task Run(
        [ServiceBusTrigger("myqueue", Connection = "")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        var msg = JsonSerializer.Deserialize<PeriodEndStoppedEvent>(message.Body.ToString());



        await messageActions.CompleteMessageAsync(message);
    }
}