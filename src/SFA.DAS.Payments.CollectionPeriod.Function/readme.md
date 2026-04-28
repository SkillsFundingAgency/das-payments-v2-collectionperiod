## App Settings Required for Local Development

Add the following at the root of the function app in a file labelled `local.settings.json`

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "PaymentsConnectionString": "",
    "SLDJobManagementAPIEndpoint": "",
    "SyncCollectionPeriodsSchedule": "" //CRON format eg. "*/2 * * * *"
    "QueueName": "",
    "CollectionPeriodQueueName": "",
    "PaymentsTopicName": "",
    "CollectionPeriodSubscriptionName": "",
  },
  "ApplicationInsights": {
    "ConnectionString": ""
  },
  "ConnectionStrings": {
    "ServiceBusConnectionString": ""
  }
}

```

To run the Acceptance Tests locally, you will also need to add the following at the root of the `SFA.DAS.Payments.CollectionPeriod.Specs` project in a file labelled `appSettings.json`
```
{
  "ConnectionStrings": {
    "StorageConnectionString": "UseDevelopmentStorage=true",
    "PaymentsConnectionString": "",
    "ServiceBusConnectionString": ""
  },
  "CollectionPeriodAPIBaseUrl": "http://localhost:7069/api/"
}
```