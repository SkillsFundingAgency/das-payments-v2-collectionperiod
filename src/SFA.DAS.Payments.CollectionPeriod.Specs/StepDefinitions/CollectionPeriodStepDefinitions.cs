using System;
using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using Reqnroll;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.CollectionPeriod.Specs.StepDefinitions
{
    [Binding]
    public class CollectionPeriodStepDefinitions
    {
        private readonly ScenarioContext scenarioContext;
        private readonly MessagingContext messagingContext;
        private TestSession testSession;
        private Model.Core.CollectionPeriod collectionPeriod;
        private short currentAcademicYear;

        public CollectionPeriodStepDefinitions(ScenarioContext scenarioContext, MessagingContext messagingContext)
        {
            this.scenarioContext = scenarioContext;
            this.messagingContext = messagingContext;            
        }

        protected void SetCurrentCollectionYear()
        {
            currentAcademicYear = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build().AcademicYear;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            testSession = new TestSession();
            await testSession.DataContext.ClearCollectionPeriodsData();
            SetCurrentCollectionYear();
            Console.WriteLine($"UKPRN : {testSession.Provider.Ukprn}, ULN: {testSession.Learner.Uln}, collection year: {currentAcademicYear}");
        }

        [AfterScenario]
        public void AfterScenario()
        {
        }


        [Given("that the collection period has opened recently")]
        public async Task GivenThatTheCollectionPeriodHasOpenedRecently()
        {
            var period = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build();
            testSession.DataContext.CollectionPeriods.Add(new CollectionPeriodModel
            {
                AcademicYear = period.AcademicYear,
                CalendarMonth = (byte)DateTime.Today.Month,
                CalendarYear = (byte)DateTime.Today.Year,
                CompletionDate = DateTime.Today,
                EndDateTime = null,
                Period = period.Period,
                ReferenceDataValidationDate = null,
                StartDateTime = DateTime.Today,
                Status = CollectionPeriodStatus.Open
            });
            await testSession.DataContext.SaveChangesAsync();
        }


        [Given("that the collection period has recently completed")]
        public async Task GivenThatTheCollectionPeriodHasRecentlyCompleted()
        {
            var period = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build();
            testSession.DataContext.CollectionPeriods.Add(new CollectionPeriodModel
            {
                AcademicYear = period.AcademicYear,
                CalendarMonth = (byte)DateTime.Today.Month,
                CalendarYear = (byte)DateTime.Today.Year,
                CompletionDate = DateTime.Today,
                EndDateTime = DateTime.Today,
                Period = period.Period,
                ReferenceDataValidationDate = DateTime.Today,
                StartDateTime = DateTime.Today.AddMonths(-1),
                Status = CollectionPeriodStatus.Completed
            });
            await testSession.DataContext.SaveChangesAsync();
        }

        [Given("the next collection period has not yet been opened")]
        public async Task GivenTheNextCollectionPeriodHasNotYetBeenOpened()
        {
            var periodMonth = DateTime.Today.AddMonths(1);
            var period = new CollectionPeriodBuilder().WithDate(periodMonth).Build();

            testSession.DataContext.CollectionPeriods.Add(new CollectionPeriodModel
            {
                AcademicYear = period.AcademicYear,
                CalendarMonth = (byte)periodMonth.Month,
                CalendarYear = (byte)periodMonth.Year,
                CompletionDate = periodMonth,
                EndDateTime = null,
                Period = period.Period,
                ReferenceDataValidationDate = null,
                StartDateTime = periodMonth.AddMonths(-1),
                Status = CollectionPeriodStatus.NotStarted
            });
            await testSession.DataContext.SaveChangesAsync();
        }

        [Given("that the collection period R01 is currently open")]
        public async Task GivenThatTheCollectionPeriodRIsCurrentlyOpen()
        {
            var periodMonth = new DateTime(DateTime.Today.Year,8,1);
            var period = new CollectionPeriodBuilder().WithDate(periodMonth).Build();

            testSession.DataContext.CollectionPeriods.Add(new CollectionPeriodModel
            {
                AcademicYear = period.AcademicYear,
                CalendarMonth = 8,
                CalendarYear = (byte)periodMonth.Year,
                CompletionDate = periodMonth.AddMonths(1),
                EndDateTime = null,
                Period = 1,
                ReferenceDataValidationDate = null,
                StartDateTime = periodMonth,
                Status = CollectionPeriodStatus.Open
            });

            await testSession.DataContext.SaveChangesAsync();
        }

        [Given("collection period R13 is also open")]
        public async Task GivenCollectionPeriodRIsAlsoOpen()
        {
            var periodMonth = new DateTime(DateTime.Today.Year, 8, 1);
            var period = new CollectionPeriodBuilder().WithDate(periodMonth).Build();

            testSession.DataContext.CollectionPeriods.Add(new CollectionPeriodModel
            {
                AcademicYear = (short)(period.AcademicYear - 101),
                CalendarMonth = 8,
                CalendarYear = (byte)periodMonth.Year,
                CompletionDate = periodMonth.AddMonths(1),
                EndDateTime = null,
                Period = 13,
                ReferenceDataValidationDate = null,
                StartDateTime = periodMonth,
                Status = CollectionPeriodStatus.Open
            });

            await testSession.DataContext.SaveChangesAsync();
        }

        private List<CollectionYear> OpenYears;

        [When("a request is made to get the open collection years from the Collection Periods API")]
        public async Task WhenARequestIsMadeToGetTheOpenCollectionYearsFromTheCollectionPeriodsAPI()
        {
            var response = await testSession.HttpClient.GetAsync("collectionYear");

            OpenYears = response.StatusCode == HttpStatusCode.NoContent
                ? null
                : await response.Content.ReadFromJsonAsync<List<CollectionYear>>();
        }

        public class CollectionYear
        {
            public short Year { get; set; }
            public string Status { get; set; }
        }

        [Then("the response should contain the current collection year")]
        public void ThenTheResponseShouldContainTheCurrentCollectionYear()
        {
            Assert.That(OpenYears, Is.Not.Null);
            Assert.That(OpenYears, Has.Count.EqualTo(1));
            Assert.That(OpenYears.Any(x => x.Year == currentAcademicYear), Is.True);
        }

        [Then("the response should contain the both open collection years")]
        public void ThenTheResponseShouldContainTheBothOpenCollectionYears()
        {
            Assert.That(OpenYears, Is.Not.Null);
            Assert.That(OpenYears, Has.Count.EqualTo(2));
            Assert.That(OpenYears.Any(x => x.Year == currentAcademicYear), Is.True);
            Assert.That(OpenYears.Any(x => x.Year == currentAcademicYear - 101), Is.True);
        }

        [When("the response should not contain the current collection year")]
        public void WhenTheResponseShouldNotContainTheCurrentCollectionYear()
        {
            Assert.That(OpenYears, Is.Null);
        }

    }
}