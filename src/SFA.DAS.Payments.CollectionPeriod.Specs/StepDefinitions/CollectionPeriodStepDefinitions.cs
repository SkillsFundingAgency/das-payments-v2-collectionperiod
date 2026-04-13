using Reqnroll;
using SFA.DAS.Payments.Model.Core;

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
        public void GivenThatTheCollectionPeriodHasRecentlyCompleted()
        {
            throw new PendingStepException();
        }

        [Given("the next collection period has not yet been opened")]
        public void GivenTheNextCollectionPeriodHasNotYetBeenOpened()
        {
            throw new PendingStepException();
        }

        [When("a request is made to get the open collection years from the Collection Periods API")]
        public void WhenARequestIsMadeToGetTheOpenCollectionYearsFromTheCollectionPeriodsAPI()
        {
            throw new PendingStepException();
        }

        [Then("the response should contain the current collection year")]
        public void ThenTheResponseShouldContainTheCurrentCollectionYear()
        {
            throw new PendingStepException();
        }

    }
}