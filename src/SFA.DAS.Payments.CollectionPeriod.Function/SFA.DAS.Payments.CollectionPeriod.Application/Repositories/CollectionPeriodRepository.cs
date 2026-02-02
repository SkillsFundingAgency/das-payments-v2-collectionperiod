using Microsoft.EntityFrameworkCore;
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
    }

    public class CollectionPeriodRepository : ICollectionPeriodRepository
    {
        private readonly IPaymentsDataContext _paymentsDataContext;
        public CollectionPeriodRepository(IPaymentsDataContext paymentsDataContext)
        {
            _paymentsDataContext = paymentsDataContext;
        }

        public async Task<IEnumerable<CollectionPeriodModel>> GetCollectionPeriodByAcademicYear(short academicYear)
        {
            return await _paymentsDataContext.CollectionPeriod.Where(cp => cp.AcademicYear == academicYear)
                .ToListAsync();
        }
    }
}
