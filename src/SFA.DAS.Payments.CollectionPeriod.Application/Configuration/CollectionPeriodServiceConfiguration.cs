using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Configuration
{
    public class CollectionPeriodServiceConfiguration
    {
        public string PaymentsConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string CollectionPeriodApiBaseAddress { get; set; }
        public string SLDJobManagementAPIEndpoint { get; set; }
        public string SyncCollectionPeriodsSchedule { get; set; }

        public string CollectionPeriodQueueName { get; set; }
        public string PaymentsTopicName { get; set; }
        public string CollectionPeriodSubscriptionName { get; set; }
    }
}
