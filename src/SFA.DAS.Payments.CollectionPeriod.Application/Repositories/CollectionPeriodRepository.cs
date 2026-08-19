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
        public Task<CollectionPeriodModel> CollectionPeriodForCollectionYear(short? collectionYear, short? period);
        public Task UpdateCollectionPeriodSetCompleted(short collectionYear, short period);
        public Task UpdateCollectionPeriods(IEnumerable<CollectionPeriodModel> collectionPeriods);
        public Task<short> GetCurrentCollectionYear();
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

        public async Task UpdateCollectionPeriods(IEnumerable<CollectionPeriodModel> collectionPeriods)
        {
            try
            {
                foreach (var collectionPeriod in collectionPeriods)
                {
                    var existingCollectionPeriod = _paymentsDataContext.CollectionPeriod
                        .FirstOrDefault(cp => cp.AcademicYear == collectionPeriod.AcademicYear && cp.Period == collectionPeriod.Period);

                    if (existingCollectionPeriod != null)
                    {
                        existingCollectionPeriod.Status = collectionPeriod.Status;
                        _paymentsDataContext.CollectionPeriod.Update(existingCollectionPeriod);
                    }
                    else
                    {
                        _paymentsDataContext.CollectionPeriod.Add(collectionPeriod);
                    }
                }

                await _paymentsDataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for UpdateCollectionPeriods. Message {message}", ex.Message);
                throw;
            }
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

        // Retrieve the current collection year based on the current date and open collection periods
        // If there are multiple open collection periods, it will return the oldest collection year
        public async Task<short> GetCurrentCollectionYear()
        {
            try
            {
                return await _paymentsDataContext.CollectionPeriod
                    .Where(cp => cp.EndDateTime > DateTime.Today)
                    .Select(cp => cp.AcademicYear)
                    .OrderBy(cp => cp)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetCurrentCollectionYear. Message {message}", ex.Message);
                throw;
            }
        }
        public async Task UpdateCollectionPeriodSetCompleted(short collectionYear, short period)
        {
            try
            {
                var collectionPeriod = _paymentsDataContext.CollectionPeriod
                        .FirstOrDefault(cp => cp.AcademicYear == collectionYear && cp.Period == period);

                if (collectionPeriod != null)
                {
                    collectionPeriod.Status = CollectionPeriodStatus.Completed;

                    _paymentsDataContext.CollectionPeriod.Update(collectionPeriod);

                    await _paymentsDataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for UpdateCollectionPeriodSetCompleted. Collection year {collectionYear}, Period {period}. Message {message}", collectionYear, period, ex.Message);
                throw;
            }
        }
    }
}
