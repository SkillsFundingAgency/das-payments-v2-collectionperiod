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
        private readonly IPeriodEndRepository _periodEndRepository;

        public PeriodEndStoppedEventHandler(ILogger<PeriodEndStoppedEventHandler> logger, 
                                            ICollectionPeriodRepository collectionPeriodRepository,
                                            IPeriodEndRepository periodEndRepository)
        {
            _logger = logger;
            _collectionPeriodRepository = collectionPeriodRepository;
            _periodEndRepository = periodEndRepository;
        }

        public async Task Handle(PeriodEndStoppedEvent message)
        {
            var referenceDataValidationDate = _periodEndRepository.GetReferenceDataValidationDate(message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period);
            if (referenceDataValidationDate == null)
            {
                throw new InvalidOperationException($"Failed to find successful PeriodEndSubmissionWindowValidationJob for academic year: {message.CollectionPeriod.AcademicYear} and period: {message.CollectionPeriod.Period} with an EndTime set");
            }

            await _collectionPeriodRepository.UpdateCollectionPeriodSetCompleted(message.CollectionPeriod.AcademicYear, 
                                                                                 message.CollectionPeriod.Period,
                                                                                 referenceDataValidationDate,
                                                                                 message.EventTime.DateTime);
        }
    }
}
