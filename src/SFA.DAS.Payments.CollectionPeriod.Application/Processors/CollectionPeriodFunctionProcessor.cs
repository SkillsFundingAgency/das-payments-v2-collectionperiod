using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.CollectionPeriod.Application.Mappers;
using SFA.DAS.Payments.CollectionPeriod.Application.Models;
using SFA.DAS.Payments.CollectionPeriod.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.CollectionPeriod.Application.Processors
{
    public interface ICollectionPeriodFunctionProcessor
    {
        Task<IEnumerable<CollectionYearResponseModel>> ProcessCollectionYear();

        Task<CollectionPeriodsForCollectionYearResponseModel> ProcessCollectionYear(short? collectionYear, CollectionPeriodStatus? status);

        Task<CollectionPeriodResponseModel> ProcessCollectionPeriod(short collectionYear, short period);
    }

    public class CollectionPeriodFunctionProcessor : ICollectionPeriodFunctionProcessor
    {
        private readonly ICollectionPeriodRepository _collectionPeriodRepository;
        private readonly ILogger<CollectionPeriodFunctionProcessor> _logger;
        private readonly ICollectionPeriodMapper _mapper;

        public CollectionPeriodFunctionProcessor(ICollectionPeriodRepository collectionPeriodRepository, ILogger<CollectionPeriodFunctionProcessor> logger, ICollectionPeriodMapper mapper)
        {
            _collectionPeriodRepository = collectionPeriodRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CollectionYearResponseModel>> ProcessCollectionYear()
        {
            //Retrieve a list of currently open collection years

            var openCollectionYears = await _collectionPeriodRepository.OpenCollectionYears();

            if (openCollectionYears == null || !openCollectionYears.Any())
            {
                _logger.LogInformation("No open collection years found.");
                return null;
            }

            var map = _mapper.MapToOpenCollectionYearResponseModel(openCollectionYears);

            _logger.LogInformation("Processed Open Collection Years. Retrieved {Count} open collection years", map.Count());

            return map;
        }

        public async Task<CollectionPeriodsForCollectionYearResponseModel> ProcessCollectionYear(short? collectionYear, CollectionPeriodStatus? status)
        {
            //Retrieve a list of collection periods for a given collection year.
            //Optionally filter by open/closed status.

            var collectionPeriods = await _collectionPeriodRepository.CollectionYear(collectionYear.Value, status);

            if (collectionPeriods == null || !collectionPeriods.Any())
            {
                _logger.LogInformation("No collection periods found for {CollectionYear} and status {Status}.", collectionYear.Value, status);
                return null;
            }

            var map = _mapper.MapToCollectionPeriodsForCollectionYearResponseModel(collectionPeriods, collectionYear.Value, status);

            _logger.LogInformation("Processed Collection Year. Retrieved {Count} collection periods for collection year {CollectionYear} and status {Status}.", map.Periods.Count(), collectionYear.Value, status);

            return map;
        }

        public async Task<CollectionPeriodResponseModel> ProcessCollectionPeriod(short collectionYear, short period)
        {
            //Gets Collection Period details for a given collection year and period.

            var collectionPeriod = await _collectionPeriodRepository.CollectionPeriodForCollectionYear(collectionYear, period);

            if (collectionPeriod == null)
            {
                _logger.LogInformation("No collection period found for collection year {CollectionYear} and period {Period}.", collectionYear, period);

                return null;
            }

            var map =  _mapper.MapToCollectionPeriodResponseModel(collectionPeriod);

            _logger.LogInformation("Processed Collection Period. Retrieved collection period details for collection year {CollectionYear} and period {Period}.", collectionYear, period);

            return map;
        }

    }
}
