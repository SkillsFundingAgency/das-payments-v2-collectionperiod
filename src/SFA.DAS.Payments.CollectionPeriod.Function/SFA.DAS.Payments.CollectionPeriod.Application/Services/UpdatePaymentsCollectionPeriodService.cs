using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Services
{
    public interface IUpdatePaymentsCollectionPeriodService
    {
        public void UpdatePaymentsCollectionPeriod (SLDJobManagementAPICollectionPeriods collectionPeriods);
    }

    public class UpdatePaymentsCollectionPeriodService : IUpdatePaymentsCollectionPeriodService
    {
        public readonly ICollectionPeriodRepository _collectionPeriodRepository;

        public UpdatePaymentsCollectionPeriodService(ICollectionPeriodRepository collectionPeriodRepository)
        {
            _collectionPeriodRepository = collectionPeriodRepository;
        }

        public void UpdatePaymentsCollectionPeriod(SLDJobManagementAPICollectionPeriods collectionPeriods)
        {
            //TODO : Map the SLDJobManagementAPICollectionPeriods to the Payments CollectionPeriod model
            //TODO : call the collection period repository to update the collection periods in the database
        }
    }
}
