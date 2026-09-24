using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Repositories
{
    public interface IPeriodEndDataContext
    {
        DbSet<JobModel> Job { get; }
    }

    public class PeriodEndDataContext : DbContext, IPeriodEndDataContext
    {
        protected readonly string connectionString;

        public virtual DbSet<JobModel> Job { get; set; }
        
        public PeriodEndDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public PeriodEndDataContext(DbContextOptions<PeriodEndDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new JobModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null)
                optionsBuilder.UseSqlServer(connectionString);
        }
    }

}
