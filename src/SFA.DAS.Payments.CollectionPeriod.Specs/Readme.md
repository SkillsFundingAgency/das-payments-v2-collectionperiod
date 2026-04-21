## App Settings Required to run the ATs locally

Add the following at the root of the function app in a file labelled `appSettings.json`

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
