using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Handlers
{
    public interface IPeriodEndStoppedEventHandler
    {
        Task Handle(PeriodEndStoppedEvent message);
    }
    public class PeriodEndStoppedEventHandler : IPeriodEndStoppedEventHandler
    {
        private readonly ILogger<PeriodEndStoppedEventHandler> _logger;
        private readonly ICollectionPeriodRepository _collectionPeriodRepository;

        public PeriodEndStoppedEventHandler(ILogger<PeriodEndStoppedEventHandler> logger, ICollectionPeriodRepository collectionPeriodRepository)
        {
            _logger = logger;
            _collectionPeriodRepository = collectionPeriodRepository;
        }

        public async Task Handle(PeriodEndStoppedEvent message)
        {
            await _collectionPeriodRepository.UpdateCollectionPeriodSetCompleted(message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period);
        }
    }
}
