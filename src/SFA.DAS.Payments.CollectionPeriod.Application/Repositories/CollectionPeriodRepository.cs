using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Repositories
{
    public interface ICollectionPeriodRepository
    {
        public Task<IEnumerable<short>> OpenCollectionYears();
        public Task<IEnumerable<CollectionPeriodModel>> CollectionYear(short collectionYear, CollectionPeriodStatus? status);
        public Task<CollectionPeriodsForCollectionYearResponseModel> CollectionPeriodsForCollectionYear(short? collectionYear, CollectionPeriodStatus? status);
        public Task<CollectionPeriodModel> CollectionPeriodForCollectionYear(short? collectionYear, short? period);
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


        public async Task<IEnumerable<CollectionPeriodModel>> CollectionYear(short collectionYear, CollectionPeriodStatus? status)
        {
            try
            {
                if (status != null)
                {
                    return await _paymentsDataContext.CollectionPeriod
                    .Where(cp => cp.Status == status && cp.AcademicYear == collectionYear)
                    .ToListAsync();
                }

                return await _paymentsDataContext.CollectionPeriod
                    .Where(cp => cp.AcademicYear == collectionYear)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetOpenCollectionPeriods. Message {message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<short>> OpenCollectionYears()
        {
            try
            {
                return await _paymentsDataContext.CollectionPeriod
                    .Where(cp => cp.Status == CollectionPeriodStatus.Open)
                    .Select(cp => cp.AcademicYear)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetOpenCollectionPeriods. Message {message}", ex.Message);
                throw;
            }
        }

        // Retrieve collection periods for a given collection year, optionally filtered by status
        public async Task<CollectionPeriodsForCollectionYearResponseModel?> CollectionPeriodsForCollectionYear(short? collectionYear, CollectionPeriodStatus? status)
        {
            throw new NotImplementedException();
        }

        // Retrieve a specific collection period for a given collection year and period number
        public async Task<CollectionPeriodModel?> CollectionPeriodForCollectionYear(short? collectionYear, short? period)
        {
            try
            {
                return await _paymentsDataContext.CollectionPeriod
                    .FirstOrDefaultAsync(cp => cp.AcademicYear == collectionYear && cp.Period == period);
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetAllCollectionPeriods. Message {message}", ex.Message);
                throw;
            }
        }

    }
}
