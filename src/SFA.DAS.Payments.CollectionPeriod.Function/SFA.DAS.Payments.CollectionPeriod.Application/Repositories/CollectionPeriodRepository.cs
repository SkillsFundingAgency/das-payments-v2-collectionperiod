using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Repositories
{
    public interface ICollectionPeriodRepository
    {
        Task<IEnumerable<CollectionPeriodModel>> GetCollectionPeriodByAcademicYear(short academicYear);
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

        public async Task<IEnumerable<CollectionPeriodModel>> GetCollectionPeriodByAcademicYear(short academicYear)
        {
            try
            {
                return await _paymentsDataContext.CollectionPeriod
                    .Where(cp => cp.AcademicYear == academicYear && cp.IsOpen == true)
                    .ToListAsync();
            }
            catch (Exception ex){
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetCollectionPeriodByAcademicYear. Message {message}", ex.Message);
                throw;
            }
        }
    }
}
