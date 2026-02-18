using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Repositories
{
    public interface ICollectionPeriodRepository
    {
        Task<IEnumerable<CollectionPeriodModel>> GetOpenCollectionPeriods();
        Task<IEnumerable<CollectionPeriodModel>> GetAllCollectionPeriods();
    }

    public class CollectionPeriodRepository : ICollectionPeriodRepository
    {
        private readonly IPaymentsDataContext _paymentsDataContext;
        private readonly ILogger<CollectionPeriodRepository> _logger;

        public CollectionPeriodRepository(IPaymentsDataContext paymentsDataContext, ILogger<CollectionPeriodRepository> logger)
        {
            _paymentsDataContext = paymentsDataContext;
            _logger = logger;
        }

        public async Task<IEnumerable<CollectionPeriodModel>> GetAllCollectionPeriods()
        {
            try
            {
                return await _paymentsDataContext.CollectionPeriod.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetAllCollectionPeriods. Message {message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CollectionPeriodModel>> GetOpenCollectionPeriods()
        {
            try
            {
                return await _paymentsDataContext.CollectionPeriod
                    .Where(cp => cp.Status == CollectionPeriodStatus.Open)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetOpenCollectionPeriods. Message {message}", ex.Message);
                throw;
            }
        }
    }
}
