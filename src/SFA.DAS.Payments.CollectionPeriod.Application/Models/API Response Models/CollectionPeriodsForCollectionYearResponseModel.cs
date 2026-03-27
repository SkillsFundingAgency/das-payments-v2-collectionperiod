using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Models
{
    public class CollectionPeriodsForCollectionYearResponseModel
    {
        public short Year { get; set; }
        public string Status { get; set; }
        public IEnumerable<CollectionPeriodResponseModel> Periods { get; set; }
    }
}
