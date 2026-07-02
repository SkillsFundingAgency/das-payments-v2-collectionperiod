
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Repositories
{
    public interface IPeriodEndRepository
    {
        DateTime? GetReferenceDataValidationDate(short academicYear, byte period);
    }

    public class PeriodEndRepository : IPeriodEndRepository
    {
        private readonly IPeriodEndDataContext _context;

        public PeriodEndRepository(IPeriodEndDataContext context)
        {
            _context = context;
        }

        public DateTime? GetReferenceDataValidationDate(short academicYear, byte period)
        {
            var job = _context.Job.Where(x => x.JobType == JobType.PeriodEndSubmissionWindowValidationJob
                                             && x.AcademicYear == academicYear
                                             && x.CollectionPeriod == period
                                             && x.EndTime != null)
                .OrderByDescending(x => x.EndTime)
                .FirstOrDefault();
            return job?.EndTime?.DateTime;
        }
    }
}
