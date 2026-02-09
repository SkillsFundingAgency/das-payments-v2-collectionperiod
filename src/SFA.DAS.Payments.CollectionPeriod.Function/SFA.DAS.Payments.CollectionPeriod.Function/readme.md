## App Settings Required for Local Development

Add the following at the root of the function app in a file labelled `local.settings.json`

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "PaymentsConnectionString": ""
  },
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey="
  }
}

```
