using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Models
{
    public class CollectionPeriodResponseModel : BaseCollectionPeriodServiceResponseModel
    {
        public long Id { get; set; }
        public short Period { get; set; }
        public byte? CalendarMonth { get; set; }
        public short? CalendarYear { get; set; }
        public string? Status { get; set; }
    }
}
