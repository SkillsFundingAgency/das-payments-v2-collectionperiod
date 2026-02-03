using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Models
{
    public class SLDJobManagementAPIResponseModel
    {
        public SLDJobManagementAPICollectionPeriods[] CollectionPeriods { get; set; }        
    }

    public class SLDJobManagementAPICollectionPeriods
    {
        public int ReturnPeriodId { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public byte PeriodNumber { get; set; }
        public string CollectionName { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
        public int CollectionId { get; set; }
        public short CollectionYear { get; set; }
        public bool IsOpen { get; set; }
    }

}
