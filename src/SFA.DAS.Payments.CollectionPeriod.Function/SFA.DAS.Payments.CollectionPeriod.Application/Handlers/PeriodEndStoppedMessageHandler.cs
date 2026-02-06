using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Services;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Handlers
{
    public interface IPeriodEndStoppedMessageHandler
    {
        void Handle(PeriodEndStoppedEvent message);
    }

    public class PeriodEndStoppedMessageHandler : IPeriodEndStoppedMessageHandler
    {
        private readonly IUpdatePaymentsCollectionPeriodService _updatePaymentsCollectionPeriodService;
        private readonly ILogger<PeriodEndStoppedMessageHandler> _logger;

        public PeriodEndStoppedMessageHandler(
            IUpdatePaymentsCollectionPeriodService updatePaymentsCollectionPeriodService,
            ILogger<PeriodEndStoppedMessageHandler> logger)
        {
            _updatePaymentsCollectionPeriodService = updatePaymentsCollectionPeriodService;
            _logger = logger;
        }

        public void Handle(PeriodEndStoppedEvent message)
        {
            _updatePaymentsCollectionPeriodService.UpdatePaymentsCollectionPeriod(message);
        }
    }
}
