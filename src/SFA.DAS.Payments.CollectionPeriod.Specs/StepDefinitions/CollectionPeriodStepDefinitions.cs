using System.Net.Http.Json;
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
        private readonly TestSession testSession;
        private Model.Core.CollectionPeriod collectionPeriod;
        private short currentAcademicYear;

        public CollectionPeriodStepDefinitions(ScenarioContext scenarioContext, MessagingContext messagingContext, TestSession testSession)
        {
            this.scenarioContext = scenarioContext;
            this.messagingContext = messagingContext;
            this.testSession = testSession;
        }

        protected void SetCurrentCollectionYear()
        {
            currentAcademicYear = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build().AcademicYear;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            SetCurrentCollectionYear();
            Console.WriteLine($"UKPRN : {testSession.Provider.Ukprn}, ULN: {testSession.Learner.Uln}, collection year: {currentAcademicYear}");
        }

        [AfterScenario]
        public void AfterScenario()
        {
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

        private List<CollectionYear> OpenYears;

        [When("a request is made to get the open collection years from the Collection Periods API")]
        public async Task WhenARequestIsMadeToGetTheOpenCollectionYearsFromTheCollectionPeriodsAPI()
        {

            OpenYears = await testSession.HttpClient.GetFromJsonAsync<List<CollectionYear>>("collectionYear");
        }

        public class CollectionYear
        {
            public short Year { get; set; }
            public short Status { get; set; }
        }

        [Then("the response should contain the current collection year")]
        public void ThenTheResponseShouldContainTheCurrentCollectionYear()
        {
            OpenYears.Any(openYear => openYear.Year.Equals(currentAcademicYear));
        }
    }
}