using Microsoft.Azure.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Models
{
    public class CollectionPeriodStatusesResponseModel
    {
        public string[] Status =>
        [
            "NotStarted",      //1
            "Open",            //2
            "Closed",         //3
            "Completed",     //4
        ];
    }
}
