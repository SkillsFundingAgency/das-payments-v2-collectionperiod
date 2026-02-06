using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Models
{
    public class SLDJobManagementAPICollectionPeriod
    {
        public byte PeriodNumber { get; set; }
        public short CollectionYear { get; set; }
        public bool IsOpen { get; set; }
    }

}
