using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Repositories
{
    public interface ICollectionPeriodRepository
    {
        Task<IEnumerable<CollectionPeriodModel>> GetCollectionPeriodByAcademicYear(short academicYear);
        Task UpdateCollectionPeriods(IEnumerable<CollectionPeriodModel> collectionPeriods);
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
                return await _paymentsDataContext.CollectionPeriod.Where(cp => cp.AcademicYear == academicYear)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("SQL Error - CollectionPeriodRepository for GetCollectionPeriodByAcademicYear. Message {message}", ex.Message);
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
                        existingCollectionPeriod.IsOpen = collectionPeriod.IsOpen;
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
    }
}
