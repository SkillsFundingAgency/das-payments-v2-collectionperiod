using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Validators
{
    public interface ICollectionPeriodHttpTriggerInputValidator
    {
        public void ValidateCollectionPeriod(short? period);
        public CollectionPeriodStatus? ValidateStatus(string? status);
        public void ValidateCollectionYearAndCollectionPeriod(short? collectionYear, short? period);
        public void ValidateCollectionYear(short? collectionYear);
    }

    public class CollectionPeriodHttpTriggerInputValidator : ICollectionPeriodHttpTriggerInputValidator
    {
        public void ValidateCollectionPeriod(short? period)
        {
            if (period.HasValue && (period < 1 || period > 14))
            {
                throw new ArgumentException("Collection period must be between 1 and 14.");
            }
        }

        public void ValidateCollectionYear(short? collectionYear)
        {
            if (collectionYear.HasValue && (collectionYear < 1000 || collectionYear > 9999))
            {
                throw new ArgumentException("Collection Year must be a 4-digit number.");
            }
        }

        public CollectionPeriodStatus? ValidateStatus(string? status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return null;
            }

            if (Enum.TryParse<CollectionPeriodStatus>(status, true, out var parsedStatus))
            {
                return parsedStatus;
            }

            throw new ArgumentException($"Invalid status value: {status}. Valid values are Open, Closed, Not Started, Completed.");
        }

        public void ValidateCollectionYearAndCollectionPeriod(short? collectionYear, short? period)
        {
            if (collectionYear.HasValue && !period.HasValue)
            {
                throw new ArgumentException("Collection Period is required when Year is specified.");
            }

            if (period.HasValue && !collectionYear.HasValue)
            {
                throw new ArgumentException("Collection Year is required when Period is specified.");
            }

            ValidateCollectionPeriod(period);
            ValidateCollectionYear(collectionYear);
        }
    }
}
